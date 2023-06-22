using UnityEngine;
using Wizcorp.Utils.Logger;

namespace BarcodeScanner.Webcam
{
	public class UnityWebcam : IWebcam
	{
		public Texture Texture { get { return Webcam; } }
		public WebCamTexture Webcam { get; private set; }

		public Vector2 Size { get { return new Vector2(Webcam.width, Webcam.height); } }
		public int Width { get; private set; }
		public int Height { get; private set; }

		public UnityWebcam() : this(new ScannerSettings())
		{
		}

		public UnityWebcam(ScannerSettings settings)
		{
			// Create Webcam Texture
			Webcam = new WebCamTexture(settings.WebcamDefaultDeviceName);
			Webcam.requestedWidth = settings.WebcamRequestedWidth;
			Webcam.requestedHeight = settings.WebcamRequestedHeight;
			Webcam.filterMode = settings.WebcamFilterMode;

			// Get size
			Width = 0;
			Height = 0;
		}

		public void SetSize()
		{
			Width = Mathf.RoundToInt(Webcam.width);
			Height = Mathf.RoundToInt(Webcam.height);
		}

		public bool IsReady()
		{
			return Webcam != null && Webcam.width >= 100 && Webcam.videoRotationAngle % 90 == 0;
		}

		public bool IsPlaying()
		{
			return Webcam.isPlaying;
		}

		public void Play()
		{
			Webcam.Play();
		}

		public void Stop()
		{
			Webcam.Stop();
		}

		public void Destroy()
		{
			if (Webcam.isPlaying)
			{
				Webcam.Stop();
			}
		}

		public Color32[] GetPixels(Color32[] data = null)
		{
			if (data == null || data.Length != Webcam.width * Webcam.height)
			{
				return Webcam.GetPixels32();
			}
			return Webcam.GetPixels32(data);
		}

		public float GetRotation()
		{
			return -Webcam.videoRotationAngle;
		}

		public bool IsVerticalyMirrored()
		{
			return Webcam.videoVerticallyMirrored;
		}

		public Vector3 GetEulerAngles()
		{
			return new Vector3(0f, 0f, GetRotation());
		}

		public Vector3 GetScale()
		{
			return new Vector3(1, IsVerticalyMirrored() ? -1f : 1f, 1);
		}

		public int GetChecksum()
		{
			return (Webcam.width + Webcam.height + Webcam.deviceName + Webcam.videoRotationAngle).GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("[UnityWebcam] Camera: {2} | Resolution: {0}x{1} | Orientation: {3}", Width, Height, Webcam.deviceName, Webcam.videoRotationAngle + ":" + Webcam.videoVerticallyMirrored);
		}
	}
}
