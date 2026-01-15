# Setup Checklist for GitHub Environments

Use this checklist to set up GitHub Environments for the deployment pipeline.

## Pre-Merge Checklist

- [ ] Review the changes in this PR
- [ ] Read [IMPLEMENTATION-SUMMARY.md](./IMPLEMENTATION-SUMMARY.md) for overview
- [ ] Understand the new deployment flow
- [ ] Have Auth0 credentials ready (or know where to get them)

## Setup After Merging (30 minutes total)

### Part 1: Create Environments (5 min)

- [ ] Navigate to **Settings** > **Environments**
- [ ] Click **New environment**
  - [ ] Create `staging` environment
    - [ ] Name: exactly `staging` (lowercase)
    - [ ] Protection rules: None
    - [ ] Environment URL: `https://divergent-flow-staging.netlify.app`
  - [ ] Create `production` environment
    - [ ] Name: exactly `production` (lowercase)
    - [ ] ✅ Enable **Required reviewers**
    - [ ] Add yourself as required reviewer
    - [ ] Environment URL: `https://divergent-flow-prod.netlify.app`
    - [ ] (Optional) Deployment branches: `main` only

### Part 2: Get Required Values (10 min)

- [ ] **Fly.io API Token**
  - [ ] Run: `fly auth token`
  - [ ] Save token for next step

- [ ] **Netlify Auth Token**
  - [ ] Go to: Netlify User Settings > Applications > Personal access tokens
  - [ ] Create new token: "GitHub Actions Deploy"
  - [ ] Save token for next step

- [ ] **Netlify Site IDs**
  - [ ] Staging: Netlify > Staging Site > Settings > General > Site details > API ID
  - [ ] Production: Netlify > Production Site > Settings > General > Site details > API ID
  - [ ] Save both IDs for next step

- [ ] **Auth0 Credentials** (if using Auth0)
  - [ ] Auth0 Dashboard > Applications > Your Application
  - [ ] Note: Domain, Client ID, Audience
  - [ ] If you have separate staging/production apps, note both Client IDs

### Part 3: Add Staging Environment Secrets (7 min)

- [ ] Navigate to **Settings** > **Environments** > **staging**
- [ ] Click **Add secret** for each:
  - [ ] `FLY_API_TOKEN` = _(Fly.io token from above)_
  - [ ] `NETLIFY_AUTH_TOKEN` = _(Netlify token from above)_
  - [ ] `NETLIFY_SITE_ID` = _(Staging site ID from above)_
  - [ ] `VITE_API_URL` = `https://divergent-flow-api-staging.fly.dev`
  - [ ] `VITE_AUTH0_DOMAIN` = _(Your Auth0 domain, e.g., `dev-xyz.us.auth0.com`)_
  - [ ] `VITE_AUTH0_CLIENT_ID` = _(Staging app client ID)_
  - [ ] `VITE_AUTH0_AUDIENCE` = _(Your API audience/identifier)_

### Part 4: Add Production Environment Secrets (7 min)

- [ ] Navigate to **Settings** > **Environments** > **production**
- [ ] Click **Add secret** for each:
  - [ ] `FLY_API_TOKEN` = _(Same Fly.io token)_
  - [ ] `NETLIFY_AUTH_TOKEN` = _(Same Netlify token)_
  - [ ] `NETLIFY_SITE_ID` = _(Production site ID from above)_
  - [ ] `VITE_API_URL` = `https://divergent-flow-api.fly.dev`
  - [ ] `VITE_AUTH0_DOMAIN` = _(Your Auth0 domain)_
  - [ ] `VITE_AUTH0_CLIENT_ID` = _(Production app client ID)_
  - [ ] `VITE_AUTH0_AUDIENCE` = _(Your API audience/identifier)_

### Part 5: Test Staging Deployment (10 min)

- [ ] Create test branch: `git checkout -b test/github-environments`
- [ ] Make small change: `echo "Test environments" >> README.md`
- [ ] Commit: `git commit -am "chore: test GitHub environments"`
- [ ] Push: `git push origin test/github-environments`
- [ ] Create PR to `main` on GitHub
- [ ] **Verify**:
  - [ ] GitHub Actions workflow runs
  - [ ] Staging deployment starts
  - [ ] Deployment comment appears in PR
  - [ ] Staging site URL works
  - [ ] No production deployment triggered

