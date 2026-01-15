# Deployment Setup Guide

This guide walks you through setting up the unified CI/CD deployment pipeline for Divergent Flow with GitHub Environments.

> 📘 **New to GitHub Environments?** See [ENVIRONMENTS-SETUP.md](./ENVIRONMENTS-SETUP.md) for a complete guide to configuring environments, protection rules, and approval workflows.

## Overview

The deployment architecture uses:
- **Backend**: .NET API on Fly.io (staging + production apps)
- **Frontend**: React app on Netlify (staging + production sites)
- **CI/CD**: GitHub Actions with automated version management via Release Please
- **Environments**: GitHub Environments for secrets management and deployment approvals

## Prerequisites

- Fly.io account with CLI installed
- Netlify account
- GitHub repository admin access

## 1. Fly.io Setup

### Create Staging App

```bash
cd backend
fly apps create divergent-flow-api-staging
fly deploy --config fly.staging.toml --remote-only
```

### Create Production App

```bash
fly apps create divergent-flow-api
fly deploy --config fly.toml --remote-only
```

### Get Fly.io API Token

```bash
fly auth token
```

Save this token - you'll add it as a GitHub secret.

## 2. Netlify Setup

### Step 1: Create Two Sites

1. Log in to [Netlify](https://app.netlify.com/)
2. Click **"Add new site"** > **"Import an existing project"**
3. Select your GitHub repository
4. Name it clearly: `divergent-flow-staging`
5. Repeat for production: `divergent-flow-prod`

### Step 2: Disable Auto-Deploys

**Critical:** This prevents Netlify from building on every commit. GitHub Actions will control all deploys.

For **both** sites:
1. Go to **Site Settings**
2. Navigate to **Build & deploy** > **Continuous Deployment**
3. Under **Build settings**, click **"Edit settings"**
4. Set **Build command** to empty
5. Click **"Stop builds"** or set to "None"

### Step 3: Get Site IDs

For **both** sites:
1. Go to **Site Settings** > **General** > **Site details**
2. Find **Site information** section
3. Copy the **API ID** (format: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`)

### Step 4: Generate Personal Access Token

1. Go to **User settings** (click your avatar)
2. Navigate to **Applications** > **Personal access tokens**
3. Click **"New access token"**
4. Name it: `GitHub Actions Deploy`
5. Copy the token (you won't see it again!)

## 3. GitHub Environments and Secrets Setup

> 📘 **Important**: This project uses GitHub Environments for secrets management and deployment approvals.
> 
> See [ENVIRONMENTS-SETUP.md](./ENVIRONMENTS-SETUP.md) for complete setup instructions including:
> - Creating staging and production environments
> - Configuring protection rules and required reviewers
> - Setting up environment-specific secrets
> - Configuring approval workflows
> - Branch protection recommendations

### Quick Setup Summary

1. **Create Environments**:
   - Go to **Settings** > **Environments**
   - Create `staging` environment (no protection rules)
   - Create `production` environment (with required reviewers)

2. **Add Environment Secrets**:
   
   **Staging environment** secrets:
   - `FLY_API_TOKEN` - Fly.io API token
   - `NETLIFY_AUTH_TOKEN` - Netlify personal access token
   - `NETLIFY_SITE_ID` - Staging site API ID
   - `VITE_API_URL` - Backend API URL (staging)
   - `VITE_AUTH0_DOMAIN` - Auth0 domain
   - `VITE_AUTH0_CLIENT_ID` - Auth0 client ID (staging app)
   - `VITE_AUTH0_AUDIENCE` - Auth0 API audience

   **Production environment** secrets:
   - Same secret names as staging
   - Different values for `NETLIFY_SITE_ID`, `VITE_API_URL`, and `VITE_AUTH0_CLIENT_ID`

3. **Configure Protection Rules**:
   - Production environment: Add required reviewers (yourself and/or co-maintainers)
   - Set environment URLs for both staging and production
   - Optionally restrict deployment branches to `main`

For detailed instructions, see [ENVIRONMENTS-SETUP.md](./ENVIRONMENTS-SETUP.md).

## 4. Verify Setup

### Test Staging Deploy

Push a commit to `main` branch:

```bash
git checkout main
git commit --allow-empty -m "chore: test staging deploy"
git push
```

Check GitHub Actions:
1. Go to **Actions** tab
2. Click on the **Deploy** workflow
3. Verify all staging jobs succeed

### Test Production Deploy

Merge a release PR created by Release Please:

1. Wait for Release Please to create a PR (after commits with `feat:` or `fix:`)
2. Review the PR (check version bump and changelog)
3. Merge the PR
4. Watch GitHub Actions deploy to production

## 5. Workflow Behavior

### On Pull Request to `main`

1. ✅ Automatic staging deployment (backend + frontend)
2. ✅ Deployment URL posted in PR comments
3. ✅ Allows smoke testing before merge
4. ❌ Production deploy **does not run** on PRs

### On Push to `main` Branch

1. ✅ Release Please analyzes commits
2. ✅ Backend deploys to staging (`divergent-flow-api-staging`)
3. ✅ Frontend deploys to Netlify staging site
4. ❌ Production deploy **only** happens on release

### On Release PR Merge

1. ✅ Release Please creates release and tags
2. ✅ Staging deploys run first (already deployed from main push)
3. ⏸️ **Production deploy waits for manual approval**
4. ✅ After approval: Backend and frontend deploy to production
5. ✅ GitHub release created with version tags

## 6. URLs

After setup, your URLs will be:

- **Backend Staging**: `https://divergent-flow-api-staging.fly.dev`
- **Backend Production**: `https://divergent-flow-api.fly.dev`
- **Frontend Staging**: `https://divergent-flow-staging.netlify.app`
- **Frontend Production**: `https://divergent-flow-prod.netlify.app`

Update your CORS settings and authentication providers with these static URLs.

## 7. Troubleshooting

### Deployment Fails with "Site not found"

- Verify `NETLIFY_STAGING_SITE_ID` and `NETLIFY_PROD_SITE_ID` are correct
- Check that sites exist in Netlify dashboard

### Backend promotion fails

- Ensure staging app has completed at least one successful deploy
- Check that staging app name matches: `divergent-flow-api-staging`
- Verify `FLY_API_TOKEN` has permissions for both apps

### Release Please not creating PRs

- Ensure commits follow [Conventional Commits](https://www.conventionalcommits.org/) format
- Only `feat:` and `fix:` commits trigger version bumps
- Check `.github/release-please-config.json` configuration

### Frontend build fails

- Verify Node.js version matches (`20` in workflow)
- Check that `frontend/package-lock.json` exists
- Try building locally: `cd frontend && npm ci && npm run build`

## 8. Custom Domains

### Netlify Custom Domains

1. Go to each Netlify site settings
2. Navigate to **Domain management** > **Custom domains**
3. Add your domain (e.g., `staging.myapp.com`, `myapp.com`)
4. Update DNS records as instructed by Netlify
5. Enable HTTPS (automatic with Netlify)

### Fly.io Custom Domains

```bash
# Production
fly certs add myapi.com -a divergent-flow-api

# Staging
fly certs add staging-api.myapi.com -a divergent-flow-api-staging
```

Update your DNS with the values provided by Fly.io.

## 9. Environment Variables

### Backend Environment Variables

Set secrets in Fly.io for each app:

```bash
# Staging
fly secrets set CORS_ALLOWED_ORIGINS="https://divergent-flow-staging.netlify.app" \
  -a divergent-flow-api-staging

# Production
fly secrets set CORS_ALLOWED_ORIGINS="https://divergent-flow-prod.netlify.app" \
  -a divergent-flow-api
```

### Frontend Environment Variables

Frontend environment variables are configured in **GitHub Environments** (not Netlify).

The workflow builds the frontend with environment variables from GitHub Environment secrets:
- `VITE_API_URL` - Backend API URL
- `VITE_AUTH0_DOMAIN` - Auth0 tenant domain
- `VITE_AUTH0_CLIENT_ID` - Auth0 application client ID
- `VITE_AUTH0_AUDIENCE` - Auth0 API audience/identifier

These are set per environment (staging/production) in GitHub. See [ENVIRONMENTS-SETUP.md](./ENVIRONMENTS-SETUP.md) for details.

**Note**: Netlify auto-builds are disabled. GitHub Actions builds the frontend and deploys the static bundle to Netlify.

## 10. Monitoring

### GitHub Actions

- View all workflow runs: **Actions** tab
- Enable notifications: **Settings** > **Notifications** > **Actions**

### Fly.io

```bash
# Check app status
fly status -a divergent-flow-api

# View logs
fly logs -a divergent-flow-api

# Monitor health checks
fly checks list -a divergent-flow-api
```

### Netlify

- View deploy logs in Netlify dashboard
- Set up deploy notifications: **Site Settings** > **Build & deploy** > **Deploy notifications**

## Support

- **GitHub Actions**: [docs.github.com/actions](https://docs.github.com/actions)
- **Fly.io**: [fly.io/docs](https://fly.io/docs)
- **Netlify**: [docs.netlify.com](https://docs.netlify.com)
- **Release Please**: [github.com/googleapis/release-please](https://github.com/googleapis/release-please)
