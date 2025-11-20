# Advanced Duplicate Detection - Technical Design

## Overview

This document describes the integration of **idealo/imagededup** for GPU-accelerated perceptual duplicate detection as a Pro-only feature in SyncMedia.

## Current vs Pro Duplicate Detection

### Current Implementation (Free Version)
- **Method**: MD5 cryptographic hashing
- **Detection**: Exact binary duplicates only
- **Speed**: Fast for exact matches
- **Limitations**: 
  - Misses similar images (crops, edits, filters)
  - Misses rotated/scaled duplicates
  - Misses same photo with different compression
  - No similarity scoring

### Pro Implementation (Advanced Detection)
- **Method**: Perceptual hashing + CNN-based deep learning
- **Detection**: Similar images with configurable threshold
- **Speed**: 10-100x faster with GPU acceleration
- **Capabilities**:
  - ✅ Finds cropped images
  - ✅ Finds edited photos (brightness, contrast, filters)
  - ✅ Finds rotated/scaled images
  - ✅ Finds re-compressed versions
  - ✅ Similarity scoring (0-100%)
  - ✅ Groups visually similar images

## Architecture

### Component Diagram

```
SyncMedia (C# .NET)
│
├── Free Version
│   └── MD5DuplicateDetector
│       └── Exact match detection (CPU)
│
└── Pro Version
    ├── MD5DuplicateDetector (fallback)
    └── AdvancedDuplicateDetector
        ├── Python Interop Layer
        │   ├── Subprocess communication
        │   ├── JSON serialization
        │   └── Error handling
        │
        └── Python Service (imagededup)
            ├── PHash (Perceptual Hash)
            ├── DHash (Difference Hash)
            ├── WHash (Wavelet Hash)
            └── CNN (Deep Learning)
                ├── MobileNet (fast)
                ├── ResNet50 (accurate)
                └── EfficientNet (balanced)
```

### Integration Options

#### Option 1: Subprocess Communication (Recommended)
**Pros:**
- Simple implementation
- Process isolation
- Easy debugging
- No dependency conflicts

**Cons:**
- Slower startup
- IPC overhead
- Additional process management

```csharp
public class PythonDuplicateService
{
    private Process _pythonProcess;
    
    public async Task<DuplicateResults> FindDuplicates(
        List<string> imagePaths, 
        DetectionMethod method = DetectionMethod.CNN)
    {
        // Start Python process
        var pythonScript = GetScriptPath("find_duplicates.py");
        var input = JsonConvert.SerializeObject(new
        {
            images = imagePaths,
            method = method.ToString().ToLower(),
            threshold = 0.9
        });
        
        // Execute and parse results
        var result = await ExecutePythonScript(pythonScript, input);
        return JsonConvert.DeserializeObject<DuplicateResults>(result);
    }
}
```

#### Option 2: Microservice (Alternative)
**Pros:**
- Scalable
- Can run on separate machine
- Easy to update independently

**Cons:**
- More complex deployment
- Network dependency
- Requires hosting

#### Option 3: IronPython (Not Recommended)
**Cons:**
- Limited library support
- Doesn't support TensorFlow/PyTorch
- Python 2.x only

## Python Service Implementation

### Directory Structure
```
SyncMedia.Pro/
├── Python/
│   ├── runtime/          # Embedded Python 3.8+
│   ├── scripts/
│   │   ├── find_duplicates.py
│   │   ├── encode_images.py
│   │   └── utils.py
│   └── requirements.txt
```

### find_duplicates.py
```python
#!/usr/bin/env python3
"""
SyncMedia Pro - Advanced Duplicate Detection Service
Uses imagededup for perceptual and CNN-based duplicate detection
"""

import sys
import json
from imagededup.methods import PHash, CNN, DHash, WHash
from pathlib import Path

def find_duplicates(config):
    """
    Find duplicates using specified method
    
    Args:
        config: {
            'images': List[str],
            'method': str ('phash'|'cnn'|'dhash'|'whash'),
            'threshold': float (0.0-1.0),
            'use_gpu': bool
        }
    
    Returns:
        {
            'duplicates': Dict[str, List[str]],
            'stats': {
                'total_groups': int,
                'total_duplicates': int,
                'processing_time': float
            }
        }
    """
    import time
    start_time = time.time()
    
    # Select detection method
    method = config.get('method', 'cnn').lower()
    threshold = config.get('threshold', 0.9)
    
    if method == 'phash':
        hasher = PHash()
    elif method == 'dhash':
        hasher = DHash()
    elif method == 'whash':
        hasher = WHash()
    elif method == 'cnn':
        hasher = CNN()
    else:
        raise ValueError(f"Unknown method: {method}")
    
    # Get image directory
    image_paths = config['images']
    if not image_paths:
        return {'duplicates': {}, 'stats': {}}
    
    # Find duplicates
    # Note: imagededup works best with a directory
    image_dir = str(Path(image_paths[0]).parent)
    
    # Encode images
    encodings = hasher.encode_images(image_dir=image_dir)
    
    # Find duplicates
    duplicates = hasher.find_duplicates(
        encoding_map=encodings,
        min_similarity_threshold=threshold
    )
    
    # Calculate stats
    total_groups = len(duplicates)
    total_duplicates = sum(len(v) for v in duplicates.values())
    
    return {
        'duplicates': duplicates,
        'stats': {
            'total_groups': total_groups,
            'total_duplicates': total_duplicates,
            'processing_time': time.time() - start_time,
            'method': method
        }
    }

if __name__ == '__main__':
    # Read config from stdin
    config = json.loads(sys.stdin.read())
    
    try:
        result = find_duplicates(config)
        print(json.dumps(result))
        sys.exit(0)
    except Exception as e:
        error = {
            'error': str(e),
            'type': type(e).__name__
        }
        print(json.dumps(error), file=sys.stderr)
        sys.exit(1)
```

