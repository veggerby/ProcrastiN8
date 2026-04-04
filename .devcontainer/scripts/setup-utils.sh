#!/bin/bash
# Shared setup utilities for container-superposition overlay scripts.
# Sourced automatically: source "$(dirname "${BASH_SOURCE[0]}")/setup-utils.sh"
#
# All setup scripts for selected overlays run in PARALLEL via postCreateCommand.
# Use the locking helpers below for any apt/dpkg operations.

# Suppress terminal-capability probes (OSC queries, colour-detection) and npm
# update notices that show up as garbage or noise in devcontainer build logs.
export NO_COLOR=1
export NPM_CONFIG_UPDATE_NOTIFIER=false
# TERM=dumb prevents most tools from querying cursor position or emitting ANSI
# escape sequences in non-interactive build logs.
export TERM=dumb

# ---------------------------------------------------------------------------
# Spinner helper
# ---------------------------------------------------------------------------

# Run a command silently while showing a spinner and a label.
# Stdout/stderr of the command are suppressed; only pass/fail is reported.
# Usage: run_spinner "Installing foo..." <command> [args...]
# Exit code: the exit code of <command>.
run_spinner() {
    local label="$1"; shift
    local rc

    if [ -t 1 ]; then
        # Interactive TTY: animated spinner with carriage-return overwriting.
        local spin='⠋⠙⠹⠸⠼⠴⠦⠧⠇⠏'
        local i=0
        "$@" &>/dev/null &
        local pid=$!
        while kill -0 "$pid" 2>/dev/null; do
            printf "\r  %s  %s" "${spin:$((i % ${#spin})):1}" "$label"
            sleep 0.1
            (( i++ )) || true
        done
        wait "$pid"; rc=$?
        if [ "$rc" -eq 0 ]; then
            printf "\r  ✓  %s\n" "$label"
        else
            printf "\r  ✗  %s (exit %d)\n" "$label" "$rc"
        fi
    else
        # Non-TTY (devcontainer build log, CI): emit a start line immediately
        # so the log shows progress, then a done line when the command finishes.
        # No carriage-return tricks — plain newlines so each line is visible.
        printf "  ⋯  %s\n" "$label"
        "$@" &>/dev/null; rc=$?
        if [ "$rc" -eq 0 ]; then
            printf "  ✓  %s\n" "$label"
        else
            printf "  ✗  %s (exit %d)\n" "$label" "$rc"
        fi
    fi
    return $rc
}

# ---------------------------------------------------------------------------
# Node.js / npm helpers
# ---------------------------------------------------------------------------

# Ensure npm (and node) are on PATH by loading nvm if needed.
# The devcontainer node feature installs via nvm; postCreateCommand scripts
# run in a non-interactive shell that may not have nvm on PATH.
# Safe to call multiple times (idempotent).
# Usage: load_nvm
load_nvm() {
    if command -v npm &>/dev/null; then
        return 0  # already on PATH
    fi
    local nvm_dir="${NVM_DIR:-/usr/local/share/nvm}"
    # shellcheck disable=SC1091
    if [ -s "$nvm_dir/nvm.sh" ]; then
        TERM=dumb \. "$nvm_dir/nvm.sh" --no-use 2>/dev/null || true
    fi
    # Fallback: add the nvm current symlink bin directory to PATH
    if ! command -v npm &>/dev/null && [ -d "$nvm_dir/current/bin" ]; then
        export PATH="$nvm_dir/current/bin:$PATH"
    fi
}

# ---------------------------------------------------------------------------
# APT / DPKG helpers
# ---------------------------------------------------------------------------

# Advisory lock file shared across all parallel postCreateCommand scripts.
_CS_APT_LOCK=/tmp/.cs-apt.lock

# Acquire an exclusive flock-based lock for apt/dpkg operations.
# Blocks until the lock is available or 300 s elapses (hard failure).
# The kernel releases the lock automatically on process exit for any reason.
acquire_apt_lock() {
    exec 9>"$_CS_APT_LOCK"
    echo "⏳ Acquiring apt lock..."
    flock -w 300 9 || {
        echo "❌ Timed out waiting for apt lock after 300s"
        return 1
    }
}

# Release the lock acquired by acquire_apt_lock.
release_apt_lock() {
    flock -u 9 2>/dev/null || true
    exec 9>&- 2>/dev/null || true
}