### Part 6: Test Production Approval (15 min)

- [ ] Merge test PR to main (if staging looks good)
- [ ] Wait for Release Please to create release PR
- [ ] Merge release PR
- [ ] Go to **Actions** tab
- [ ] Click on running **Deploy** workflow
- [ ] **Verify**:
  - [ ] Staging deploys complete
  - [ ] Production jobs show "Waiting" status
  - [ ] "Review deployments" button appears
- [ ] Click **Review deployments**
- [ ] Select **production** environment
- [ ] Click **Approve and deploy**
- [ ] **Verify**:
  - [ ] Production deployment proceeds
  - [ ] Production sites updated
  - [ ] GitHub release created

### Part 7: Cleanup Old Secrets (Optional, 5 min)

After confirming environments work:

- [ ] Go to **Settings** > **Secrets and variables** > **Actions**
- [ ] Delete old repository secrets (if they exist):
  - [ ] `VITE_API_URL_STAGING`
  - [ ] `VITE_API_URL_PROD`
  - [ ] `NETLIFY_STAGING_SITE_ID`
  - [ ] `NETLIFY_PROD_SITE_ID`
- [ ] Keep or delete these (your choice):
  - [ ] `FLY_API_TOKEN` (already in environments)
  - [ ] `NETLIFY_AUTH_TOKEN` (already in environments)

## Optional: Branch Protection (5 min)

Recommended for code quality and deployment safety:

- [ ] Go to **Settings** > **Branches**
- [ ] Click **Add rule**
- [ ] Branch name pattern: `main`
- [ ] Enable:
  - [ ] ✅ Require a pull request before merging
  - [ ] ✅ Require approvals: 1
  - [ ] ✅ Dismiss stale pull request approvals when new commits are pushed
  - [ ] ✅ Require status checks to pass before merging
    - [ ] Add: `test-frontend`
    - [ ] Add: `test-backend`
    - [ ] Add: `validate-commit-messages`
  - [ ] ✅ Require conversation resolution before merging
  - [ ] ✅ Require linear history
  - [ ] ✅ Include administrators (applies to you too)
- [ ] Click **Create** or **Save changes**

## Optional: Add Co-Maintainer (5 min)

If you have a collaborator who should approve production deployments:

- [ ] Go to **Settings** > **Collaborators and teams**
- [ ] Click **Add people**
- [ ] Enter their GitHub username
- [ ] Select role: **Maintain** or **Admin**
- [ ] Click **Add**
- [ ] Go to **Settings** > **Environments** > **production**
- [ ] Under **Required reviewers**, add their username
- [ ] Save changes

## Troubleshooting

### ❌ "Required reviewers not met"
**Fix**: Add yourself to production environment required reviewers

### ❌ "Environment not found"
**Fix**: Check environment names are exactly `staging` and `production` (lowercase)

### ❌ Secrets not working
**Fix**: Verify secrets are in Environment secrets, not Repository secrets

### ❌ Workflow syntax error
**Fix**: Workflow YAML was validated. If errors persist, check GitHub Actions logs

### ❌ Staging doesn't deploy on PR
**Fix**: Ensure PR is targeting `main` branch

### ❌ Production deploys without approval
**Fix**: Verify production environment has Required reviewers enabled

## Success Criteria

You've successfully completed the setup when:

- ✅ Creating a PR triggers automatic staging deployment
- ✅ PR shows deployment comment with staging URL
- ✅ Merging release PR queues production deployment
- ✅ Production deployment requires your approval
- ✅ After approval, production deploys successfully
- ✅ Environment URLs work in GitHub UI

## Need Help?

See these guides:
- **ENVIRONMENTS-QUICKSTART.md** - Quick reference
- **ENVIRONMENTS-SETUP.md** - Complete documentation
- **IMPLEMENTATION-SUMMARY.md** - Overview of changes

## Next Steps After Setup

Once everything is working:

1. ✅ Update your Auth0 applications with new callback URLs (if needed)
2. ✅ Update Fly.io CORS settings with correct origins
3. ✅ Test full authentication flow on staging
4. ✅ Continue normal development workflow
5. ✅ PRs will automatically deploy to staging for testing

---

**Estimated Total Time**: 30-60 minutes depending on experience level

**Difficulty**: Beginner-friendly with step-by-step instructions

**Questions?** See the detailed guides or GitHub's official documentation on Environments.
