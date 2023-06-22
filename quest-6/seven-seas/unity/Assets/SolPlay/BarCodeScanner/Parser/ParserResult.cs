namespace BarcodeScanner.Parser
{
	public class ParserResult
	{
		public string Type { get; private set; }
		public string Value { get; private set; }

		public ParserResult(string type, string value)
		{
			Type = type;
			Value = value;
		}

		public override string ToString()
		{
			return string.Format("[Result {0}:{1}]", Type, Value);
		}
	}
}