# Run a command inside an apt lock, releasing it even on failure.
# Usage: with_apt_lock <command> [args...]
# Example: with_apt_lock sudo apt-get install -y ripgrep
with_apt_lock() {
    acquire_apt_lock || return 1
    "$@"
    local rc=$?
    release_apt_lock
    return $rc
}

# Run apt-get update + apt-get install under a single held lock.
# Usage: apt_install <package> [packages...]
apt_install() {
    acquire_apt_lock || return 1
    # Pass DEBIAN_FRONTEND and TERM explicitly: sudo strips environment by default.
    sudo DEBIAN_FRONTEND=noninteractive TERM=dumb apt-get update -qq
    sudo DEBIAN_FRONTEND=noninteractive TERM=dumb apt-get install -y -qq "$@"
    local rc=$?
    release_apt_lock
    return $rc
}

# @deprecated — kept for backward compat; delegates to acquire_apt_lock.
wait_for_apt_lock() {
    acquire_apt_lock
}

# Add a third-party apt repository with a GPG key.
# Does NOT require the apt lock — only writes key/sources files.
# Caller is responsible for calling apt-get update afterward (inside a lock).
# Usage: add_apt_repo <key-url> <keyring-path> <repo-line> <sources-file>
add_apt_repo() {
    local key_url="$1"
    local keyring_path="$2"
    local repo_line="$3"
    local sources_file="$4"

    sudo mkdir -p "$(dirname "$keyring_path")" "$(dirname "$sources_file")"
    curl -fsSL "$key_url" | sudo gpg --dearmor -o "$keyring_path"
    echo "$repo_line" | sudo tee "$sources_file" >/dev/null
}

# ---------------------------------------------------------------------------
# Idempotency helpers
# ---------------------------------------------------------------------------

# Returns 0 if the command exists on PATH.
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Returns 0 if an apt package is installed.
apt_package_installed() {
    dpkg -s "$1" >/dev/null 2>&1
}

# ---------------------------------------------------------------------------
# Architecture detection
# ---------------------------------------------------------------------------

# Sets CS_ARCH to "amd64" or "arm64" based on uname -m.
# Usage:
#   detect_arch             # returns 1 on unknown arch
#   detect_arch amd64       # falls back to amd64 on unknown arch
detect_arch() {
    local fallback="${1:-}"
    local raw
    raw=$(uname -m)
    case "$raw" in
        x86_64)         CS_ARCH="amd64" ;;
        aarch64|arm64)  CS_ARCH="arm64" ;;
        *)
            if [ -n "$fallback" ]; then
                echo "⚠️  Unsupported architecture: $raw, falling back to $fallback"
                CS_ARCH="$fallback"
            else
                echo "❌ Unsupported architecture: $raw"
                return 1
            fi
            ;;
    esac
    export CS_ARCH
}

# ---------------------------------------------------------------------------
# Binary installation helpers
# ---------------------------------------------------------------------------

# Download a single binary, install it to /usr/local/bin, clean up on failure.
# Usage: install_binary <url> <name> [mode]
install_binary() {
    local url="$1"
    local name="$2"
    local mode="${3:-0755}"
    local tmp
    tmp=$(mktemp) || return 1
    curl -fsSL "$url" -o "$tmp" || { rm -f "$tmp"; return 1; }
    sudo install -m "$mode" "$tmp" "/usr/local/bin/$name"
    local rc=$?
    rm -f "$tmp"
    return $rc
}

# Download a .tar.gz, extract a named binary, install to /usr/local/bin.
# Usage: install_binary_from_tar <url> <bin-in-archive> [dest-name] [mode]
install_binary_from_tar() {
    local url="$1"
    local bin_in_archive="$2"
    local dest_name="${3:-$bin_in_archive}"
    local mode="${4:-0755}"
    local tmpdir
    tmpdir=$(mktemp -d) || return 1
    curl -fsSL "$url" -o "${tmpdir}/archive.tar.gz" || { rm -rf "$tmpdir"; return 1; }
    tar -xzf "${tmpdir}/archive.tar.gz" -C "$tmpdir" "$bin_in_archive" || { rm -rf "$tmpdir"; return 1; }
    sudo install -m "$mode" "${tmpdir}/${bin_in_archive}" "/usr/local/bin/${dest_name}"
    local rc=$?
    rm -rf "$tmpdir"
    return $rc
}
