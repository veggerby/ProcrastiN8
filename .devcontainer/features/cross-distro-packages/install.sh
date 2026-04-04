#!/bin/bash
# Cross-distribution package installer
# Automatically detects package manager (apt vs apk) and installs appropriate packages

set -e

APT_PACKAGES="${APT:-}"
APK_PACKAGES="${APK:-}"

# Function to deduplicate space-separated package list
deduplicate_packages() {
    local packages="$1"
    # Convert to array, sort, remove duplicates, join back
    echo "$packages" | tr ' ' '\n' | sort -u | tr '\n' ' ' | sed 's/ $//'
}

resolve_package_token() {
    local manager="$1"
    local token="$2"
    local IFS='|'
    read -r -a candidates <<< "$token"

    for candidate in "${candidates[@]}"; do
        if [ -z "$candidate" ]; then
            continue
        fi

        if [ "$manager" = "apt" ]; then
            if apt-cache show "$candidate" >/dev/null 2>&1; then
                echo "$candidate"
                return 0
            fi
        elif [ "$manager" = "apk" ]; then
            if apk search -x "$candidate" >/dev/null 2>&1; then
                echo "$candidate"
                return 0
            fi
        fi
    done

    return 1
}

resolve_package_list() {
    local manager="$1"
    local packages="$2"
    local resolved=()

    for token in $packages; do
        local selected=""
        if ! selected=$(resolve_package_token "$manager" "$token"); then
            echo "❌ No package candidate found for \"$token\" using $manager" >&2
            exit 1
        fi
        resolved+=("$selected")
    done

    deduplicate_packages "${resolved[*]}"
}

# Exit early if no packages specified
if [ -z "$APT_PACKAGES" ] && [ -z "$APK_PACKAGES" ]; then
    echo "⚠️  No packages specified for installation"
    exit 0
fi

echo "📦 Installing cross-distro packages..."

# Detect package manager and install
if command -v apk > /dev/null 2>&1; then
    # Alpine Linux (apk)
    if [ -n "$APK_PACKAGES" ]; then
        APK_PACKAGES=$(resolve_package_list "apk" "$APK_PACKAGES")

        echo "  Detected: Alpine Linux (apk)"
        echo "  Installing: $APK_PACKAGES"
        apk add --no-cache $APK_PACKAGES
        echo "✓ Packages installed via apk"
    else
        echo "⚠️  No apk packages specified, skipping"
    fi
elif command -v apt-get > /dev/null 2>&1; then
    # Debian/Ubuntu (apt)
    if [ -n "$APT_PACKAGES" ]; then
        apt-get update
        APT_PACKAGES=$(resolve_package_list "apt" "$APT_PACKAGES")

        echo "  Detected: Debian/Ubuntu (apt)"
        echo "  Installing: $APT_PACKAGES"
        DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends $APT_PACKAGES
        apt-get clean
        rm -rf /var/lib/apt/lists/*
        echo "✓ Packages installed via apt"
    else
        echo "⚠️  No apt packages specified, skipping"
    fi
else
    echo "❌ Unsupported package manager (neither apk nor apt-get found)"
    exit 1
fi
