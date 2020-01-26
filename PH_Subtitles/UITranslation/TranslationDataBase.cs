namespace UnityEngine.UI.Translation
{
	internal class TranslationDataBase
	{
		public string Path { get; protected set; }

		public string Value { get; protected set; }

		protected TranslationDataBase() { }

		protected TranslationDataBase(string path) : this(path, string.Empty) { }

		protected TranslationDataBase(string path, string value)
		{
			Path = path;
			Value = value;
		}

		public override string ToString()
		{
			return $"{Path} {{ \"{Value}\" }}";
		}
	}
}
