/**
 * Commitlint configuration for Divergent Flow
 * Enforces Conventional Commits specification
 * 
 * @see https://www.conventionalcommits.org/
 * @see https://commitlint.js.org/
 */

export default {
  extends: ['@commitlint/config-conventional'],
  
  rules: {
    // Type must be one of the specified values
    'type-enum': [
      2,
      'always',
      [
        'feat',     // New feature
        'fix',      // Bug fix
        'docs',     // Documentation changes
        'style',    // Code style changes (formatting, etc.)
        'refactor', // Code refactoring
        'perf',     // Performance improvements
        'test',     // Adding or updating tests
        'chore',    // Build process, dependencies, or tooling
        'ci',       // CI configuration changes
        'build',    // Changes to build system or dependencies
        'revert',   // Revert a previous commit
      ],
    ],
    
    // Scope is optional but should be lowercase
    'scope-case': [2, 'always', 'lower-case'],
    'scope-enum': [
      0, // Warning only, not error
      'always',
      [
        'capture',
        'review',
        'inference',
        'ui',
        'storage',
        'types',
        'tests',
        'agent',
        'deps',
      ],
    ],
    
    // Subject must start with lowercase and not end with period
    'subject-case': [2, 'always', 'lower-case'],
    'subject-empty': [2, 'never'],
    'subject-full-stop': [2, 'never', '.'],
    'subject-max-length': [2, 'always', 72],
    
    // Type must be lowercase
    'type-case': [2, 'always', 'lower-case'],
    'type-empty': [2, 'never'],
    
    // Body and footer rules
    'body-leading-blank': [2, 'always'],
    'body-max-line-length': [2, 'always', 100],
    'footer-leading-blank': [2, 'always'],
    'footer-max-line-length': [2, 'always', 100],
    
    // Header (first line) maximum length
    'header-max-length': [2, 'always', 72],
  },
  
  // Custom parser options
  parserPreset: {
    parserOpts: {
      // Allow multiple body paragraphs
      issuePrefixes: ['#'],
    },
  },
  
  // Custom prompt for interactive mode
  prompt: {
    messages: {
      type: 'Select the type of change that you\'re committing:',
      scope: 'What is the scope of this change (e.g., capture, review, inference)?',
      subject: 'Write a short, imperative tense description of the change:',
      body: 'Provide a longer description of the change (optional):',
      breaking: 'List any BREAKING CHANGES (optional):',
      footer: 'List any issues closed by this change (e.g., #123):',
      confirmCommit: 'Are you sure you want to proceed with the commit above?',
    },
    questions: {
      type: {
        description: 'Select the type of change',
        enum: {
          feat: {
            description: 'A new feature',
            title: 'Features',
          },
          fix: {
            description: 'A bug fix',
            title: 'Bug Fixes',
          },
          docs: {
            description: 'Documentation only changes',
            title: 'Documentation',
          },
          style: {
            description: 'Code style changes (formatting, missing semi-colons, etc.)',
            title: 'Styles',
          },
          refactor: {
            description: 'A code change that neither fixes a bug nor adds a feature',
            title: 'Code Refactoring',
          },
          perf: {
            description: 'A code change that improves performance',
            title: 'Performance Improvements',
          },
          test: {
            description: 'Adding missing tests or correcting existing tests',
            title: 'Tests',
          },
          chore: {
            description: 'Changes to the build process or auxiliary tools',
            title: 'Chores',
          },
          ci: {
            description: 'Changes to CI configuration files and scripts',
            title: 'Continuous Integrations',
          },
          build: {
            description: 'Changes that affect the build system or external dependencies',
            title: 'Builds',
          },
        },
      },
      scope: {
        description: 'What is the scope of this change (e.g., capture, review)?',
      },
      subject: {
        description: 'Write a short, imperative tense description of the change',
      },
      body: {
        description: 'Provide a longer description of the change',
      },
      isBreaking: {
        description: 'Are there any breaking changes?',
      },
      breakingBody: {
        description: 'A BREAKING CHANGE commit requires a body. Please enter a longer description',
      },
      breaking: {
        description: 'Describe the breaking changes',
      },
      isIssueAffected: {
        description: 'Does this change affect any open issues?',
      },
      issuesBody: {
        description: 'If issues are closed, the commit requires a body. Please enter a longer description',
      },
      issues: {
        description: 'Add issue references (e.g., "fix #123", "re #456")',
      },
    },
  },
};