## C# Integration Layer

### Interface Definition
```csharp
namespace SyncMedia.Pro.DuplicateDetection
{
    public enum DetectionMethod
    {
        /// <summary>Fast CPU-based perceptual hash</summary>
        PHash,
        
        /// <summary>Difference hash - good for rotations</summary>
        DHash,
        
        /// <summary>Wavelet hash - scale/rotation invariant</summary>
        WHash,
        
        /// <summary>CNN deep learning - most accurate, requires GPU</summary>
        CNN
    }
    
    public class DuplicateGroup
    {
        public string MasterImage { get; set; }
        public List<string> Duplicates { get; set; }
        public double SimilarityScore { get; set; }
    }
    
    public class DuplicateResults
    {
        public List<DuplicateGroup> Groups { get; set; }
        public DuplicateStats Stats { get; set; }
    }
    
    public class DuplicateStats
    {
        public int TotalGroups { get; set; }
        public int TotalDuplicates { get; set; }
        public double ProcessingTime { get; set; }
        public string Method { get; set; }
    }
    
    public interface IAdvancedDuplicateDetector
    {
        Task<DuplicateResults> FindDuplicatesAsync(
            List<string> imagePaths,
            DetectionMethod method = DetectionMethod.CNN,
            double threshold = 0.9,
            IProgress<int> progress = null);
            
        Task<bool> IsGpuAvailableAsync();
        Task<string> GetPythonVersionAsync();
    }
}
```

### Implementation
```csharp
namespace SyncMedia.Pro.DuplicateDetection
{
    public class AdvancedDuplicateDetector : IAdvancedDuplicateDetector
    {
        private readonly string _pythonExecutable;
        private readonly string _scriptsPath;
        
        public AdvancedDuplicateDetector()
        {
            // Get bundled Python runtime
            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            _pythonExecutable = Path.Combine(appPath, "Python", "runtime", "python.exe");
            _scriptsPath = Path.Combine(appPath, "Python", "scripts");
            
            if (!File.Exists(_pythonExecutable))
            {
                throw new InvalidOperationException(
                    "Python runtime not found. Please reinstall SyncMedia Pro.");
            }
        }
        
        public async Task<DuplicateResults> FindDuplicatesAsync(
            List<string> imagePaths,
            DetectionMethod method = DetectionMethod.CNN,
            double threshold = 0.9,
            IProgress<int> progress = null)
        {
            var scriptPath = Path.Combine(_scriptsPath, "find_duplicates.py");
            
            var config = new
            {
                images = imagePaths,
                method = method.ToString().ToLower(),
                threshold = threshold,
                use_gpu = await IsGpuAvailableAsync()
            };
            
            var configJson = JsonConvert.SerializeObject(config);
            
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = _pythonExecutable,
                    Arguments = $"\"{scriptPath}\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                process.Start();
                
                // Send config
                await process.StandardInput.WriteAsync(configJson);
                process.StandardInput.Close();
                
                // Read result
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                
                await process.WaitForExitAsync();
                
                if (process.ExitCode != 0)
                {
                    throw new Exception($"Python script failed: {error}");
                }
                
                // Parse results
                var pythonResult = JsonConvert.DeserializeObject<dynamic>(output);
                
                // Convert to C# objects
                var results = new DuplicateResults
                {
                    Groups = ParseDuplicateGroups(pythonResult.duplicates),
                    Stats = new DuplicateStats
                    {
                        TotalGroups = pythonResult.stats.total_groups,
                        TotalDuplicates = pythonResult.stats.total_duplicates,
                        ProcessingTime = pythonResult.stats.processing_time,
                        Method = pythonResult.stats.method
                    }
                };
                
                return results;
            }
        }
        
        public async Task<bool> IsGpuAvailableAsync()
        {
            try
            {
                var scriptPath = Path.Combine(_scriptsPath, "check_gpu.py");
                // ... check for CUDA/GPU availability
                return false; // Placeholder
            }
            catch
            {
                return false;
            }
        }
        
        private List<DuplicateGroup> ParseDuplicateGroups(dynamic duplicates)
        {
            // Convert Python dict to C# list
            var groups = new List<DuplicateGroup>();
            // ... implementation
            return groups;
        }
    }
}
```

