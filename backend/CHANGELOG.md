# Changelog

All notable changes to the Divergent Flow API will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-api-v1.0.0...divergent-flow-api-v2.0.0) (2025-12-30)


### ⚠ BREAKING CHANGES

* **deploy:** add Fly.io deployment configurations for backend (staging and production) ([#142](https://github.com/jgsteeler/divergent-flow-mvp/issues/142))

### Features

* **api:** add /hello endpoint and corresponding test ([#139](https://github.com/jgsteeler/divergent-flow-mvp/issues/139)) ([1439187](https://github.com/jgsteeler/divergent-flow-mvp/commit/1439187cb7c1156429706e71f50c69b57bd67de2))
* **cors:** implement CORS policy with environment variable configura… ([#144](https://github.com/jgsteeler/divergent-flow-mvp/issues/144)) ([850c93e](https://github.com/jgsteeler/divergent-flow-mvp/commit/850c93e464443bc86841756d05c55ba345b3a332))
* **deploy:** add Fly.io deployment configurations for backend (staging and production) ([#142](https://github.com/jgsteeler/divergent-flow-mvp/issues/142)) ([c2f657c](https://github.com/jgsteeler/divergent-flow-mvp/commit/c2f657c24e2dcd1a687e4b49a6b495ec6b3a0a96))
* redis capture repository ([#167](https://github.com/jgsteeler/divergent-flow-mvp/issues/167)) ([3539697](https://github.com/jgsteeler/divergent-flow-mvp/commit/3539697419ec193c2c0893d97a76adbc277ecdfa))
* solid architeture ([#159](https://github.com/jgsteeler/divergent-flow-mvp/issues/159)) ([4d2af17](https://github.com/jgsteeler/divergent-flow-mvp/commit/4d2af17d5af75d66c39a7b19d89c57ab82fc9583))
* **storage:** replace in-memory storage with Redis/Upstash repository pattern ([#155](https://github.com/jgsteeler/divergent-flow-mvp/issues/155)) ([4964198](https://github.com/jgsteeler/divergent-flow-mvp/commit/496419847db3d991b491128c61c68729534c4a42))
* **type:** create type inference interface and controller ([#156](https://github.com/jgsteeler/divergent-flow-mvp/issues/156)) ([64278d0](https://github.com/jgsteeler/divergent-flow-mvp/commit/64278d0c23b858826ad12367f45de206f0cbddf9))

## [0.2.5](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-api-v0.2.4...divergent-flow-api-v0.2.5) (2025-12-30)


### Features

* redis capture repository ([#167](https://github.com/jgsteeler/divergent-flow-mvp/issues/167)) ([3539697](https://github.com/jgsteeler/divergent-flow-mvp/commit/3539697419ec193c2c0893d97a76adbc277ecdfa))

## [0.2.4](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-api-v0.2.3...divergent-flow-api-v0.2.4) (2025-12-30)


### Features

* solid architeture ([#159](https://github.com/jgsteeler/divergent-flow-mvp/issues/159)) ([4d2af17](https://github.com/jgsteeler/divergent-flow-mvp/commit/4d2af17d5af75d66c39a7b19d89c57ab82fc9583))

## [0.2.3](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-api-v0.2.2...divergent-flow-api-v0.2.3) (2025-12-29)


### Features

* **storage:** replace in-memory storage with Redis/Upstash repository pattern ([#155](https://github.com/jgsteeler/divergent-flow-mvp/issues/155)) ([4964198](https://github.com/jgsteeler/divergent-flow-mvp/commit/496419847db3d991b491128c61c68729534c4a42))

## [0.2.2](https://github.com/jgsteeler/divergent-flow-mvp/compare/divergent-flow-api-v0.2.1...divergent-flow-api-v0.2.2) (2025-12-29)


### Features

* **type:** create type inference interface and controller ([#156](https://github.com/jgsteeler/divergent-flow-mvp/issues/156)) ([64278d0](https://github.com/jgsteeler/divergent-flow-mvp/commit/64278d0c23b858826ad12367f45de206f0cbddf9))

## [Unreleased]

## [0.1.0] - 2025-12-24

### Added
- Initial .NET 10 Web API implementation
- CRUD endpoints for capture items
  - GET /api/captures - List all captures
  - POST /api/captures - Create new capture
  - GET /api/captures/{id} - Get single capture
  - PUT /api/captures/{id} - Update capture
  - DELETE /api/captures/{id} - Delete capture
- Swagger/OpenAPI documentation
- CORS configuration for frontend integration
- Dependency injection architecture with service interfaces
- In-memory storage implementation (temporary)
- Integration test suite with xUnit
- CI/CD pipeline for building and testing
