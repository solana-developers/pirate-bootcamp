using System;
using System.Linq;
using UnityEngine;

namespace BarcodeScanner
{
	public class ScannerSettings
	{
		// Scanner Options
		public bool ScannerBackgroundThread { get; set; }
		public int ScannerDelayFrameMin { get; set; }
		public float ScannerDecodeInterval { get; set; }

		// Parser Options
		public bool ParserAutoRotate { get; set; }
		public bool ParserTryInverted { get; set; }
		public bool ParserTryHarder { get; set; }

		// Webcam Options
		public string WebcamDefaultDeviceName { get; set; }
		public int WebcamRequestedWidth { get; set; }
		public int WebcamRequestedHeight { get; set; }
		public FilterMode WebcamFilterMode { get; set; }

		public ScannerSettings()
		{
			ScannerBackgroundThread = true;
			ScannerDelayFrameMin = 3;
			ScannerDecodeInterval = 0.1f;

			ParserAutoRotate = true;
			ParserTryInverted = true;
			ParserTryHarder = false;
			
			WebcamDefaultDeviceName = (WebCamTexture.devices.Length > 0) ? WebCamTexture.devices.First().name : "";
			WebcamRequestedWidth = 512;
			WebcamRequestedHeight = 512;
			WebcamFilterMode = FilterMode.Trilinear;

			// Device dependent settings

			// Disable background thread for webgl : Thread not supported
			#if UNITY_WEBGL
			ScannerDecodeInterval = 0.5f;
			ScannerBackgroundThread = false;
			#endif

			// Enable only for desktop usage : heavy CPU consumption
			#if UNITY_STANDALONE || UNITY_EDITOR
			ParserTryHarder = true;
			#endif
		}
	}
}
