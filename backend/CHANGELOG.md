# Changelog

All notable changes to the Divergent Flow API will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
