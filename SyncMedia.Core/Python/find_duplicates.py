"""
SyncMedia Advanced Duplicate Detection Service
Uses imagededup library for perceptual hashing and CNN-based duplicate detection
"""

import sys
import json
import os
from typing import List, Dict, Any, Optional

try:
    from imagededup.methods import PHash, DHash, WHash, CNN
    import numpy as np
except ImportError:
    print(json.dumps({
        "error": "imagededup not installed",
        "message": "Please install imagededup: pip install imagededup"
    }))
    sys.exit(1)


class DuplicateDetector:
    """Advanced duplicate detection using perceptual hashing and CNN"""
    
    def __init__(self, method: str = "phash", use_gpu: bool = False):
        """
        Initialize detector with specified method
        
        Args:
            method: Detection method (phash, dhash, whash, cnn)
            use_gpu: Whether to use GPU acceleration (CNN only)
        """
        self.method = method.lower()
        self.use_gpu = use_gpu and self._check_gpu_available()
        self.detector = self._create_detector()
    
    def _check_gpu_available(self) -> bool:
        """Check if GPU is available for acceleration"""
        try:
            import torch
            return torch.cuda.is_available()
        except ImportError:
            return False
    
    def _create_detector(self):
        """Create detector instance based on method"""
        if self.method == "phash":
            return PHash()
        elif self.method == "dhash":
            return DHash()
        elif self.method == "whash":
            return WHash()
        elif self.method == "cnn":
            return CNN()
        else:
            raise ValueError(f"Unknown method: {self.method}")
    
    def find_duplicates(
        self,
        image_paths: List[str],
        threshold: float = 0.9
    ) -> Dict[str, Any]:
        """
        Find duplicate images
        
        Args:
            image_paths: List of image file paths
            threshold: Similarity threshold (0-1)
        
        Returns:
            Dictionary with duplicate groups and statistics
        """
        try:
            # Validate all images exist
            valid_images = [p for p in image_paths if os.path.exists(p)]
            if not valid_images:
                return {
                    "error": "No valid image files found",
                    "duplicates": {},
                    "statistics": {
                        "total_files": len(image_paths),
                        "valid_files": 0,
                        "duplicate_groups": 0
                    }
                }
            
            # Create image directory mapping
            image_dir = os.path.dirname(valid_images[0])
            
            # Find duplicates based on method
            if self.method in ["phash", "dhash", "whash"]:
                # Perceptual hashing
                encodings = self.detector.encode_images(image_dir=image_dir)
                duplicates = self.detector.find_duplicates(
                    encoding_map=encodings,
                    max_distance_threshold=1.0 - threshold
                )
            else:
                # CNN-based detection
                encodings = self.detector.encode_images(image_dir=image_dir)
                duplicates = self.detector.find_duplicates(
                    encoding_map=encodings,
                    min_similarity_threshold=threshold
                )
            
            # Format results
            duplicate_groups = self._format_duplicates(duplicates)
            
            return {
                "success": True,
                "method": self.method,
                "gpu_used": self.use_gpu,
                "duplicates": duplicate_groups,
                "statistics": {
                    "total_files": len(image_paths),
                    "valid_files": len(valid_images),
                    "duplicate_groups": len(duplicate_groups),
                    "total_duplicates": sum(len(group) for group in duplicate_groups.values())
                }
            }
            
        except Exception as e:
            return {
                "error": str(e),
                "duplicates": {},
                "statistics": {
                    "total_files": len(image_paths),
                    "valid_files": len(valid_images) if 'valid_images' in locals() else 0,
                    "duplicate_groups": 0
                }
            }
    
    def _format_duplicates(self, duplicates: Dict) -> Dict[str, List[str]]:
        """Format duplicate results into groups"""
        groups = {}
        processed = set()
        
        for image, similar_images in duplicates.items():
            if image in processed:
                continue
            
            if similar_images:
                group_key = image
                groups[group_key] = similar_images
                processed.add(image)
                processed.update(similar_images)
        
        return groups


def main():
    """Main entry point for command-line usage"""
    if len(sys.argv) < 2:
        print(json.dumps({
            "error": "No input provided",
            "usage": "python find_duplicates.py <json_input>"
        }))
        sys.exit(1)
    
    try:
        # Parse input JSON
        input_data = json.loads(sys.argv[1])
        
        image_paths = input_data.get("images", [])
        method = input_data.get("method", "phash")
        threshold = input_data.get("threshold", 0.9)
        use_gpu = input_data.get("use_gpu", False)
        
        # Create detector and find duplicates
        detector = DuplicateDetector(method=method, use_gpu=use_gpu)
        results = detector.find_duplicates(image_paths, threshold)
        
        # Output results as JSON
        print(json.dumps(results, indent=2))
        
    except Exception as e:
        print(json.dumps({
            "error": str(e),
            "duplicates": {},
            "statistics": {}
        }))
        sys.exit(1)


if __name__ == "__main__":
    main()
