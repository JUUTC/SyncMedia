# SyncMedia.Tests

Comprehensive unit test suite for SyncMedia.Core library with 80%+ code coverage target.

## Test Structure

- **Models/** - Tests for data models (LicenseInfo, GamificationData, SyncStatistics)
- **Services/** - Tests for core services (LicenseManager, GamificationService, FeatureFlagService)
- **Helpers/** - Tests for utility classes (FileHelper, PerformanceBenchmark, GamificationPersistence)
- **Constants/** - Tests for constant definitions (ProFeatures, MediaConstants)

## Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generate HTML coverage report (requires reportgenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

# Open coverage report
open coveragereport/index.html  # macOS
xdg-open coveragereport/index.html  # Linux
start coveragereport/index.html  # Windows
```

## Test Coverage Goals

- **Overall**: 80%+ total coverage
- **Services**: 85%+ coverage (critical business logic)
- **Helpers**: 80%+ coverage (utility functions)
- **Models**: 90%+ coverage (data validation)
- **Constants**: 95%+ coverage (simple validation)

## Test Frameworks

- **xUnit** - Test framework
- **Moq** - Mocking framework for dependencies
- **Coverlet** - Code coverage collector

## Current Coverage

Run `dotnet test /p:CollectCoverage=true` to see current coverage metrics.
