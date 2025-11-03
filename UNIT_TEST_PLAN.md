# Unit Test Plan - 80% Code Coverage Target

## Overview

**Goal**: Achieve 80%+ code coverage for SyncMedia.Core library  
**Framework**: xUnit + Moq + Coverlet  
**Target**: All 16 C# files in SyncMedia.Core  
**Estimated Effort**: 2-3 days  

## Files to Test (16 Total)

### Services (5 files - High Priority)
1. ✅ `LicenseManager.cs` - License validation and trial period
2. ✅ `SyncService.cs` - Core sync engine
3. ✅ `FeatureFlagService.cs` - Feature gating logic
4. ✅ `GamificationService.cs` - Achievement tracking
5. ✅ `AdvancedDuplicateDetectionService.cs` - Python/AI integration

### Helpers (4 files - Medium Priority)
6. ✅ `PerformanceBenchmark.cs` - Performance measurement
7. ✅ `GamificationPersistence.cs` - Save/load gamification data
8. ✅ `FileHelper.cs` - File operations utilities
9. ✅ `PerformanceOptimizer.cs` - Performance utilities

### Models (4 files - Medium Priority)
10. ✅ `GamificationData.cs` - Gamification data model
11. ✅ `AchievementData.cs` - Achievement model
12. ✅ `SyncStatistics.cs` - Sync statistics model
13. ✅ `LicenseInfo.cs` - License information model

### Constants (2 files - Low Priority)
14. ✅ `ProFeatures.cs` - Feature flag constants
15. ✅ `MediaConstants.cs` - Media file constants

### Data Access (1 file - Medium Priority)
16. ✅ `XmlData.cs` - XML persistence layer

## Test Strategy

### 1. Unit Test Structure
```csharp
namespace SyncMedia.Tests.[Category]
{
    public class [ClassName]Tests
    {
        // Arrange - Act - Assert pattern
        // One test class per production class
        // Multiple test methods per functionality
    }
}
```

### 2. Coverage Targets by Category

| Category | Files | Target Coverage | Priority |
|----------|-------|-----------------|----------|
| Services | 5 | 85%+ | P1 - Critical |
| Helpers | 4 | 80%+ | P2 - Important |
| Models | 4 | 90%+ | P2 - Important |
| Constants | 2 | 95%+ | P3 - Nice to have |
| Data Access | 1 | 75%+ | P2 - Important |

### 3. Test Categories

**Happy Path Tests** (50% of tests)
- Valid inputs
- Expected behaviors
- Successful operations

**Edge Case Tests** (30% of tests)
- Boundary values
- Empty/null inputs
- Large datasets

**Error Handling Tests** (20% of tests)
- Invalid inputs
- Exception scenarios
- Failure modes

## Detailed Test Plans

### Services Tests

#### 1. LicenseManager.cs Tests
```csharp
public class LicenseManagerTests
{
    [Fact] LoadLicense_ValidFile_ReturnsLicenseInfo
    [Fact] LoadLicense_NoFile_ReturnsFreeLicense
    [Fact] ValidateLicenseKey_ValidKey_ReturnsTrue
    [Fact] ValidateLicenseKey_InvalidKey_ReturnsFalse
    [Fact] ValidateLicenseKey_InvalidFormat_ReturnsFalse
    [Fact] ActivateLicense_ValidKey_SavesLicense
    [Fact] IsTrialPeriodActive_WithinTrialDays_ReturnsTrue
    [Fact] IsTrialPeriodActive_AfterTrialDays_ReturnsFalse
    [Fact] IsPro_WithProLicense_ReturnsTrue
    [Fact] IsPro_WithFreeLicense_ReturnsFalse
    [Fact] GenerateTestKey_CreatesValidKey
    [Theory] ValidateLicenseKey_VariousFormats_ValidatesCorrectly
}
```

#### 2. SyncService.cs Tests
```csharp
public class SyncServiceTests
{
    [Fact] StartSync_ValidPaths_CompletesSuccessfully
    [Fact] StartSync_InvalidSourcePath_ThrowsException
    [Fact] StartSync_InvalidDestPath_ThrowsException
    [Fact] StartSync_WithProgress_RaisesProgressEvents
    [Fact] StartSync_WithCancellation_StopsGracefully
    [Fact] StartSync_FindsDuplicates_ReturnsCorrectCount
    [Fact] StartSync_UpdatesStatistics_Correctly
    [Theory] StartSync_DifferentFileTypes_HandlesCorrectly
    [Fact] MoveFile_ValidPaths_MovesSuccessfully
    [Fact] MoveFile_FileExists_HandlesCorrectly
}
```

