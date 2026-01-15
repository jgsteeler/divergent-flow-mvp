# GitHub Environments Setup Guide

This guide explains how to configure GitHub Environments for the Divergent Flow deployment pipeline, including protection rules, secrets management, and approval workflows.

## Overview

The deployment workflow uses two GitHub Environments:

- **staging**: Deploys automatically on PR creation/updates and main branch pushes
- **production**: Requires manual approval, deploys only when release PR is merged

### Benefits of Using GitHub Environments

1. **Simplified Secrets Management**: Single set of secrets per environment (no `_STAGING`/`_PROD` suffixes)
2. **Deployment Protection**: Require manual approval for production deployments
3. **Environment URLs**: Direct links to deployed environments in GitHub UI
4. **Deployment History**: Track all deployments per environment
5. **Status Tracking**: Visual deployment status in pull requests and commits
6. **Reviewer Assignment**: Control who can approve production deployments

## 1. Create GitHub Environments

### Step 1: Navigate to Environment Settings

1. Go to your GitHub repository
2. Click **Settings** > **Environments**
3. Click **New environment**

### Step 2: Create Staging Environment

1. Name: `staging`
2. Click **Configure environment**
3. **Environment protection rules**: Leave disabled (staging should deploy automatically)
4. **Environment secrets**: Will be added in next section
5. Click **Add environment**

**Environment URL**: Add `https://divergent-flow-staging.netlify.app` (or your custom staging domain)

### Step 3: Create Production Environment

1. Click **New environment** again
2. Name: `production`
3. Click **Configure environment**
4. **Enable required reviewers**:
   - Check **Required reviewers**
   - Add yourself and any other maintainers who should approve production deployments
   - Optionally set **Prevent self-review** if you want peer review requirements
5. **Deployment branches**: (Optional, recommended)
   - Select **Selected branches**
   - Add rule: `main` (only main branch can deploy to production)
6. **Environment URL**: Add `https://divergent-flow-prod.netlify.app` (or your custom production domain)
7. Click **Add environment**

## 2. Configure Environment Secrets

Environment secrets are scoped to specific environments, eliminating the need for duplicating secrets with `_STAGING`/`_PROD` suffixes.

### Staging Environment Secrets

Navigate to **Settings** > **Environments** > **staging** > **Environment secrets**

Add the following secrets:

| Secret Name | Value | Description |
|-------------|-------|-------------|
| `FLY_API_TOKEN` | `<your-fly-token>` | Fly.io API token for deployments |
| `NETLIFY_AUTH_TOKEN` | `<your-netlify-token>` | Netlify personal access token |
| `NETLIFY_SITE_ID` | `<staging-site-id>` | Netlify staging site API ID |
| `VITE_API_URL` | `https://divergent-flow-api-staging.fly.dev` | Backend API URL for staging |
| `VITE_AUTH0_DOMAIN` | `<your-auth0-domain>` | Auth0 tenant domain (e.g., `dev-xyz.us.auth0.com`) |
| `VITE_AUTH0_CLIENT_ID` | `<staging-client-id>` | Auth0 application client ID for staging |
| `VITE_AUTH0_AUDIENCE` | `<your-api-audience>` | Auth0 API audience/identifier |

**Note**: Auth0 secrets are shared between environments if you use a single Auth0 tenant. You can create separate Auth0 applications for staging/production if needed.

### Production Environment Secrets

Navigate to **Settings** > **Environments** > **production** > **Environment secrets**

Add the following secrets:

| Secret Name | Value | Description |
|-------------|-------|-------------|
| `FLY_API_TOKEN` | `<your-fly-token>` | Same Fly.io token (has access to both apps) |
| `NETLIFY_AUTH_TOKEN` | `<your-netlify-token>` | Same Netlify token |
| `NETLIFY_SITE_ID` | `<prod-site-id>` | Netlify production site API ID |
| `VITE_API_URL` | `https://divergent-flow-api.fly.dev` | Backend API URL for production |
| `VITE_AUTH0_DOMAIN` | `<your-auth0-domain>` | Auth0 tenant domain (same or different) |
| `VITE_AUTH0_CLIENT_ID` | `<prod-client-id>` | Auth0 application client ID for production |
| `VITE_AUTH0_AUDIENCE` | `<your-api-audience>` | Auth0 API audience/identifier |

