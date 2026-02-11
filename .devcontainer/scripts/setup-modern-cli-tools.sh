#!/bin/bash
# Modern CLI tools setup script

set -e

echo "🚀 Setting up modern CLI tools..."

# Update package list
sudo apt-get update -qq

# Install jq (JSON processor)
echo "📦 Installing jq..."
sudo apt-get install -y jq
if command -v jq &> /dev/null; then
    echo "✓ jq installed: $(jq --version)"
else
    echo "✗ jq installation failed"
    exit 1
fi

# Install yq (YAML processor)
echo "📦 Installing yq..."
YQ_VERSION="4.52.2"
ARCH=$(uname -m)

# SHA256 checksums for yq v4.52.2
if [ "$ARCH" = "x86_64" ]; then
    YQ_ARCH="amd64"
    YQ_CHECKSUM="a74bd266990339e0c48a2103534aef692abf99f19390d12c2b0ce6830385c459"
elif [ "$ARCH" = "aarch64" ] || [ "$ARCH" = "arm64" ]; then
    YQ_ARCH="arm64"
    YQ_CHECKSUM="c82856ac30da522f50dcdd4f53065487b5a2927e9b87ff637956900986f1f7c2"
else
    echo "⚠️  Unsupported architecture: $ARCH, defaulting to amd64"
    YQ_ARCH="amd64"
    YQ_CHECKSUM="a74bd266990339e0c48a2103534aef692abf99f19390d12c2b0ce6830385c459"
fi

curl -L "https://github.com/mikefarah/yq/releases/download/v${YQ_VERSION}/yq_linux_${YQ_ARCH}" \
    -o /tmp/yq

# Verify checksum
echo "🔐 Verifying yq checksum..."
echo "${YQ_CHECKSUM}  /tmp/yq" | sha256sum -c - || {
    echo "✗ yq checksum verification failed"
    rm /tmp/yq
    exit 1
}

sudo mv /tmp/yq /usr/local/bin/yq
sudo chmod +x /usr/local/bin/yq

if command -v yq &> /dev/null; then
    echo "✓ yq installed: $(yq --version)"
else
    echo "✗ yq installation failed"
    exit 1
fi

# Install ripgrep (fast grep alternative)
echo "📦 Installing ripgrep..."
sudo apt-get install -y ripgrep
if command -v rg &> /dev/null; then
    echo "✓ ripgrep installed: $(rg --version | head -n 1)"
else
    echo "✗ ripgrep installation failed"
    exit 1
fi

# Install fd (fast find alternative)
echo "📦 Installing fd..."
sudo apt-get install -y fd-find
# Create symlink for 'fd' command (package installs as 'fdfind' on Debian)
sudo ln -sf $(which fdfind) /usr/local/bin/fd
if command -v fd &> /dev/null; then
    echo "✓ fd installed: $(fd --version)"
else
    echo "✗ fd installation failed"
    exit 1
fi

# Install bat (better cat with syntax highlighting)
echo "📦 Installing bat..."
sudo apt-get install -y bat
# Create symlink for 'bat' command (package installs as 'batcat' on Debian)
sudo ln -sf $(which batcat) /usr/local/bin/bat
if command -v bat &> /dev/null; then
    echo "✓ bat installed: $(bat --version)"
else
    echo "✗ bat installation failed"
    exit 1
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
