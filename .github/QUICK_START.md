# Quick Start Guide for PbfLite NuGet Package

## What Was Set Up

This PR configures PbfLite for automated NuGet package publishing. Here's what's ready to go:

### ✅ Completed Automatically
- **NuGet Package Metadata**: Version 1.0.0, MIT License, full description
- **LICENSE file**: MIT License added to repository
- **CHANGELOG.md**: Ready for tracking version history
- **README.md**: Updated with NuGet installation instructions
- **Release Workflow**: Automated GitHub Actions workflow for releases
- **PR Validation**: Updated to validate package builds on every PR

### 🔧 Required Manual Setup (One-Time)

**You need to configure the NuGet API key in GitHub Secrets**

Quick steps:
1. Go to https://www.nuget.org/account/apikeys
2. Create an API key with push permissions for PbfLite
3. Copy the key (shown only once!)
4. Go to GitHub repository Settings → Secrets → Actions
5. Create secret named `NUGET_API_KEY` with the key value

📖 **Detailed instructions**: See `.github/NUGET_SETUP.md`

## How to Create Your First Release

Once the API key is configured:

```bash
# 1. Update version in src/PbfLite/PbfLite.csproj
# 2. Update CHANGELOG.md with release notes
# 3. Commit changes
git add src/PbfLite/PbfLite.csproj CHANGELOG.md
git commit -m "Prepare release v1.0.0"
git push

# 4. Create and push a tag
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0
```

The release workflow will automatically:
- ✅ Build the project
- ✅ Run all tests
- ✅ Create NuGet package
- ✅ Publish to NuGet.org
- ✅ Create GitHub Release

📖 **Full release process**: See `RELEASING.md`

## Testing the Setup

To verify everything works without publishing:

```bash
# Build and pack locally
dotnet build src/PbfLite.slnx --configuration Release
dotnet pack src/PbfLite/PbfLite.csproj --configuration Release --output ./test-nupkg

# Check the package
ls -lh ./test-nupkg/
unzip -l ./test-nupkg/PbfLite.1.0.0.nupkg

# Clean up
rm -rf ./test-nupkg/
```

## Package Information

- **Package ID**: `PbfLite`
- **Initial Version**: `1.0.0`
- **Target Framework**: .NET 8.0
- **License**: MIT
- **Repository**: https://github.com/lukaskabrt/PbfLITE

## Support & Documentation

- **NuGet Setup**: `.github/NUGET_SETUP.md`
- **Release Process**: `RELEASING.md`
- **Version History**: `CHANGELOG.md`
- **Workflows**: `.github/workflows/`

## Questions?

If you have questions about:
- Setting up NuGet API key → See `.github/NUGET_SETUP.md`
- Creating releases → See `RELEASING.md`
- Package metadata → Check `src/PbfLite/PbfLite.csproj`
