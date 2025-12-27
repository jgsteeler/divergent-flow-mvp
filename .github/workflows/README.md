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

## Release Please

The `release-please.yml` workflow automates version management and changelog generation using [Release Please](https://github.com/googleapis/release-please).

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
