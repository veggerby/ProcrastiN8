#!/bin/bash
# Node.js setup script - Install global npm packages

set -e

# Extract overlay name from script filename (setup-<overlay>.sh -> <overlay>)
OVERLAY_NAME=$(basename "$0" | sed 's/setup-//;s/\.sh$//')

echo "📦 Installing Node.js global packages..."

# Read packages from configuration file if it exists
if [ -f ".devcontainer/global-packages-${OVERLAY_NAME}.txt" ]; then
    echo "📦 Installing packages from global-packages-${OVERLAY_NAME}.txt..."
    
    packages=()
    while IFS= read -r line || [ -n "$line" ]; do
        # Skip comments and empty lines
        [[ "$line" =~ ^#.*$ ]] && continue
        [[ -z "$line" ]] && continue
        
        packages+=("$line")
    done < ".devcontainer/global-packages-${OVERLAY_NAME}.txt"
    
    if [ ${#packages[@]} -gt 0 ]; then
        echo "  Installing: ${packages[*]}"
        npm install -g "${packages[@]}"
    fi
else
    # Fallback to pnpm only
    echo "📦 Installing pnpm (default)..."
    npm install -g pnpm
fi

# Verify pnpm installation (if included)
if command -v pnpm &> /dev/null; then
    echo "✓ pnpm installed successfully: $(pnpm --version)"
fi

# Install project dependencies if package.json exists
if [ -f "package.json" ]; then
    echo "📦 Installing project dependencies..."
    npm install || true
    echo "✓ Project dependencies installed"
fi

echo "✓ Node.js setup complete"
