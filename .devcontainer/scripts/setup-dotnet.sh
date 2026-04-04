#!/bin/bash
# .NET setup script - Install global tools and configure environment

set -e

# Source shared setup utilities (provides run_spinner)
# shellcheck source=setup-utils.sh
source "$(dirname "${BASH_SOURCE[0]}")/setup-utils.sh"

# Suppress the .NET SDK first-run welcome banner and telemetry noise
export DOTNET_NOLOGO=1
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

# Extract overlay name from script filename (setup-<overlay>.sh -> <overlay>)
OVERLAY_NAME=$(basename "$0" | sed 's/setup-//;s/\.sh$//')

echo "🔧 Installing .NET global tools..."

# Read tools from configuration file if it exists
if [ -f ".devcontainer/global-tools-${OVERLAY_NAME}.txt" ]; then
    echo "📦 Installing tools from global-tools-${OVERLAY_NAME}.txt..."
    
    while IFS= read -r line || [ -n "$line" ]; do
        # Skip comments and empty lines
        [[ "$line" =~ ^#.*$ ]] && continue
        [[ -z "$line" ]] && continue
        
        # Parse tool name and optional version (format: tool::version)
        if [[ "$line" =~ ^(.+)::(.+)$ ]]; then
            tool="${BASH_REMATCH[1]}"
            version="${BASH_REMATCH[2]}"
            run_spinner "$tool@$version" dotnet tool install --global "$tool" --version "$version" || true
        else
            tool="$line"
            run_spinner "$tool" dotnet tool install --global "$tool" || true
        fi
    done < ".devcontainer/global-tools-${OVERLAY_NAME}.txt"
else
    # Fallback to hardcoded list
    echo "📦 Installing default .NET global tools..."
    run_spinner "dotnet-ef" dotnet tool install --global dotnet-ef || true
    run_spinner "dotnet-format" dotnet tool install --global dotnet-format || true
    run_spinner "dotnet-outdated-tool" dotnet tool install --global dotnet-outdated-tool || true
fi

# Verify installations
echo "✓ .NET global tools installed:"
dotnet tool list --global

# Restore project dependencies if solution/project exists
if [ -f "*.sln" ] || [ -f "*.csproj" ] || [ -f "*.fsproj" ]; then
    echo "📦 Restoring .NET dependencies..."
    dotnet restore || true
    echo "✓ .NET dependencies restored"
fi

echo "✓ .NET setup complete"
