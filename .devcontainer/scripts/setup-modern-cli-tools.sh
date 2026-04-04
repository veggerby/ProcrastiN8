#!/bin/bash
# Modern CLI tools setup script
# jq, ripgrep, fd, bat are installed via cross-distro-packages devcontainer feature.
# This script installs yq (not in standard repos), creates Debian symlinks, and configures tools.

set -e

echo "🚀 Setting up modern CLI tools..."

# Install yq (YAML processor — not in standard distro repos)
echo "📦 Installing yq..."
YQ_VERSION="${YQ_VERSION:-4.52.2}"
ARCH=$(uname -m)
case "$ARCH" in
    x86_64)  YQ_ARCH="amd64" ;;
    aarch64|arm64) YQ_ARCH="arm64" ;;
    *) echo "⚠️  Unsupported architecture: $ARCH, defaulting to amd64"; YQ_ARCH="amd64" ;;
esac

curl -fsSL "https://github.com/mikefarah/yq/releases/download/v${YQ_VERSION}/yq_linux_${YQ_ARCH}" \
    -o /tmp/yq
sudo install -m 0755 /tmp/yq /usr/local/bin/yq
rm /tmp/yq

if command -v yq &>/dev/null; then
    echo "✓ yq installed: $(yq --version)"
else
    echo "✗ yq installation failed"
    exit 1
fi

# Create fd symlink — Debian installs the binary as 'fdfind'
if command -v fdfind &>/dev/null && ! command -v fd &>/dev/null; then
    sudo ln -sf "$(which fdfind)" /usr/local/bin/fd
    echo "✓ fd symlink created (fdfind → fd)"
fi

# Create bat symlink — Debian installs the binary as 'batcat'
if command -v batcat &>/dev/null && ! command -v bat &>/dev/null; then
    sudo ln -sf "$(which batcat)" /usr/local/bin/bat
    echo "✓ bat symlink created (batcat → bat)"
fi

# Configure bat
mkdir -p "$HOME/.config/bat"
cat > "$HOME/.config/bat/config" << 'EOF'
# bat configuration
--theme="Monokai Extended"
--style="numbers,changes,header"
--italic-text=always
--paging=auto
EOF

# Create shell aliases
ALIAS_FILE="$HOME/.bash_aliases"
cat > "$ALIAS_FILE" << 'EOF'
# Modern CLI tools aliases

# bat aliases
alias cat='bat'
alias less='bat'

# ripgrep aliases
alias grep='rg'

# fd aliases
alias find='fd'

# jq pretty print
alias jqp='jq -C . | bat -l json'

# yq pretty print
alias yqp='yq -C | bat -l yaml'
EOF

echo "✓ Shell aliases created in $ALIAS_FILE"

# Source aliases in .bashrc if not already done
if [ -f "$HOME/.bashrc" ]; then
    if ! grep -q ".bash_aliases" "$HOME/.bashrc"; then
        echo "" >> "$HOME/.bashrc"
        echo "# Load modern CLI tool aliases" >> "$HOME/.bashrc"
        echo "if [ -f ~/.bash_aliases ]; then" >> "$HOME/.bashrc"
        echo "    . ~/.bash_aliases" >> "$HOME/.bashrc"
        echo "fi" >> "$HOME/.bashrc"
    fi
fi

echo "✓ Modern CLI tools setup complete"
echo ""
echo "💡 Installed tools:"
echo "  - jq:      JSON processor"
echo "  - yq:      YAML/XML/TOML processor"
echo "  - ripgrep: Fast search (rg, aliased as grep)"
echo "  - fd:      Fast find (aliased as find)"
echo "  - bat:     Cat with syntax highlighting (aliased as cat/less)"
echo ""
echo "Examples:"
echo "  cat file.json     # Syntax-highlighted with bat"
echo "  rg 'pattern' .    # Fast recursive search"
echo "  fd '*.js'         # Fast file finding"
echo "  echo '{}' | jq    # Pretty-print JSON"
echo "  cat config.yml | yq  # Process YAML"
