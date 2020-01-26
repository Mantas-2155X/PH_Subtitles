namespace UnityEngine.UI.Translation
{
	internal class TranslationData : TranslationDataBase
	{
		protected string Key { get; }

		protected TranslationData() { }

		public TranslationData(string path) : base(path) { }

		public TranslationData(string path, string value) : base(path, value) { }

		protected TranslationData(string path, string key, string value) : base(path, value)
		{
			Key = key;
		}

		public override string ToString()
		{
			return $"{Path} {{ \"{Key}\" = \"{Value}\" }}";
		}
	}
}
