# GitHub Environments Quick Start

> 📘 **Full Guide**: See [ENVIRONMENTS-SETUP.md](./ENVIRONMENTS-SETUP.md) for complete instructions

## 5-Minute Setup

### 1. Create Environments

**Settings** > **Environments** > **New environment**

#### Staging Environment
- Name: `staging`
- Protection: None
- URL: `https://divergent-flow-staging.netlify.app`

#### Production Environment
- Name: `production`
- Protection: ✅ **Required reviewers** (add yourself)
- URL: `https://divergent-flow-prod.netlify.app`
- Deployment branches: `main` only

### 2. Add Secrets to Each Environment

**Settings** > **Environments** > [environment] > **Environment secrets**

Add these to **both** staging and production (with appropriate values):

```
✓ FLY_API_TOKEN
✓ NETLIFY_AUTH_TOKEN
✓ NETLIFY_SITE_ID
✓ VITE_API_URL
✓ VITE_AUTH0_DOMAIN
✓ VITE_AUTH0_CLIENT_ID
✓ VITE_AUTH0_AUDIENCE
```

**Staging values:**
- `NETLIFY_SITE_ID`: Staging site API ID
- `VITE_API_URL`: `https://divergent-flow-api-staging.fly.dev`
- `VITE_AUTH0_CLIENT_ID`: Auth0 staging app client ID

**Production values:**
- `NETLIFY_SITE_ID`: Production site API ID
- `VITE_API_URL`: `https://divergent-flow-api.fly.dev`
- `VITE_AUTH0_CLIENT_ID`: Auth0 production app client ID

### 3. Test It

**Create a test PR:**
```bash
git checkout -b test/environments
echo "test" >> README.md
git commit -m "chore: test environments"
git push origin test/environments
```

Then create PR on GitHub → Watch staging deploy automatically

**Test production approval:**
- Merge a feature PR to trigger Release Please
- Merge the Release PR
- Go to Actions tab → Review deployments → Approve production

## How It Works

### PR Created/Updated → Staging Deploy
- ✅ Automatic (no approval)
- 💬 PR comment with deployment URL
- 🔗 Environment visible in PR

### Release PR Merged → Production Deploy
- ⏸️ Waits for approval
- 📧 Notifies required reviewers
- ✅ Deploys after approval

## Common Tasks

### Add a Co-Maintainer
1. **Settings** > **Collaborators** > Add person
2. **Settings** > **Environments** > **production** > Add to required reviewers

### Add New Environment Variable
1. **Settings** > **Environments** > [staging/production]
2. **Environment secrets** > **Add secret**
3. Add to both staging and production

### Change Who Can Approve Production
1. **Settings** > **Environments** > **production**
2. **Required reviewers** > Edit list

## Troubleshooting

**"Required reviewers not met"**
→ Add yourself to production environment required reviewers

**"Environment not found"**
→ Ensure environments named exactly `staging` and `production` (lowercase)

**Secrets not working**
→ Check secrets are in environment secrets, not repository secrets

**Production deployed without approval**
→ Verify production environment has required reviewers enabled

## Migration from Old Setup

Already have `VITE_API_URL_STAGING` and `VITE_API_URL_PROD` repository secrets?

**Old Setup (Repository Secrets):**
```
VITE_API_URL_STAGING
VITE_API_URL_PROD
NETLIFY_STAGING_SITE_ID
NETLIFY_PROD_SITE_ID
```

**New Setup (Environment Secrets):**
```
staging environment:
  VITE_API_URL (no suffix!)
  NETLIFY_SITE_ID (no suffix!)

production environment:
  VITE_API_URL (no suffix!)
  NETLIFY_SITE_ID (no suffix!)
```

After migrating to environments, delete old repository secrets.

## Resources

- [Full Setup Guide](./ENVIRONMENTS-SETUP.md) - Complete instructions
- [Deployment Setup](./DEPLOYMENT-SETUP.md) - Fly.io and Netlify setup
- [GitHub Docs](https://docs.github.com/en/actions/deployment/targeting-different-environments/using-environments-for-deployment)