### Secret Precedence

Secrets are resolved in this order:
1. **Environment secrets** (highest priority)
2. Repository secrets
3. Default values in workflow file

For this setup, all deployment-specific secrets should be in **environment secrets** to keep configuration clean and environment-specific.

## 3. Workflow Behavior

### Staging Deployments

**Triggers**:
- PR opened, synchronized, or reopened against `main` branch
- Push to `main` branch

**Behavior**:
- ✅ Deploys automatically (no approval required)
- ✅ Comments on PR with deployment URL
- ✅ Updates deployment status in GitHub UI
- ✅ Uses `staging` environment secrets

**Purpose**: 
- Provides immediate feedback on changes
- Allows smoke testing before merging
- Validates build and deployment process

### Production Deployments

**Triggers**:
- Release PR merged (created by Release Please)
- Only when `releases_created == 'true'` output from Release Please

**Behavior**:
- ⏸️ **Pauses for manual approval** (required reviewer must approve)
- ✅ Sends notification to required reviewers
- ✅ Deploys after approval
- ✅ Uses `production` environment secrets
- ✅ Creates GitHub release with tags

**Purpose**:
- Prevents accidental production deployments
- Enforces review process for production changes
- Provides audit trail of production deployments
- Controls costs by gating production builds

## 4. Approval Workflow

### How to Approve a Production Deployment

When a release PR is merged, GitHub will:

1. **Queue the production deployment**
2. **Send notification** to required reviewers
3. **Wait for approval** before proceeding

To approve:

1. Navigate to **Actions** tab in GitHub
2. Click on the running **Deploy** workflow
3. Locate the **production** environment jobs (they will show "Waiting" status)
4. Click **Review deployments**
5. Select **production** environment
6. Click **Approve and deploy**

The deployment will then proceed automatically.

### Approval Best Practices

- **Review the changelog**: Check what changes are being deployed
- **Verify staging**: Ensure staging deployment succeeded and was tested
- **Check tests**: Confirm all CI tests passed
- **Smoke test staging**: Test critical features on staging before approving production
- **Consider timing**: Deploy during low-traffic periods if possible

## 5. Branch Protection Rules (Recommended)

To ensure code quality and require PR reviews before merging to `main`:

### Step 1: Enable Branch Protection

1. Go to **Settings** > **Branches**
2. Click **Add rule** under "Branch protection rules"
3. **Branch name pattern**: `main`

### Step 2: Configure Protection Rules

#### Require Pull Request Reviews
- ☑️ **Require a pull request before merging**
- ☑️ **Require approvals**: Set to `1` (or more for team review)
- ☑️ **Dismiss stale pull request approvals when new commits are pushed**
- ☑️ (Optional) **Require review from Code Owners**

#### Require Status Checks
- ☑️ **Require status checks to pass before merging**
- ☑️ **Require branches to be up to date before merging**
- Add required checks:
  - `test-frontend` (Frontend Tests)
  - `test-backend` (Backend Tests)
  - `validate-commit-messages` (Conventional Commits)

#### Additional Settings
- ☑️ **Require conversation resolution before merging**
- ☑️ **Require linear history** (prevents merge commits)
- ☑️ **Include administrators** (apply rules to admins too - recommended for solo dev)

#### Allow Force Pushes and Deletions
- ☐ Leave **unchecked** to protect main branch

### Step 3: Save Changes

Click **Create** or **Save changes**

## 6. Contributor Permissions

### Repository Roles

Configure team/contributor access under **Settings** > **Collaborators and teams**

