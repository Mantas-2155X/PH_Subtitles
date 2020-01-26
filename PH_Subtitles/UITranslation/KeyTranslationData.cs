using System;

namespace UnityEngine.UI.Translation
{
	internal class KeyTranslationData : TranslationDataBase
	{
		public string Key { get; private set; }

		public KeyTranslationData()
		{
		}

		public KeyTranslationData(string key, TranslationDataBase data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			Path = data.Path;
			Key = key;
			Value = data.Value;
		}

		public KeyTranslationData(string path, string key, string value) : base(path, value)
		{
			Key = key;
		}

		public override string ToString()
		{
			return string.Format("{0} {{ \"{1}\", \"{2}\" }}", Path, Key, Value);
		}
	}
}
