#!/bin/bash
# Git helpers setup script

set -e

echo "🔧 Setting up Git helpers..."

# Verify Git installation
if command -v git &> /dev/null; then
    echo "✓ Git installed: $(git --version)"
else
    echo "✗ Git not found"
    exit 1
fi

# Verify Git LFS installation
if command -v git-lfs &> /dev/null; then
    echo "✓ Git LFS installed: $(git-lfs version)"
    # Initialize Git LFS for the user
    git lfs install --skip-repo
else
    echo "✗ Git LFS not found"
    exit 1
fi

# Verify GitHub CLI installation
if command -v gh &> /dev/null; then
    echo "✓ GitHub CLI installed: $(gh --version | head -n 1)"
else
    echo "✗ GitHub CLI not found"
    exit 1
fi

# Set up SSH permissions if .ssh directory exists
if [ -d "$HOME/.ssh" ]; then
    chmod 700 "$HOME/.ssh"
    chmod 600 "$HOME/.ssh"/* 2>/dev/null || true
    echo "✓ SSH directory permissions configured"
fi

# Set up GPG permissions if .gnupg directory exists
if [ -d "$HOME/.gnupg" ]; then
    chmod 700 "$HOME/.gnupg"
    chmod 600 "$HOME/.gnupg"/* 2>/dev/null || true
    echo "✓ GPG directory permissions configured"
fi

# Create sample .gitconfig-include if it doesn't exist
if [ ! -f "$HOME/.gitconfig-devcontainer" ]; then
    cat > "$HOME/.gitconfig-devcontainer" << 'EOF'
# Git configuration for devcontainer
# Include this in your .gitconfig with:
# [include]
#     path = ~/.gitconfig-devcontainer

[core]
    autocrlf = input
    filemode = false

[push]
    default = current
    autoSetupRemote = true

[pull]
    rebase = true

[rebase]
    autoStash = true

# Useful aliases
[alias]
    st = status
    co = checkout
    br = branch
    ci = commit
    unstage = reset HEAD --
    last = log -1 HEAD
    visual = log --graph --oneline --all --decorate
    amend = commit --amend --no-edit
EOF
    echo "✓ Sample .gitconfig-devcontainer created"
fi

echo "✓ Git helpers setup complete"
echo ""
echo "💡 Tips:"
echo "  - SSH keys are mounted from your host ~/.ssh directory (read-only)"
echo "  - GPG keys are mounted from your host ~/.gnupg directory"
echo "  - To enable commit signing, set git.enableCommitSigning in VS Code settings"
echo "  - Authenticate with GitHub CLI: gh auth login"
echo "  - Initialize Git LFS in your repo: git lfs install"
