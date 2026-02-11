#!/bin/bash
# Verification script for .NET overlay
# Confirms .NET SDK is installed

set -e

echo "🔍 Verifying .NET overlay..."
echo ""

# Check dotnet is installed
echo "1️⃣ Checking .NET SDK..."
if command -v dotnet &> /dev/null; then
    dotnet --version
    echo "   ✅ .NET SDK found"
else
    echo "   ❌ .NET SDK not found"
    exit 1
fi

# List installed SDKs
echo ""
echo "2️⃣ Installed .NET SDKs:"
dotnet --list-sdks

echo ""
echo "✅ .NET overlay verification complete"