#### 3. FeatureFlagService.cs Tests
```csharp
public class FeatureFlagServiceTests
{
    [Fact] IsFeatureEnabled_ProLicense_AllFeaturesEnabled
    [Fact] IsFeatureEnabled_FreeLicense_OnlyBasicEnabled
    [Fact] IsFeatureEnabled_TrialActive_AllFeaturesEnabled
    [Fact] IsFeatureEnabled_TrialExpired_OnlyBasicEnabled
    [Theory] IsFeatureEnabled_AllFeatures_ReturnsCorrectly
    [Fact] Singleton_MultipleAccess_ReturnsSameInstance
}
```

#### 4. GamificationService.cs Tests
```csharp
public class GamificationServiceTests
{
    [Fact] CheckAndUnlockAchievements_FirstSync_UnlocksBeginnerAchievement
    [Fact] CheckAndUnlockAchievements_100Files_UnlocksOrganizedAchievement
    [Fact] CheckAndUnlockAchievements_1000Files_UnlocksPowerUserAchievement
    [Fact] CheckAndUnlockAchievements_AlreadyUnlocked_DoesNotDuplicate
    [Fact] CalculatePoints_SmallSync_ReturnsCorrectPoints
    [Fact] CalculatePoints_LargeSync_ReturnsCorrectPoints
    [Fact] SaveAndLoadData_PreservesAllProperties
    [Fact] Singleton_MultipleAccess_ReturnsSameInstance
}
```

#### 5. AdvancedDuplicateDetectionService.cs Tests
```csharp
public class AdvancedDuplicateDetectionServiceTests
{
    [Fact] CheckEnvironmentAsync_PythonInstalled_ReturnsAvailable
    [Fact] CheckEnvironmentAsync_PythonMissing_ReturnsUnavailable
    [Fact] FindDuplicatesAsync_ValidImages_ReturnsDuplicates
    [Fact] FindDuplicatesAsync_NoDuplicates_ReturnsEmpty
    [Fact] FindDuplicatesAsync_InvalidPath_ReturnsError
    [Theory] FindDuplicatesAsync_AllMethods_WorkCorrectly
    [Fact] TryGetBundledPython_NotInMSIX_ReturnsNull
    [Fact] FindSystemPython_PythonInPath_ReturnsPath
}
```

### Helpers Tests

#### 6. PerformanceBenchmark.cs Tests
```csharp
public class PerformanceBenchmarkTests
{
    [Fact] Start_AndStop_CalculatesElapsedTime
    [Fact] Reset_ClearsElapsedTime
    [Fact] MultipleStartStop_AccumulatesTime
}
```

#### 7. GamificationPersistence.cs Tests
```csharp
public class GamificationPersistenceTests
{
    [Fact] Save_ValidData_WritesFile
    [Fact] Load_ExistingFile_ReturnsData
    [Fact] Load_NoFile_ReturnsNewData
    [Fact] Save_InvalidPath_HandlesGracefully
    [Fact] RoundTrip_PreservesData
}
```

#### 8. FileHelper.cs Tests
```csharp
public class FileHelperTests
{
    [Fact] GetRelativePath_ValidPaths_ReturnsRelative
    [Fact] IsMediaFile_ImageFile_ReturnsTrue
    [Fact] IsMediaFile_VideoFile_ReturnsTrue
    [Fact] IsMediaFile_TextFile_ReturnsFalse
    [Theory] IsMediaFile_AllExtensions_ValidatesCorrectly
    [Fact] GetFileHash_SameFile_ReturnsSameHash
    [Fact] GetFileHash_DifferentFiles_ReturnsDifferentHash
}
```

#### 9. PerformanceOptimizer.cs Tests
```csharp
public class PerformanceOptimizerTests
{
    [Fact] OptimizeFileOperations_AppliesOptimizations
    [Fact] GetRecommendedBufferSize_ReturnsAppropriateSize
    [Theory] GetRecommendedBufferSize_DifferentSizes_ReturnsCorrectly
}
```

### Models Tests

#### 10. GamificationData.cs Tests
```csharp
public class GamificationDataTests
{
    [Fact] InitialState_HasZeroValues
    [Fact] AddSessionPoints_IncrementsBoth
    [Fact] ResetSessionPoints_ClearsSession
    [Fact] UpdateLifetimeStats_AddsValues
    [Fact] HasAchievement_ExistingAchievement_ReturnsTrue
    [Fact] HasAchievement_MissingAchievement_ReturnsFalse
    [Fact] AddAchievement_NewAchievement_Adds
    [Fact] AddAchievement_DuplicateAchievement_DoesNotDuplicate
}
```

#### 11. AchievementData.cs Tests
```csharp
public class AchievementDataTests
{
    [Fact] Constructor_SetsProperties
    [Fact] Equality_SameId_ReturnsTrue
    [Fact] Equality_DifferentId_ReturnsFalse
}
```

#### 12. SyncStatistics.cs Tests
```csharp
public class SyncStatisticsTests
{
    [Fact] InitialState_HasZeroValues
    [Fact] IncrementProcessedFiles_Increments
    [Fact] IncrementSuccessfulFiles_Increments
    [Fact] IncrementFailedFiles_Increments
    [Fact] AddBytesProcessed_AddsBytes
    [Fact] CalculateSuccessRate_ReturnsPercentage
}
```