| Role | Permissions | Use Case |
|------|-------------|----------|
| **Admin** | Full access, settings, deployments | Repository owner/maintainer |
| **Maintain** | Manage issues, PRs, some settings | Core contributors |
| **Write** | Push to branches, create PRs | Regular contributors |
| **Triage** | Manage issues and PRs | Community moderators |
| **Read** | View and clone repository | Public/external contributors |

### Deployment Permissions

**Who can trigger staging deployments?**
- Anyone who can create PRs against `main` (Write access or higher)

**Who can approve production deployments?**
- Only users listed as **required reviewers** in production environment settings
- Configure in **Settings** > **Environments** > **production** > **Required reviewers**

### Adding a Co-Maintainer

To add someone with full deployment approval permissions:

1. **Add as collaborator**: Settings > Collaborators > Add people
2. **Set role**: Choose **Admin** or **Maintain**
3. **Add as reviewer**: Settings > Environments > production > Required reviewers > Add
4. **Enable notifications**: They will receive email notifications for pending deployments

### Solo Development Best Practices

Even when developing solo, consider:

- ☑️ **Keep branch protection enabled** (prevents accidental direct pushes to main)
- ☑️ **Require PR reviews** (forces you to review your own changes before merging)
- ☑️ **Require status checks** (ensures tests pass before merge)
- ☑️ **Require deployment approval** (gives you time to review before production deploy)

You can disable "Prevent self-review" if you want to self-approve your own PRs while still maintaining the review process.

## 7. Cost Management

### Fly.io Considerations

Fly.io charges based on:
- Machine uptime
- Network egress
- Persistent volumes

**Cost-saving features in workflow**:
- Staging deploys always (provides continuous feedback)
- Production deploys only on releases (controlled gate)
- Manual approval prevents accidental production deploys
- Auto-stop machines when idle (configured in `fly.toml`)

### Netlify Considerations

Netlify free tier includes:
- 300 build minutes/month
- 100 GB bandwidth/month

**Cost-saving features in workflow**:
- Pre-built bundles from GitHub Actions (Netlify just serves static files)
- No Netlify auto-deploys (all deploys controlled by GitHub Actions)
- Efficient concurrency control (prevents duplicate builds)

## 8. Monitoring and Notifications

### GitHub Notifications

Configure in **Settings** > **Notifications** (user settings, not repo):

- ☑️ **Actions**: Workflow run notifications
- ☑️ **Deployments**: Deployment notifications

### Deployment Status

View deployment status:
- **Actions** tab: All workflow runs
- **Deployments**: Sidebar on repo main page (shows recent deployments)
- **Pull Requests**: Deployment status shown on each PR
- **Commits**: Deployment status shown on commit details

### Slack/Discord Integration (Optional)

To send deployment notifications to Slack/Discord:

1. Add webhook URL as repository secret: `SLACK_WEBHOOK_URL`
2. Add notification step to workflow (after deployments):

```yaml
- name: Notify Slack
  if: always()
  uses: slackapi/slack-github-action@v1
  with:
    payload: |
      {
        "text": "Deployment to ${{ github.event.inputs.environment }} ${{ job.status }}"
      }
  env:
    SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}
```

## 9. Testing the Setup

### Test Staging Deployment

1. **Create a feature branch**:
   ```bash
   git checkout -b feat/test-environments
   ```

2. **Make a small change** (e.g., update README):
   ```bash
   echo "Testing environments" >> README.md
   git add README.md
   git commit -m "feat(docs): test environment setup"
   git push origin feat/test-environments
   ```

3. **Create a PR** against `main`:
   - Go to GitHub > Pull Requests > New PR
   - Select your branch
   - Create PR

4. **Verify**:
   - ✅ Workflow runs automatically
   - ✅ Staging deployment succeeds
   - ✅ Deployment URL appears in PR comments
   - ✅ Deployment status shows in PR checks

### Test Production Deployment

1. **Merge the test PR** to `main`

2. **Wait for Release Please**:
   - Check **Pull Requests** tab
   - Look for PR titled "chore(main): release ..." created by Release Please