## Deployment

### Python Runtime Bundling

1. **Create embeddable Python package**:
   ```bash
   # Download Python 3.10 embeddable
   # Install pip
   # Install dependencies
   pip install imagededup tensorflow-cpu
   ```

2. **Package structure**:
   ```
   SyncMedia.Pro.Installer/
   ├── SyncMedia.exe
   ├── Python/
   │   ├── python310.dll
   │   ├── python.exe
   │   ├── Lib/
   │   ├── Scripts/
   │   └── site-packages/
   │       ├── imagededup/
   │       ├── tensorflow/
   │       └── ...
   ```

3. **Installer size**: ~500MB (with TensorFlow-CPU)

### GPU Version (Optional)

For users with NVIDIA GPUs:
- Detect GPU at runtime
- Download CUDA + cuDNN + TensorFlow-GPU on first run
- Fall back to CPU if GPU unavailable

## User Experience

### UI Integration

#### Settings Panel (Pro)
```
Advanced Duplicate Detection
├─ [x] Enable AI-powered duplicate detection
├─ Detection Method: 
│   ├─ ( ) Fast - Perceptual Hash (CPU)
│   ├─ ( ) Balanced - Difference Hash (CPU)
│   └─ (•) Best - Deep Learning (GPU)
├─ Similarity Threshold: [========|--] 90%
├─ GPU Status: ✓ NVIDIA GTX 1660 (6GB)
└─ [Preview Similar Images]
```

#### During Sync
```
Finding Duplicates...
├─ Standard duplicates: 45 found (MD5)
└─ Similar images: 23 groups found (CNN)
    ├─ Group 1: IMG_001.jpg + 3 similar
    ├─ Group 2: IMG_045.jpg + 2 similar
    └─ ...
```

### Performance Expectations

| Method | Speed (1000 images) | Accuracy | GPU Required |
|--------|---------------------|----------|--------------|
| MD5 (Free) | ~5 sec | Exact only | No |
| PHash (Pro) | ~10 sec | Good | No |
| DHash (Pro) | ~8 sec | Good | No |
| CNN (Pro) | ~30 sec (CPU)<br>~3 sec (GPU) | Excellent | Optional |

## Testing Strategy

### Unit Tests
- [ ] Python service with mock images
- [ ] C# interop layer with test script
- [ ] Error handling (missing files, Python errors)
- [ ] GPU detection logic

### Integration Tests
- [ ] End-to-end duplicate detection
- [ ] Performance benchmarks
- [ ] Memory usage monitoring
- [ ] GPU vs CPU comparison

### User Acceptance Tests
- [ ] Find cropped images
- [ ] Find edited images (filters)
- [ ] Find rotated images
- [ ] Similarity threshold tuning
- [ ] False positive rate

## Security Considerations

1. **Sandboxing**: Python process runs with limited permissions
2. **Input Validation**: Validate all image paths before passing to Python
3. **Resource Limits**: Set memory and CPU limits for Python process
4. **Error Handling**: Don't expose Python stack traces to users

## Future Enhancements

1. **Cloud Processing**: Offload CNN processing to cloud API
2. **Custom Models**: Train on user's photo library
3. **Face Detection**: Group by people
4. **Scene Detection**: Group by location/context
5. **Deduplication Suggestions**: AI-powered keep/delete recommendations

## Dependencies

### Python Packages
```
imagededup==0.3.2
tensorflow==2.13.0  # or tensorflow-cpu
numpy>=1.21.0
pillow>=9.0.0
```

### C# NuGet Packages
```
Newtonsoft.Json
System.Diagnostics.Process (built-in)
```

## Cost Analysis

### Development Time
- Python integration: 1-2 days
- C# wrapper: 1-2 days
- UI integration: 2-3 days
- Testing: 2-3 days
- **Total**: 1-2 weeks

### Distribution Size
- Base app: ~50MB
- Python runtime: ~100MB
- TensorFlow (CPU): ~400MB
- **Total Pro version**: ~550MB

### Performance Impact
- Startup time: +2-3 seconds (Python init)
- Memory usage: +500MB (TensorFlow loaded)
- Disk space: +500MB

## Conclusion

The imagededup integration provides significant value for Pro users:
- **10x better duplicate detection** than simple MD5 hashing
- **GPU acceleration** for 10-100x speed improvement
- **Unique selling point** for Pro version
- **Competitive advantage** over other media management tools

This feature justifies the Pro version price and provides clear differentiation from the free version.
