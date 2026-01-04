# GitHub Workflows

This directory contains GitHub Actions workflows for the Divergent Flow MVP project.

## Conventional Commits Validation

The `conventional-commits.yml` workflow enforces Conventional Commits format on all commits and PR titles.

### What It Does

1. **PR Title Validation**: Checks that PR titles follow the format `<type>(<scope>): <description>`
2. **Commit Message Validation**: Validates all commits in a PR against Conventional Commits specification
3. **Automatic Feedback**: Provides clear error messages when validation fails

### Validation Rules

- **Required format**: `<type>(<scope>): <description>`
- **Valid types**: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `chore`, `ci`, `build`
- **Valid scopes**: `capture`, `review`, `inference`, `ui`, `storage`, `types`, `tests`, `agent`, `deps` (optional)
- **Description**: Must start with lowercase, no period at end, max 72 characters

### When It Runs

- On pull request creation
- On pull request title edit
- On new commits pushed to PR
- On PR synchronization

### Configuration

- Workflow: `.github/workflows/conventional-commits.yml`
- Commitlint config: `commitlint.config.cjs`
- Commit template: `.github/commit_template.txt`

### Local Setup

Run the setup script to configure local validation:

```bash
./scripts/setup-dev.sh
```

See [COMMIT-GUIDELINES.md](../../COMMIT-GUIDELINES.md) for quick reference.

---

## Unified Deployment Workflow

The `deploy.yml` workflow provides a complete CI/CD pipeline with version management, staging deployments, and production promotions.

### Architecture Overview

This workflow implements a **two-environment strategy** with static URLs:

- **Staging Environment**: Always deployed from `main` branch
  - Backend: `divergent-flow-api-staging` (Fly.io)
  - Frontend: Static staging URL via dedicated Netlify site
- **Production Environment**: Promoted only on releases
  - Backend: `divergent-flow-api` (Fly.io) - Binary image promotion
  - Frontend: Dedicated Netlify production site

### Workflow Steps

1. **Version Management** (Release Please)
   - Analyzes conventional commits on `main` branch
   - Creates/updates release PR with version bumps and changelog
   - Sets `releases_created` flag when release PR is merged

2. **Deploy to Staging** (Always runs)
   - **Backend**: Builds and deploys to Fly.io staging app
   - **Frontend**: Builds React app and deploys to Netlify staging site

3. **Promote to Production** (Only on release)
   - **Backend**: Promotes exact staging container image (binary promotion, no rebuild)
   - **Frontend**: Rebuilds and deploys to Netlify production site
   - Triggered when `releases_created == true`

### Key Features

- ✅ **Binary Promotion**: Backend production uses the exact container that passed staging health checks
- ✅ **Static URLs**: Both staging and production have unchanging URLs (no dynamic preview URLs)
- ✅ **Consistent Deploys**: Staging and production always deploy from the same code state
- ✅ **Concurrency Control**: Prevents overlapping deploys with `concurrency` group
- ✅ **Image Validation**: Uses `jq` to extract and validate Fly.io image references

### GitHub Secrets Required

Add these secrets in **Settings > Secrets and variables > Actions**:

- `FLY_API_TOKEN`: Fly.io deploy token
- `NETLIFY_AUTH_TOKEN`: Netlify personal access token
- `NETLIFY_STAGING_SITE_ID`: API ID for staging Netlify site
- `NETLIFY_PROD_SITE_ID`: API ID for production Netlify site

### Netlify Setup

1. **Create Two Sites** in Netlify:
   - Site A (Production): e.g., `my-app-prod`
   - Site B (Staging): e.g., `my-app-staging`
   
2. **Disable Auto-Deploys** on both sites:
   - Go to **Site Settings > Build & deploy > Continuous Deployment**
   - Set **Build settings** to "Stop builds"
   - This prevents Netlify from building automatically; GitHub Actions controls all deploys

3. **Get Site IDs**:
   - Go to **Site Settings > General > Site details**
   - Copy the **API ID** for each site
   - Add as GitHub secrets

### When It Runs

- **Trigger**: Every push to `main` branch
- **Staging**: Always deploys (provides continuous feedback)
- **Production**: Only deploys when release PR is merged

### Technical Details

**Image Promotion Command:**
```bash
# Get staging image reference
IMAGE_REF=$(flyctl status --app divergent-flow-api-staging --json | jq -r '.ImageRef')

# Deploy to production using staging image
flyctl deploy --app divergent-flow-api --image "$IMAGE_REF"
```

This ensures the exact Docker image that passed staging health checks is deployed to production.

### Migration Notes

The following workflows have been archived (renamed to `.disabled`):
- `release-please.yml.disabled` - Now integrated into `deploy.yml`
- `deploy-backend-fly.yml.disabled` - Replaced by unified deployment
- `deploy-backend-fly-staging.yml.disabled` - Staging now part of unified flow

---

## Release Please (Integrated)

Version management is now integrated into the `deploy.yml` workflow using [Release Please](https://github.com/googleapis/release-please).

### How It Works

1. **Automatic Version Bumps**: When you push commits to the `main` branch using [Conventional Commits](https://www.conventionalcommits.org/), Release Please analyzes the commit messages and determines the appropriate version bump:
   - `feat:` → Minor version bump (0.1.0 → 0.2.0)
   - `fix:` → Patch version bump (0.1.0 → 0.1.1)
   - `feat!:` or `BREAKING CHANGE:` → Major version bump (0.1.0 → 1.0.0)

2. **Pull Request Creation**: Release Please creates or updates a release PR with:
   - Updated version numbers in `package.json` and other tracked files
   - Auto-generated `CHANGELOG.md` based on commit messages
   - Release notes summarizing changes

3. **Release Creation**: When you merge the release PR, Release Please automatically:
   - Creates a GitHub release
   - Tags the commit with the new version
   - Publishes the updated changelog

### Multi-Package Support

This repository is configured to support both:

- **Frontend (root)**: TypeScript/React application (node release-type)
  - Changelog: `CHANGELOG.md`
- **Backend**: Future .NET Web API project (simple release-type)
  - Changelog: `backend/CHANGELOG.md`
  - **Note**: When creating the backend project, update `.github/release-please-config.json` to add an `extra-files` array with the .csproj file path(s) for automatic version updates. For example:

    ```json
    "extra-files": [
      "backend/DivergentFlow.Api/DivergentFlow.Api.csproj"
    ]
    ```

Each package has its own version tracking and changelog to avoid conflicts.

### Conventional Commit Format

Use these commit message formats:

```
feat: add new capture feature
fix: resolve storage issue
docs: update README
chore: update dependencies
feat!: breaking API change
```

### Configuration Files

- `.github/workflows/release-please.yml` - Workflow definition
- `.github/release-please-config.json` - Package configuration
- `.github/.release-please-manifest.json` - Version tracking

### References

- [Release Please Documentation](https://github.com/googleapis/release-please)
- [Conventional Commits Specification](https://www.conventionalcommits.org/)
- Reference Implementation: [jgsteeler/gsc-tracking](https://github.com/jgsteeler/gsc-tracking)
