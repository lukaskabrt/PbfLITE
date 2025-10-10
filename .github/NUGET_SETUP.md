# NuGet Package Publishing Setup

This document describes the one-time setup required to enable automated NuGet package publishing.

## Required Setup

### 1. Create a NuGet.org Account

### 2. Generate a NuGet API Key

1. Sign in to https://www.nuget.org/
2. Go to your account settings: https://www.nuget.org/account/apikeys
3. Click "Create"
4. Configure the API key:
   - **Select Packages**: Choose "Glob Pattern"
   - **Glob Pattern**: Enter `PbfLite*` to restrict to PbfLite packages only
5. Click "Create"

### 3. Add API Key to GitHub Secrets

1. Go to your GitHub repository: https://github.com/lukaskabrt/PbfLITE
2. Navigate to Settings → Secrets and variables → Actions
3. Click "New repository secret"
4. Configure the secret:
   - **Name**: `NUGET_API_KEY` (must match exactly)
   - **Value**: Paste the API key you copied from NuGet.org
5. Click "Add secret"

## Additional Resources

- [NuGet API Keys Documentation](https://docs.microsoft.com/en-us/nuget/nuget-org/publish-a-package#managing-api-keys)
- [GitHub Secrets Documentation](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [PbfLite Release Process](../RELEASING.md)
