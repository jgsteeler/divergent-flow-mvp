# Changelog

## [4.6.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-ui-v4.5.0...divergent-flow-ui-v4.6.0) (2026-01-15)


### Features

* **workflow:** implement complete capture → review → dashboard workflow for open source launch ([#231](https://github.com/jgsteeler/divergent-flow-mvp/issues/231)) ([480ba65](https://github.com/jgsteeler/divergent-flow-mvp/commit/480ba658550076c376ca579eb183cb203a9954c0))

## [4.5.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-ui-v4.4.0...divergent-flow-ui-v4.5.0) (2026-01-09)


### Features

* **auth:** add Auth0 PKCE authentication flow ([#222](https://github.com/jgsteeler/divergent-flow-mvp/issues/222)) ([0fc3b64](https://github.com/jgsteeler/divergent-flow-mvp/commit/0fc3b64c3563baf7b5848d454ef579b392798700))

## [4.4.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-ui-v4.3.0...divergent-flow-ui-v4.4.0) (2026-01-06)


### Features

* **capture:** add confidence display and updatedAt field ([064b712](https://github.com/jgsteeler/divergent-flow-mvp/commit/064b712d2eadceacc5cf8ebae223de64cc115bf0))

## [4.3.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-ui-v4.2.0...divergent-flow-ui-v4.3.0) (2026-01-04)


### Features

* **infrastructure:** add MongoDB persistence with async inference queue ([#190](https://github.com/jgsteeler/divergent-flow-mvp/issues/190)) ([f93646d](https://github.com/jgsteeler/divergent-flow-mvp/commit/f93646d527b41ffc5b6a0a8049a37e5aa2c55769))


## [4.2.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-ui-v4.1.0...divergent-flow-ui-v4.2.0) (2025-12-28)


### Features

* **cors:** implement CORS policy with environment variable configura… ([#144](https://github.com/jgsteeler/divergent-flow-mvp/issues/144)) ([850c93e](https://github.com/jgsteeler/divergent-flow-mvp/commit/850c93e464443bc86841756d05c55ba345b3a332))

## [4.1.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/v4.0.0...v4.1.0) (2025-12-27)

### Features

* **api:** replace localStorage with backend API for capture operations ([#135](https://github.com/jgsteeler/divergent-flow-mvp/issues/135)) ([6045230](https://github.com/jgsteeler/divergent-flow-mvp/commit/60452306b201b2f5d8866278805adb635b40211e))

## [4.0.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/v3.3.0...v4.0.0) (2025-12-25)

### ⚠ BREAKING CHANGES

* **app:** strip to minimal capture and view functionality ([#116](https://github.com/jgsteeler/divergent-flow-mvp/issues/116))
* **inference:** TypeLearningData structure changed from pattern-based to keyword-based
* **inference:** TypeLearningData structure changed from pattern-based to keyword-based

### Features

* **agent:** enforce conventional commits for all commits and PR titles ([#83](https://github.com/jgsteeler/divergent-flow-mvp/issues/83)) ([1b2fd15](https://github.com/jgsteeler/divergent-flow-mvp/commit/1b2fd151fb323b400d01dee9fd19b0ecd559ea39))
* **api:** create .NET 10 Web API with CRUD endpoints, tests, and CI/CD ([#127](https://github.com/jgsteeler/divergent-flow-mvp/issues/127)) ([3129218](https://github.com/jgsteeler/divergent-flow-mvp/commit/31292186f5145021560bafd77c0bea8969c27f8f))
* **capture:** add InferredType and TypeConfidence properties to backend capture model ([#131](https://github.com/jgsteeler/divergent-flow-mvp/issues/131)) ([04bf644](https://github.com/jgsteeler/divergent-flow-mvp/commit/04bf644fe36768f5eb3e77647933db767ee06391))
* implement comprehensive inference review UI with learning feedback loop ([#84](https://github.com/jgsteeler/divergent-flow-mvp/issues/84)) ([489d842](https://github.com/jgsteeler/divergent-flow-mvp/commit/489d842496bb9656c0c22e1e6453e2729d153231))
* **inference:** enhance confidence calculation logic ([#97](https://github.com/jgsteeler/divergent-flow-mvp/issues/97)) ([350666c](https://github.com/jgsteeler/divergent-flow-mvp/commit/350666ce126790a0f2ee89e39394f4a9032c2e1c))
* replace @github/spark with localStorage for state management, update dependencies, and improve type inference ([#24](https://github.com/jgsteeler/divergent-flow-mvp/issues/24)) ([90d0781](https://github.com/jgsteeler/divergent-flow-mvp/commit/90d078195767d14df841ad1f4f35b03f09e64b70))
* **services:** create DivergentFlow.Services project and refactor architecture ([#129](https://github.com/jgsteeler/divergent-flow-mvp/issues/129)) ([c9a031b](https://github.com/jgsteeler/divergent-flow-mvp/commit/c9a031b50e8bfcd601c74fc9dd0cf128c5dd2148))

### Bug Fixes

* **workflow:** update PR title validation to follow Conventional Commits format ([#89](https://github.com/jgsteeler/divergent-flow-mvp/issues/89)) ([bda1fa2](https://github.com/jgsteeler/divergent-flow-mvp/commit/bda1fa2a6a8026219d4c37e59b28e8b93c96dc1d))

### Code Refactoring

* **app:** strip to minimal capture and view functionality ([#116](https://github.com/jgsteeler/divergent-flow-mvp/issues/116)) ([be6c374](https://github.com/jgsteeler/divergent-flow-mvp/commit/be6c374f9c1d8e2aed7f8f74017586fb98b2d86d))
* **inference:** implement keyword-based type learning system ([#93](https://github.com/jgsteeler/divergent-flow-mvp/issues/93)) ([0a6ce73](https://github.com/jgsteeler/divergent-flow-mvp/commit/0a6ce73a4c352c769098d75a022f0a0d32ec48ac))

## [3.3.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-mvp-v3.2.0...divergent-flow-mvp-v3.3.0) (2025-12-25)

### Features

* **capture:** add InferredType and TypeConfidence properties to backend capture model ([#131](https://github.com/jgsteeler/divergent-flow-mvp/issues/131)) ([04bf644](https://github.com/jgsteeler/divergent-flow-mvp/commit/04bf644fe36768f5eb3e77647933db767ee06391))

## [3.2.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-mvp-v3.1.0...divergent-flow-mvp-v3.2.0) (2025-12-24)

### Features

* **services:** create DivergentFlow.Services project and refactor architecture ([#129](https://github.com/jgsteeler/divergent-flow-mvp/issues/129)) ([c9a031b](https://github.com/jgsteeler/divergent-flow-mvp/commit/c9a031b50e8bfcd601c74fc9dd0cf128c5dd2148))

## [3.1.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-mvp-v3.0.0...divergent-flow-mvp-v3.1.0) (2025-12-24)

### Features

* **api:** create .NET 10 Web API with CRUD endpoints, tests, and CI/CD ([#127](https://github.com/jgsteeler/divergent-flow-mvp/issues/127)) ([3129218](https://github.com/jgsteeler/divergent-flow-mvp/commit/31292186f5145021560bafd77c0bea8969c27f8f))

## [3.0.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-mvp-v2.0.0...divergent-flow-mvp-v3.0.0) (2025-12-24)

### ⚠ BREAKING CHANGES

* **app:** strip to minimal capture and view functionality ([#116](https://github.com/jgsteeler/divergent-flow-mvp/issues/116))

### Code Refactoring

* **app:** strip to minimal capture and view functionality ([#116](https://github.com/jgsteeler/divergent-flow-mvp/issues/116)) ([be6c374](https://github.com/jgsteeler/divergent-flow-mvp/commit/be6c374f9c1d8e2aed7f8f74017586fb98b2d86d))

## [2.0.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-mvp-v1.2.0...divergent-flow-mvp-v2.0.0) (2025-12-23)

### ⚠ BREAKING CHANGES

* **inference:** TypeLearningData structure changed from pattern-based to keyword-based

### Code Refactoring

* **inference:** implement keyword-based type learning system ([#93](https://github.com/jgsteeler/divergent-flow-mvp/issues/93)) ([0a6ce73](https://github.com/jgsteeler/divergent-flow-mvp/commit/0a6ce73a4c352c769098d75a022f0a0d32ec48ac))

## [1.2.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-mvp-v1.1.1...divergent-flow-mvp-v1.2.0) (2025-12-21)

### Features

* implement comprehensive inference review UI with learning feedback loop ([#84](https://github.com/jgsteeler/divergent-flow-mvp/issues/84)) ([489d842](https://github.com/jgsteeler/divergent-flow-mvp/commit/489d842496bb9656c0c22e1e6453e2729d153231))

## [1.1.1](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-mvp-v1.1.0...divergent-flow-mvp-v1.1.1) (2025-12-20)

### Bug Fixes

* **workflow:** update PR title validation to follow Conventional Commits format ([#89](https://github.com/jgsteeler/divergent-flow-mvp/issues/89)) ([bda1fa2](https://github.com/jgsteeler/divergent-flow-mvp/commit/bda1fa2a6a8026219d4c37e59b28e8b93c96dc1d))

## [1.1.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-mvp-v1.0.0...divergent-flow-mvp-v1.1.0) (2025-12-19)

### Features

* **agent:** enforce conventional commits for all commits and PR titles ([#83](https://github.com/jgsteeler/divergent-flow-mvp/issues/83)) ([1b2fd15](https://github.com/jgsteeler/divergent-flow-mvp/commit/1b2fd151fb323b400d01dee9fd19b0ecd559ea39))

## 1.0.0 (2025-12-19)

### Features

* replace @github/spark with localStorage for state management, update dependencies, and improve type inference ([#24](https://github.com/jgsteeler/divergent-flow-mvp/issues/24)) ([90d0781](https://github.com/jgsteeler/divergent-flow-mvp/commit/90d078195767d14df841ad1f4f35b03f09e64b70))
