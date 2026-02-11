#!/bin/bash
# Verification script for Node.js overlay
# Confirms Node.js and npm are installed

set -e

echo "🔍 Verifying Node.js overlay..."
echo ""

# Check node is installed
echo "1️⃣ Checking Node.js..."
if command -v node &> /dev/null; then
    node --version
    echo "   ✅ Node.js found"
else
    echo "   ❌ Node.js not found"
    exit 1
fi

# Check npm is installed
echo ""
echo "2️⃣ Checking npm..."
if command -v npm &> /dev/null; then
    npm --version
    echo "   ✅ npm found"
else
    echo "   ❌ npm not found"
    exit 1
fi

echo ""
echo "✅ Node.js overlay verification complete"