3. **Review and merge the Release PR**:
   - Review changelog
   - Merge the PR

4. **Approve production deployment**:
   - Go to **Actions** tab
   - Click on the running Deploy workflow
   - Click **Review deployments**
   - Approve production environment

5. **Verify**:
   - ✅ Production deployment succeeds after approval
   - ✅ GitHub release created
   - ✅ Version tags created
   - ✅ Deployment shows in Environments page

## 10. Troubleshooting

### "Required reviewers not met"

**Issue**: Production deployment blocked, no approval option visible

**Solutions**:
- Ensure you're listed as a required reviewer in production environment settings
- Check that the user attempting to approve has proper permissions
- Verify production environment protection rules are correctly configured

### "Environment not found"

**Issue**: Workflow fails with environment not found error

**Solutions**:
- Ensure environments are named exactly `staging` and `production` (lowercase)
- Verify environments exist in Settings > Environments
- Check workflow file references correct environment names

### Secrets not available in environment

**Issue**: Build fails with missing environment variables

**Solutions**:
- Verify secrets are added to the correct environment (not repository secrets)
- Check secret names match exactly (case-sensitive)
- Ensure the workflow job specifies the correct `environment:` key

### Deployment doesn't wait for approval

**Issue**: Production deployment proceeds without approval

**Solutions**:
- Verify production environment has "Required reviewers" enabled
- Check that the workflow job has `environment: production` configured
- Ensure deployment branches rule includes `main`

## 11. Migrating from Repository Secrets

If you're migrating from the old setup with `_STAGING`/`_PROD` suffixed secrets:

### Migration Checklist

- [ ] Create staging and production environments
- [ ] Add environment URLs to both environments
- [ ] Configure production environment protection rules
- [ ] Copy secrets from repository to staging environment (remove `_STAGING` suffix)
- [ ] Copy secrets from repository to production environment (remove `_PROD` suffix)
- [ ] Update workflow file to use environments (already done in this PR)
- [ ] Test staging deployment with a PR
- [ ] Test production deployment with a release
- [ ] Delete old repository secrets (after confirming environments work)

### Old vs New Secret Names

| Old Repository Secret | New Environment | New Secret Name |
|-----------------------|-----------------|-----------------|
| `VITE_API_URL_STAGING` | staging | `VITE_API_URL` |
| `VITE_API_URL_PROD` | production | `VITE_API_URL` |
| `NETLIFY_STAGING_SITE_ID` | staging | `NETLIFY_SITE_ID` |
| `NETLIFY_PROD_SITE_ID` | production | `NETLIFY_SITE_ID` |
| `NETLIFY_AUTH_TOKEN` | Both environments | `NETLIFY_AUTH_TOKEN` |
| `FLY_API_TOKEN` | Both environments | `FLY_API_TOKEN` |

Add new Auth0 secrets to both environments:
- `VITE_AUTH0_DOMAIN`
- `VITE_AUTH0_CLIENT_ID`
- `VITE_AUTH0_AUDIENCE`

## 12. Additional Resources

- [GitHub Environments Documentation](https://docs.github.com/en/actions/deployment/targeting-different-environments/using-environments-for-deployment)
- [GitHub Actions Security Best Practices](https://docs.github.com/en/actions/security-guides/security-hardening-for-github-actions)
- [Release Please Documentation](https://github.com/googleapis/release-please)
- [Fly.io Deployment Guide](https://fly.io/docs/)
- [Netlify Deploy with GitHub Actions](https://github.com/nwtgck/actions-netlify)

## Summary

With GitHub Environments configured, your deployment pipeline now provides:

- ✅ Automatic staging deployments on every PR
- ✅ Manual approval gates for production
- ✅ Simplified secrets management (no more `_STAGING`/`_PROD` suffixes)
- ✅ Deployment URLs visible in GitHub UI
- ✅ Audit trail of all deployments
- ✅ Cost control through manual production approvals
- ✅ Support for multiple reviewers/maintainers

This setup is production-ready and scales from solo development to team collaboration.
