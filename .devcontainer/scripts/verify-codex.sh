#!/bin/bash
# Codex overlay verification script

set -e

echo "🔍 Verifying Codex overlay setup..."

# Check if bubblewrap is installed for Codex sandboxing on Linux
if ! command -v bwrap &> /dev/null; then
    echo "✗ bubblewrap is not installed or not in PATH"
    exit 1
fi

echo "✓ bubblewrap is installed: $(bwrap --version 2>/dev/null | head -n 1 || echo 'installed')"

# Check if codex CLI is installed
if ! command -v codex &> /dev/null; then
    echo "✗ codex CLI is not installed or not in PATH"
    exit 1
fi

echo "✓ codex CLI is installed: $(codex --version 2>/dev/null || echo 'installed')"

# Check .codex directory exists
if [ -d "$HOME/.codex" ]; then
    echo "✓ .codex directory exists: $HOME/.codex"
else
    echo "⚠️  .codex directory not found at $HOME/.codex"
fi

echo ""
echo "✅ Codex overlay verification complete!"
echo ""

echo "💡 Tips:"
echo "  - Run 'codex --help' to see available commands"
echo "  - Your Codex home directory is: $HOME/.codex"
echo "  - Ensure 'codex' is available on your PATH inside the devcontainer"
