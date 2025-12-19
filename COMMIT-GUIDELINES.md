# Conventional Commits Quick Reference

## Format
```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

## Common Types

| Type | Description | Example | Version Bump |
|------|-------------|---------|--------------|
| `feat` | New feature | `feat(capture): add keyboard shortcut` | MINOR (0.x.0) |
| `fix` | Bug fix | `fix(review): correct sorting logic` | PATCH (0.0.x) |
| `docs` | Documentation | `docs(readme): update installation` | None |
| `style` | Code style | `style(ui): fix formatting` | None |
| `refactor` | Refactoring | `refactor(inference): simplify logic` | None |
| `test` | Tests | `test(inference): add edge cases` | None |
| `chore` | Maintenance | `chore(deps): update dependencies` | None |
| `perf` | Performance | `perf(storage): optimize queries` | PATCH (0.0.x) |
| `ci` | CI/CD | `ci(github): add workflow` | None |
| `build` | Build system | `build(vite): update config` | None |

## Common Scopes

- `capture` - Capture input functionality
- `review` - Review queue feature
- `inference` - Type inference engine
- `ui` - UI components
- `storage` - Data persistence layer
- `types` - TypeScript definitions
- `tests` - Test files
- `agent` - AI agent configuration
- `deps` - Dependencies

## Examples

### Good ✅
```bash
feat(capture): add keyboard shortcut Cmd+Enter to submit
fix(review): correct priority sorting in review queue
docs(readme): update installation instructions
refactor(inference): extract pattern matching to separate function
test(inference): add tests for edge cases
chore(deps): update React to v19.2.3
style(ui): fix button padding
perf(storage): optimize query performance
ci(github): add commit message validation
```

### Bad ❌
```bash
Added feature                    # Missing type and scope
Fix bug                          # Missing scope and description
feat: Add Feature                # Description starts with capital letter
fix(review): fix the bug.        # Description ends with period
feat(review) add feature         # Missing colon after scope
updated readme                   # Missing type and scope
WIP                              # Not descriptive
```

## Breaking Changes

Add `!` after type/scope OR include `BREAKING CHANGE:` in footer:

```bash
feat(api)!: redesign inference API

BREAKING CHANGE: The inference API has been completely redesigned.
All existing integrations need to be updated.
```

## Body and Footer

```bash
feat(capture): add autosave functionality

Implements automatic saving every 5 seconds when user is typing.
This reduces the risk of losing data if the browser crashes.

Closes #123
Refs #456
```

## Tips

1. **Use imperative mood**: "add feature" not "added feature"
2. **Keep subject under 72 chars**: Short and concise
3. **No period at end**: `fix: bug` not `fix: bug.`
4. **Lowercase subject**: `add feature` not `Add feature`
5. **Reference issues**: Use `Closes #123` or `Fixes #123`

## Validation

Your commits are validated by:
- **Local hook** (if installed via `scripts/setup-dev.sh`)
- **GitHub Actions** (on PR creation/update)
- **commitlint** (uses `commitlint.config.js`)

## Setup

Run the setup script to configure your local environment:
```bash
chmod +x scripts/setup-dev.sh
./scripts/setup-dev.sh
```

This will:
- Configure Git commit template
- Install commitlint dependencies
- Optionally install commit-msg validation hook

## Resources

- [Conventional Commits](https://www.conventionalcommits.org/)
- [Commitlint](https://commitlint.js.org/)
- [CONTRIBUTING.md](./CONTRIBUTING.md) - Full contribution guide
- [.github/copilot-instructions.md](./.github/copilot-instructions.md) - AI agent guide
