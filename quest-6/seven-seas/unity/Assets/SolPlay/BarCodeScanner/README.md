# Unity Barcode Scanner
Small Barcode Scanner library for Unity
 
## Informations
**Author**: Kefniark

**Version**: 0.3

**Main Repository**: https://github.com/kefniark/UnityBarcodeScanner

**Samples**: [Here](https://github.com/kefniark/UnityBarcodeScanner/tree/master/Assets/Samples)

## Usage
Here is a basic example
```csharp
// Create a basic scanner
BarcodeScanner = new Scanner();

// Start playing the camera
BarcodeScanner.Camera.Play();

// Event when for the camera is ready to scan
BarcodeScanner.OnReady += (sender, arg) => {
    
    // Bind the Camera texture to any RawImage in your scene
    Image.texture = BarcodeScanner.Camera.Texture;

    // Start Scanning
    BarcodeScanner.Scan((barCodeType, barCodeValue) => {

        // This callback is call when something is scanned
        Debug.Log("Found: " + barCodeType + " / " + barCodeValue);
    });
};

...

void Update()
{
    // The barcode scanner has to be updated manually
	BarcodeScanner.Update();
}
```
Check the samples to have a better example of how to implement it.

## API
**Events**
```csharp
// trigger when the scanner can be used
event EventHandler OnReady;
// trigger when the status of the scanner change
event EventHandler StatusChanged;
```

**Properties**
```csharp
// Status of the scanner (enum with different values: Initialize, Running, Paused)
ScannerStatus Status { get; }
// The current parser used (by default ZXingParser)
IParser Parser { get; }
// The current camera used (by default UnityWebcam)
IWebcam Camera { get; }
```

**Method**
```csharp
// Start to scan (the callback provide the type and the value of any barcode found)
void Scan(Action<string, string> Callback);
// Stop the scan
void Stop();
// NEED to be call in Update or FixedUpdate
void Update();
// NEED to be call before leaving the scene
void Destroy();
```

## Changes

### 0.3 (30/11/2016)
* Changed how options are exposed
* Fixed Aspect Ratio, Rotation & Flip of the RawTexture
* Improved Logs
* Improved Performance (lower the amount of GC)
* Update samples (vsync, disabled auto-rotation)
* Tested with iOS (iPhone & iPad)

### 0.2 (24/09/2016)
* Implement Basic Samples
* Tested with WebGL & Desktop (pc/mac)
* Add lots of comments
* Fix an issue with releasing the camera when leaving the scene

### 0.1 (22/09/2016)
Just the startup of this small project.
Lots of elements are still missing but should arrive soon.
