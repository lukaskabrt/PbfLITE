# NuGet Package Publishing Setup

This document describes the one-time setup required to enable automated NuGet package publishing.

## Required Setup

### 1. Create a NuGet.org Account

If you don't already have one:

1. Go to https://www.nuget.org/
2. Sign up for a free account
3. Verify your email address

### 2. Generate a NuGet API Key

1. Sign in to https://www.nuget.org/
2. Go to your account settings: https://www.nuget.org/account/apikeys
3. Click "Create"
4. Configure the API key:
   - **Key Name**: `PbfLite-GitHub-Actions` (or any descriptive name)
   - **Expiration**: Choose an appropriate expiration date (e.g., 365 days)
   - **Select Scopes**: Choose "Push" and "Push new packages and package versions"
   - **Select Packages**: Choose "Glob Pattern"
   - **Glob Pattern**: Enter `PbfLite*` to restrict to PbfLite packages only
5. Click "Create"
6. **Important**: Copy the generated API key immediately - it will only be shown once!

### 3. Add API Key to GitHub Secrets

1. Go to your GitHub repository: https://github.com/lukaskabrt/PbfLITE
2. Navigate to Settings → Secrets and variables → Actions
3. Click "New repository secret"
4. Configure the secret:
   - **Name**: `NUGET_API_KEY` (must match exactly)
   - **Value**: Paste the API key you copied from NuGet.org
5. Click "Add secret"

### 4. Verify the Setup

Once the API key is configured:

1. The release workflow will be able to publish packages automatically
2. Test the workflow by creating a test tag (you can delete it after):
   ```bash
   git tag -a v0.0.1-test -m "Test release workflow"
   git push origin v0.0.1-test
   ```
3. Check the Actions tab to see if the workflow runs successfully
4. If successful, delete the test tag and package:
   ```bash
   git tag -d v0.0.1-test
   git push origin :refs/tags/v0.0.1-test
   ```
   And unlist the test package on NuGet.org

## Security Best Practices

- **Never commit the API key**: The key should only be stored in GitHub Secrets
- **Use appropriate expiration**: Set a reasonable expiration date for the API key
- **Limit scope**: The API key should only have push permissions for the PbfLite package
- **Rotate regularly**: Create a new API key before the old one expires
- **Monitor usage**: Check NuGet.org for unexpected package versions

## Troubleshooting

### Package push fails with 403 Forbidden
- Verify the NUGET_API_KEY secret is correctly set in GitHub
- Check if the API key has expired on NuGet.org
- Ensure the API key has push permissions for the package

### Package push fails with 409 Conflict
- The version already exists on NuGet.org
- Update the version in `src/PbfLite/PbfLite.csproj`
- Create a new tag with the updated version

### Cannot find NUGET_API_KEY secret
- Ensure the secret name is exactly `NUGET_API_KEY` (case-sensitive)
- Verify you have the necessary permissions to create secrets in the repository
- Check that the secret is created in the correct repository

## Support

For issues with:
- **NuGet.org account or API keys**: Contact NuGet.org support
- **GitHub Secrets**: Contact GitHub support or check repository settings
- **Release workflow**: Check the Actions tab for detailed error logs

## Additional Resources

- [NuGet API Keys Documentation](https://docs.microsoft.com/en-us/nuget/nuget-org/publish-a-package#managing-api-keys)
- [GitHub Secrets Documentation](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [PbfLite Release Process](../RELEASING.md)
