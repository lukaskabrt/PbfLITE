# Release Process

This document describes how to create a new release of PbfLite.

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

## Tag Naming Convention

Tags must follow the format `vX.Y.Z` where:
- `v` is a literal prefix
- `X.Y.Z` is the semantic version number

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