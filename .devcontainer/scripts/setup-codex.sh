#!/bin/bash
# Codex setup script - Install OpenAI Codex CLI

set -e

echo "📦 Installing OpenAI Codex CLI..."

# Install @openai/codex globally
npm install -g @openai/codex

# Verify codex installation
if command -v codex &> /dev/null; then
    echo "✓ Codex CLI installed successfully: $(codex --version 2>/dev/null || echo 'installed')"
else
    echo "✗ Codex CLI installation failed"
    exit 1
fi

# Create .codex directory for configuration
mkdir -p "$HOME/.codex"

echo "✓ Codex setup complete"
echo "ℹ️  OpenAI Codex CLI: https://github.com/openai/openai-codex-cli"
echo "ℹ️  Configuration directory: $HOME/.codex"