#### 13. LicenseInfo.cs Tests
```csharp
public class LicenseInfoTests
{
    [Fact] Constructor_SetsProperties
    [Fact] IsPro_WithProType_ReturnsTrue
    [Fact] IsPro_WithFreeType_ReturnsFalse
    [Fact] IsTrialActive_WithinPeriod_ReturnsTrue
    [Fact] IsTrialActive_AfterPeriod_ReturnsFalse
}
```

### Constants Tests

#### 14. ProFeatures.cs Tests
```csharp
public class ProFeaturesTests
{
    [Fact] AllFeatureConstants_AreDefined
    [Theory] FeatureNames_AreUnique
}
```

#### 15. MediaConstants.cs Tests
```csharp
public class MediaConstantsTests
{
    [Fact] ImageExtensions_ContainsCommonFormats
    [Fact] VideoExtensions_ContainsCommonFormats
    [Theory] AllExtensions_StartWithDot
}
```

### Data Access Tests

#### 16. XmlData.cs Tests
```csharp
public class XmlDataTests
{
    [Fact] LoadSettings_ValidFile_ReturnsSettings
    [Fact] LoadSettings_NoFile_ReturnsDefaults
    [Fact] SaveSettings_ValidSettings_WritesFile
    [Fact] RoundTrip_PreservesAllSettings
    [Fact] LoadSettings_CorruptedFile_HandlesGracefully
}
```

## Test Infrastructure Setup

### 1. Project File (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
    <PackageReference Include="coverlet.msbuild" Version="6.0.4" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="Moq" Version="4.20.72" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\SyncMedia.Core\SyncMedia.Core.csproj" />
  </ItemGroup>
</Project>
```

### 2. Running Tests with Coverage
```bash
# Run tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generate HTML report (requires ReportGenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

### 3. CI/CD Integration
```yaml
# Example GitHub Actions workflow
- name: Run tests
  run: dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
  
- name: Check coverage
  run: |
    coverage=$(grep -oP 'line-rate="\K[0-9.]+' coverage.cobertura.xml | head -1)
    if (( $(echo "$coverage < 0.80" | bc -l) )); then
      echo "Coverage $coverage is below 80%"
      exit 1
    fi
```

## Implementation Order

### Phase 1: Foundation (Day 1 Morning)
1. ✅ Set up test project structure
2. ✅ Configure xUnit + Moq + Coverlet
3. ✅ Create base test classes
4. ✅ Implement Models tests (easiest, highest coverage)

### Phase 2: Core Logic (Day 1 Afternoon + Day 2 Morning)
5. ✅ Implement Services tests (most complex)
6. ✅ Focus on LicenseManager, FeatureFlagService
7. ✅ Implement SyncService tests (critical)

### Phase 3: Supporting Code (Day 2 Afternoon)
8. ✅ Implement Helpers tests
9. ✅ Implement Constants tests
10. ✅ Implement Data Access tests

### Phase 4: Polish & Coverage (Day 3)
11. ✅ Run coverage analysis
12. ✅ Fill gaps to reach 80%
13. ✅ Add edge case tests
14. ✅ Document test results

## Coverage Calculation

**Formula**: Lines Covered / Total Lines × 100

**Minimum per Category**:
- Services: 85% (most critical business logic)
- Helpers: 80% (utility functions)
- Models: 90% (simple, high-value coverage)
- Constants: 95% (trivial to test)
- Data Access: 75% (I/O can be complex)

**Overall Target**: 80%+ across all categories

## Success Metrics

✅ **Passing Tests**: All tests green  
✅ **Code Coverage**: 80%+ total coverage  
✅ **Test Quality**: 
   - Each test follows AAA pattern
   - Tests are independent
   - Tests are repeatable
   - Tests are fast (< 1s each)

✅ **Documentation**:
   - Test names describe scenarios
   - Complex tests have comments
   - Edge cases documented

## Benefits of 80% Coverage

1. **Quality Assurance**: Catch regressions early
2. **Refactoring Safety**: Change code with confidence
3. **Documentation**: Tests describe expected behavior
4. **Design Feedback**: Discover tight coupling issues
5. **Debugging Aid**: Isolate problems quickly
6. **Team Confidence**: Ship with less risk

## Maintenance

- Run tests before every commit
- Update tests when requirements change
- Review coverage reports monthly
- Add tests for bug fixes
- Keep tests fast and focused

## Next Steps

1. Create test project structure
2. Implement Phase 1 (Models tests)
3. Run initial coverage report
4. Continue with Phases 2-4
5. Achieve 80%+ coverage
6. Document results in UNIT_TEST_RESULTS.md

---

**Status**: ✅ Plan Complete - Ready for Implementation  
**Last Updated**: 2025-11-03  
**Owner**: Development Team  
**Priority**: P1 - High Priority
