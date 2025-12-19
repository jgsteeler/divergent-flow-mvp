#!/bin/bash

# Setup script for Divergent Flow development environment
# Configures Git to use conventional commits

set -e

echo "🚀 Setting up Divergent Flow development environment..."
echo ""

# Check if we're in a git repository
if [ ! -d .git ]; then
    echo "❌ Error: Not in a git repository"
    exit 1
fi

# Configure git commit template
echo "📝 Configuring Git commit template..."
git config commit.template .github/commit_template.txt
echo "✅ Git commit template configured"
echo "   Now when you run 'git commit', you'll see a helpful template"
echo ""

# Install commitlint dependencies
echo "📦 Installing commitlint dependencies..."
if command -v npm &> /dev/null; then
    npm install --save-dev @commitlint/cli@19 @commitlint/config-conventional@19
    echo "✅ Commitlint dependencies installed"
else
    echo "⚠️  npm not found - skipping commitlint installation"
    echo "   Please install Node.js and run: npm install"
fi
echo ""

# Offer to install commit-msg hook (optional)
echo "🔧 Optional: Install git commit-msg hook for validation?"
echo "   This will validate your commit messages before committing"
read -p "Install commit-msg hook? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    cat > .git/hooks/commit-msg << 'EOF'
#!/bin/bash
# Validate commit message using commitlint

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if commitlint is available
if ! command -v npx &> /dev/null; then
    echo -e "${YELLOW}⚠️  npx not found - skipping commit message validation${NC}"
    exit 0
fi

if ! npx commitlint --version &> /dev/null 2>&1; then
    echo -e "${YELLOW}⚠️  commitlint not found - skipping commit message validation${NC}"
    echo -e "${YELLOW}   Run: npm install --save-dev @commitlint/cli @commitlint/config-conventional${NC}"
    exit 0
fi

# Validate the commit message
if ! npx commitlint --edit "$1" --verbose; then
    echo ""
    echo -e "${RED}❌ Commit message validation failed!${NC}"
    echo ""
    echo -e "${YELLOW}Please use the Conventional Commits format:${NC}"
    echo "  <type>(<scope>): <description>"
    echo ""
    echo "Examples:"
    echo "  feat(capture): add keyboard shortcut"
    echo "  fix(review): correct priority sorting"
    echo "  docs(readme): update installation instructions"
    echo ""
    echo "See .github/copilot-instructions.md for complete guidelines."
    exit 1
fi

echo -e "${GREEN}✅ Commit message validated${NC}"
EOF
    
    chmod +x .git/hooks/commit-msg
    echo "✅ commit-msg hook installed"
    echo "   Your commit messages will now be validated before committing"
else
    echo "⏭️  Skipping commit-msg hook installation"
fi
echo ""

# Display configuration summary
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "✅ Setup complete!"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "📚 Next steps:"
echo "   1. Run 'git commit' to see the helpful commit template"
echo "   2. Read CONTRIBUTING.md for contribution guidelines"
echo "   3. See .github/copilot-instructions.md for complete docs"
echo ""
echo "💡 Tips:"
echo "   • All commits MUST follow Conventional Commits format"
echo "   • Use: <type>(<scope>): <description>"
echo "   • Example: feat(capture): add keyboard shortcut"
echo ""
echo "🔗 Resources:"
echo "   • Conventional Commits: https://www.conventionalcommits.org/"
echo "   • Contributing Guide: CONTRIBUTING.md"
echo "   • Commit Template: .github/commit_template.txt"
echo ""
