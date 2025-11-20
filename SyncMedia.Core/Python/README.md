# SyncMedia Advanced Duplicate Detection (Python Integration)

This directory contains Python scripts for AI-powered duplicate detection using the `imagededup` library.

## Features

- **Perceptual Hashing (PHash)**: Fast detection of exact and near-exact duplicates
- **Difference Hash (DHash)**: Good for finding similar images
- **Wavelet Hash (WHash)**: More accurate but slower
- **CNN-based Detection**: Deep learning for finding visually similar images (requires GPU for speed)

## Installation

### Prerequisites

- Python 3.8 or higher
- pip (Python package manager)

### Install Dependencies

```bash
cd SyncMedia.Core/Python
pip install -r requirements.txt
```

### Optional: GPU Support

For CNN-based detection with GPU acceleration:

1. Install NVIDIA CUDA Toolkit 11.8 or higher
2. Install PyTorch with CUDA support:

```bash
pip install torch torchvision --index-url https://download.pytorch.org/whl/cu118
```

## Usage

### Command Line

```bash
python find_duplicates.py '{"images": ["path/to/image1.jpg", "path/to/image2.jpg"], "method": "phash", "threshold": 0.9, "use_gpu": false}'
```

### From C#

The `AdvancedDuplicateDetectionService` class handles Python interop:

```csharp
var service = new AdvancedDuplicateDetectionService();

// Check Python environment
var status = await service.CheckEnvironmentAsync();
if (!status.IsAvailable)
{
    Console.WriteLine($"Python not available: {status.Message}");
    return;
}

// Find duplicates
var imagePaths = new List<string> { "image1.jpg", "image2.jpg", "image3.jpg" };
var result = await service.FindDuplicatesAsync(
    imagePaths,
    DetectionMethod.PHash,
    threshold: 0.9,
    useGpu: false
);

if (result.Success)
{
    Console.WriteLine($"Found {result.DuplicateGroupCount} duplicate groups");
    foreach (var group in result.DuplicateGroups)
    {
        Console.WriteLine($"Original: {group.Key}");
        foreach (var duplicate in group.Value)
        {
            Console.WriteLine($"  - {duplicate}");
        }
    }
}
```

## Detection Methods

### PHash (Perceptual Hash)
- **Speed**: Fast (~100-200 images/second)
- **Accuracy**: Good for exact duplicates
- **Use Case**: Finding identical images with minor compression differences

### DHash (Difference Hash)
- **Speed**: Very fast (~200-300 images/second)
- **Accuracy**: Good for similar images
- **Use Case**: Finding images with minor edits

### WHash (Wavelet Hash)
- **Speed**: Slower (~50-100 images/second)
- **Accuracy**: Better for rotated/scaled images
- **Use Case**: Finding duplicates across different sizes

### CNN (Convolutional Neural Network)
- **Speed**: Slow without GPU (~5-10 images/second), fast with GPU (~50-100 images/second)
- **Accuracy**: Excellent for visually similar images
- **Use Case**: Finding images that look similar but may have significant edits

## Threshold

The similarity threshold determines how strict the matching is:

- **0.9-1.0**: Very strict (exact or near-exact duplicates only)
- **0.7-0.9**: Moderate (similar images with minor differences)
- **0.5-0.7**: Lenient (visually similar images)

## Performance

### CPU Performance
- **PHash**: ~100-200 images/second
- **DHash**: ~200-300 images/second
- **WHash**: ~50-100 images/second
- **CNN**: ~5-10 images/second

### GPU Performance (with CUDA)
- **CNN**: ~50-100 images/second (10-20x faster)

## Troubleshooting

### "imagededup not installed"
```bash
pip install imagededup
```

### "CUDA not available" (for GPU acceleration)
1. Install NVIDIA CUDA Toolkit
2. Reinstall PyTorch with CUDA support
3. Verify CUDA is available: `python -c "import torch; print(torch.cuda.is_available())"`

### "Permission denied" on Python script
```bash
chmod +x find_duplicates.py
```

## License

This Python integration uses the imagededup library which is licensed under Apache License 2.0.
