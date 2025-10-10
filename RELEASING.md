# Release Process

This document describes how to create a new release of PbfLite.

## Prerequisites

1. Ensure all tests pass
2. Update CHANGELOG.md with release notes
3. Update version in `src/PbfLite/PbfLite.csproj`
4. Ensure you have NuGet API key configured as GitHub secret (`NUGET_API_KEY`)

## Versioning Strategy

PbfLite follows [Semantic Versioning](https://semver.org/):

- **MAJOR** version: incompatible API changes
- **MINOR** version: new functionality in a backward-compatible manner
- **PATCH** version: backward-compatible bug fixes

Version is manually updated in `src/PbfLite/PbfLite.csproj` in the `<Version>` property.

## Release Steps

### 1. Prepare the Release

1. Create a new branch for the release:
   ```bash
   git checkout -b release/vX.Y.Z
   ```

2. Update the version in `src/PbfLite/PbfLite.csproj`:
   ```xml
   <Version>X.Y.Z</Version>
   ```

3. Update `CHANGELOG.md`:
   - Move items from `[Unreleased]` to a new `[X.Y.Z]` section
   - Add the release date
   - Update the comparison links at the bottom

4. Commit the changes:
   ```bash
   git add src/PbfLite/PbfLite.csproj CHANGELOG.md
   git commit -m "Prepare release vX.Y.Z"
   ```

5. Push the branch and create a Pull Request:
   ```bash
   git push origin release/vX.Y.Z
   ```

6. Wait for PR validation to pass and get approval

7. Merge the PR to master

### 2. Create and Push the Release Tag

1. After the PR is merged, checkout and update master:
   ```bash
   git checkout master
   git pull origin master
   ```

2. Create an annotated tag:
   ```bash
   git tag -a vX.Y.Z -m "Release version X.Y.Z"
   ```

3. Push the tag to GitHub:
   ```bash
   git push origin vX.Y.Z
   ```

### 3. Automated Release Process

Once the tag is pushed, the release workflow (`.github/workflows/release.yml`) will automatically:

1. Build the project in Release configuration
2. Run all tests
3. Pack the NuGet package
4. Publish to NuGet.org
5. Create a GitHub Release with the package attached

### 4. Verify the Release

1. Check the GitHub Actions workflow run to ensure it completed successfully
2. Verify the package appears on NuGet.org: https://www.nuget.org/packages/PbfLite/
3. Check the GitHub Releases page: https://github.com/lukaskabrt/PbfLITE/releases

## Tag Naming Convention

Tags must follow the format `vX.Y.Z` where:
- `v` is a literal prefix
- `X.Y.Z` is the semantic version number

Examples:
- `v1.0.0` - Initial release
- `v1.1.0` - Minor version with new features
- `v1.1.1` - Patch version with bug fixes
- `v2.0.0` - Major version with breaking changes

## Release Notes Template

When updating CHANGELOG.md, use the following structure:

```markdown
## [X.Y.Z] - YYYY-MM-DD

### Added
- New features

### Changed
- Changes to existing functionality

### Deprecated
- Features that will be removed in upcoming releases

### Removed
- Removed features

### Fixed
- Bug fixes

### Security
- Security fixes
```

## NuGet API Key Setup

The NuGet API key must be configured as a GitHub secret:

1. Go to https://www.nuget.org/account/apikeys
2. Create a new API key with push permissions for PbfLite
3. In GitHub repository settings, go to Secrets and Variables → Actions
4. Create a new secret named `NUGET_API_KEY` with the API key value

## Troubleshooting

### Release workflow fails at NuGet push
- Verify the NUGET_API_KEY secret is correctly set
- Check if the version already exists on NuGet.org
- Ensure the API key has push permissions

### Package validation errors
- Run `dotnet pack` locally to validate the package
- Check that all required metadata is present in the .csproj file
- Verify README.md and LICENSE files are included

### Tests fail during release
- Ensure all tests pass locally before creating the tag
- Check if there are environment-specific test failures
- Review the workflow logs for details
