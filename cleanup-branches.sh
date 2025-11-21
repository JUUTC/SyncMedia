#!/bin/bash
# Branch Cleanup Script for SyncMedia Repository
# Generated: 2025-11-21
# Purpose: Delete superseded and merged branches

set -e  # Exit on error

echo "=========================================="
echo "SyncMedia Branch Cleanup Script"
echo "=========================================="
echo ""
echo "This script will delete the following branches:"
echo ""
echo "✅ MERGED BRANCHES:"
echo "  - copilot/add-microsoft-ads-features (merged via PR#7)"
echo "  - copilot/consolidate-and-merge-branches (PR#7 itself)"
echo "  - copilot/migrate-to-windows-app-storage (merged via PR#2)"
echo "  - feature/deep-review-improvements (merged via PR#7)"
echo ""
echo "🗑️ SUPERSEDED BRANCHES:"
echo "  - copilot/review-existing-features-options (incomplete, planning only)"
echo "  - copilot/upgrade-to-dotnet-10 (incompatible Windows Forms)"
echo ""
echo "⚠️  NOT DELETING:"
echo "  - copilot/fix-16197614... (needs review for docs)"
echo "  - master (active branch)"
echo "  - copilot/analyze-branches-and-prs (current PR)"
echo ""

read -p "Are you sure you want to proceed? (yes/no): " confirm

if [ "$confirm" != "yes" ]; then
    echo "Aborted."
    exit 0
fi

echo ""
echo "Starting cleanup..."
echo ""

# Delete merged branches
echo "Deleting merged branches..."
git push origin --delete copilot/add-microsoft-ads-features || echo "  ⚠️  Branch may already be deleted"
git push origin --delete copilot/consolidate-and-merge-branches || echo "  ⚠️  Branch may already be deleted"
git push origin --delete copilot/migrate-to-windows-app-storage || echo "  ⚠️  Branch may already be deleted"
git push origin --delete feature/deep-review-improvements || echo "  ⚠️  Branch may already be deleted"

echo ""
echo "Deleting superseded branches..."
git push origin --delete copilot/review-existing-features-options || echo "  ⚠️  Branch may already be deleted"
git push origin --delete copilot/upgrade-to-dotnet-10 || echo "  ⚠️  Branch may already be deleted"

echo ""
echo "=========================================="
echo "✅ Cleanup Complete!"
echo "=========================================="
echo ""
echo "Branches deleted: 6"
echo ""
echo "⚠️  MANUAL ACTION REQUIRED:"
echo ""
echo "1. Review and close PR#4 (copilot/add-microsoft-ads-features)"
echo "   - Already merged via PR#7"
echo "   - https://github.com/JUUTC/SyncMedia/pull/4"
echo ""
echo "2. Review copilot/fix-16197614... branch for useful docs:"
echo "   - Contains Store compliance docs"
echo "   - Contains feature parity analysis"
echo "   - Contains Linux roadmap"
echo "   - Consider cherry-picking before deletion"
echo ""
echo "3. Clean up local branches (optional):"
echo "   git fetch --prune"
echo "   git branch -vv | grep ': gone]' | awk '{print \$1}' | xargs git branch -D"
echo ""
