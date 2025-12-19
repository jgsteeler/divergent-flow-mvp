# Testing Guide

This project uses [Vitest](https://vitest.dev/) for unit testing.

## Running Tests

```bash
# Run tests in watch mode (recommended during development)
npm test

# Run tests once
npm run test:run

# Run tests with UI
npm run test:ui

# Run tests with coverage report
npm run test:coverage
```

## Test Structure

Tests are located next to the files they test:
- `src/lib/typeInference.test.ts` - Tests for type inference logic
- `src/lib/reviewPriority.test.ts` - Tests for review priority calculations

## Writing Tests

Tests follow the Arrange-Act-Assert pattern:

```typescript
import { describe, it, expect } from 'vitest'
import { myFunction } from './myModule'

describe('myFunction', () => {
  it('should do something specific', () => {
    // Arrange
    const input = 'test'
    
    // Act
    const result = myFunction(input)
    
    // Assert
    expect(result).toBe('expected')
  })
})
```

## Testing Best Practices

1. **Test behavior, not implementation** - Focus on what the function does, not how it does it
2. **One assertion per test** - Makes failures easier to diagnose
3. **Descriptive test names** - Use `it('should do X when Y')` format
4. **Cover edge cases** - Test null values, empty inputs, extreme numbers
5. **Mock external dependencies** - Keep tests fast and isolated

## CI/CD Integration

Tests run automatically on:
- Every branch push
- Pull requests to main

The CI pipeline runs:
1. Linter (`npm run lint`)
2. Tests (`npm run test:run`)
3. Build (`npm run build`)

See `.github/workflows/test.yml` for the complete CI configuration.

## Test Coverage

Run `npm run test:coverage` to generate a coverage report. The report will be available in the `coverage` directory.

Coverage goals:
- **Business logic** (`src/lib/`): >80%
- **Components**: >60%
- **Critical paths**: 100%

## Debugging Tests

1. Use `it.only()` to run a single test
2. Use `describe.only()` to run a single test suite
3. Use `console.log()` for debugging (will show in test output)
4. Run with `--reporter=verbose` for more details

```bash
npm test -- --reporter=verbose
```
