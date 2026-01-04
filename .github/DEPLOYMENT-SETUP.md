# Deployment Setup Guide

This guide walks you through setting up the unified CI/CD deployment pipeline for Divergent Flow.

## Overview

The deployment architecture uses:
- **Backend**: .NET API on Fly.io (staging + production apps)
- **Frontend**: React app on Netlify (staging + production sites)
- **CI/CD**: GitHub Actions with automated version management via Release Please

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

## 3. GitHub Secrets Setup

Add these secrets to your repository:

1. Go to your GitHub repository
2. Click **Settings** > **Secrets and variables** > **Actions**
3. Click **"New repository secret"** for each:

| Secret Name | Value | Description |
|-------------|-------|-------------|
| `FLY_API_TOKEN` | `<your-fly-token>` | From step 1 |
| `NETLIFY_AUTH_TOKEN` | `<your-netlify-token>` | From step 2.4 |
| `NETLIFY_STAGING_SITE_ID` | `<staging-site-id>` | From step 2.3 |
| `NETLIFY_PROD_SITE_ID` | `<prod-site-id>` | From step 2.3 |

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

### On Every Push to `main`

1. ✅ Release Please analyzes commits
2. ✅ Backend deploys to staging (`divergent-flow-api-staging`)
3. ✅ Frontend deploys to Netlify staging site
4. ❌ Production deploy **only** happens on release

### On Release PR Merge

1. ✅ Release Please creates release and tags
2. ✅ Staging deploys run first
3. ✅ Backend: Staging image promoted to production (binary promotion)
4. ✅ Frontend: Rebuilt and deployed to production Netlify site

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

Set secrets in Fly.io:

```bash
# Staging
fly secrets set CORS_ALLOWED_ORIGINS="https://staging.myapp.com" \
  -a divergent-flow-api-staging

# Production
fly secrets set CORS_ALLOWED_ORIGINS="https://myapp.com" \
  -a divergent-flow-api
```

### Frontend Environment Variables

Add to Netlify site settings:

1. **Site Settings** > **Environment variables**
2. Add variables for each environment
3. Prefix with `VITE_` for Vite to expose them:
   - `VITE_API_URL`
   - `VITE_APP_NAME`

Note: Environment variables in Netlify are set per site, so staging and production can have different values.

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
