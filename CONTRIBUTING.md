# Contributing to Divergent Flow

Thank you for your interest in contributing to Divergent Flow! This document provides guidelines for contributing to the project.

## 🚨 Critical Requirements

### Conventional Commits (Required)

**ALL commits and PR titles MUST follow the [Conventional Commits](https://www.conventionalcommits.org/) specification.**

This is not optional. Our CI/CD pipeline will automatically validate commit messages and PR titles. Non-compliant commits will cause PR checks to fail.

#### Format
```
<type>(<scope>): <description>

[optional body]

[optional footer(s)]
```

#### Examples
```bash
feat(capture): add keyboard shortcut Cmd+Enter to submit
fix(review): correct priority sorting in review queue
docs(readme): update installation instructions
refactor(inference): extract pattern matching to separate function
test(inference): add edge case tests for type detection
chore(deps): update dependencies to latest versions
```

#### Commit Types
- **feat**: New feature (triggers MINOR version bump)
- **fix**: Bug fix (triggers PATCH version bump)
- **docs**: Documentation only changes
- **style**: Code style changes (formatting, etc.)
- **refactor**: Code refactoring
- **perf**: Performance improvement
- **test**: Adding or updating tests
- **chore**: Build process, dependencies, or tooling
- **ci**: CI configuration changes
- **build**: Build system or external dependencies

#### Scopes (Optional)
- `capture`: Capture input functionality
- `review`: Review queue feature
- `inference`: Type inference engine
- `ui`: UI components
- `storage`: Data persistence layer
- `types`: TypeScript type definitions
- `tests`: Test files
- `agent`: AI agent configuration
- `deps`: Dependencies

#### Breaking Changes
If your change introduces breaking changes:
```bash
feat(api)!: redesign inference API

BREAKING CHANGE: The inference API has been completely redesigned.
All existing integrations will need to be updated.
```

### Setup Git Commit Template (Recommended)

Configure Git to use our commit template:
```bash
git config commit.template .github/commit_template.txt
```

This will pre-populate your commit messages with the correct format.

## Development Workflow

### 1. Fork and Clone
```bash
git clone https://github.com/YOUR_USERNAME/divergent-flow-mvp.git
cd divergent-flow-mvp
```

### 2. Install Dependencies
```bash
npm install
```

### 3. Create a Feature Branch
```bash
git checkout -b feat/your-feature-name
# or
git checkout -b fix/your-bug-fix
```

### 4. Make Your Changes
- Follow the code style guidelines in `.github/copilot-instructions.md`
- Write tests for new functionality
- Keep components under 200 lines
- Use TypeScript strict mode
- Follow existing patterns

### 5. Test Your Changes
```bash
# Run tests
npm test

# Run linter
npm run lint

# Build the project
npm run build
```

### 6. Commit Your Changes
```bash
# Using the template (if configured)
git commit

# Or manually following the format
git commit -m "feat(capture): add new feature"
```

### 7. Push to Your Fork
```bash
git push origin feat/your-feature-name
```

### 8. Create a Pull Request
- Go to the original repository
- Click "New Pull Request"
- Select your branch
- **Ensure your PR title follows Conventional Commits format**
- Fill out the PR template
- Submit the PR

## Code Style Guidelines

### TypeScript
- Use strict mode
- No `any` types (use `unknown` if truly dynamic)
- Prefer interfaces for objects
- Use type inference where possible

### React
- Functional components only (no class components)
- Use hooks for state and side effects
- Keep components focused and under 200 lines
- Separate business logic into `lib/` files

### CSS/Styling
- Use Tailwind CSS utility classes
- Follow the existing color scheme (OKLCH)
- Maintain animations at 200-300ms duration
- Ensure responsive design (mobile-first)

### File Naming
- Components: `PascalCase.tsx` (e.g., `CaptureInput.tsx`)
- Hooks: `camelCase.ts` with `use` prefix (e.g., `useItems.ts`)
- Utils/Lib: `camelCase.ts` (e.g., `typeInference.ts`)
- Types: `PascalCase` (e.g., `Item`, `ItemType`)

## Testing

### Writing Tests
- Use Vitest for unit/integration tests
- Use Playwright for E2E tests (when added)
- Test files: `*.test.ts` or `*.spec.ts`
- Aim for >80% coverage on business logic
- Mock external dependencies

### Test Structure
```typescript
import { describe, it, expect } from 'vitest'

describe('MyComponent', () => {
  it('should render correctly', () => {
    // Arrange
    const props = { /* ... */ }
    
    // Act
    const result = render(<MyComponent {...props} />)
    
    // Assert
    expect(result).toBeDefined()
  })
})
```

## Design Principles

This is an ADHD-friendly tool, so:

### DO ✅
- Minimize clicks and user friction
- Use generous whitespace
- Provide clear, immediate feedback
- Use soothing colors and smooth animations
- Keep UI simple and focused
- Test on mobile viewports

### DON'T ❌
- Add multi-step wizards or complex flows
- Change the carefully-designed color scheme
- Remove animations (they provide crucial feedback)
- Add pagination to lists (cognitive load)
- Commit secrets or API keys
- Add dependencies without discussion

## Release Process

This project uses [Release Please](https://github.com/googleapis/release-please) for automated versioning:

1. **Commits are analyzed**: Conventional Commits determine version bumps
2. **Release PR is created**: Automatically generated with changelog
3. **Merge to release**: When merged, a new release is published

Version bumping:
- `feat`: Bumps MINOR version (0.x.0)
- `fix`: Bumps PATCH version (0.0.x)
- `feat!` or `BREAKING CHANGE`: Bumps MAJOR version (x.0.0) when >= 1.0.0

## Questions or Issues?

- Read [PRD.md](./PRD.md) for requirements
- Check [.github/copilot-instructions.md](./.github/copilot-instructions.md) for detailed guidelines
- Review [TRANSITION-GUIDE.md](./TRANSITION-GUIDE.md) for architecture
- Open an issue for bugs or feature requests

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing to Divergent Flow! 🚀
