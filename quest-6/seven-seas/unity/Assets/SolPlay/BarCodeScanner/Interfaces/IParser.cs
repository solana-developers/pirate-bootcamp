using BarcodeScanner.Parser;
using UnityEngine;

namespace BarcodeScanner
{
	public interface IParser
	{
		ParserResult Decode(Color32[] colors, int width, int height);
	}
}