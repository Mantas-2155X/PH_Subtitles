using System.Collections.Generic;

namespace UnityEngine.UI.Translation
{
	internal class SubtitleData : SubtitleDataBase
	{
		public string Key { get; protected set; }

		public SubtitleData()
		{
		}

		public SubtitleData(string path) : base(path)
		{
		}

		public SubtitleData(string path, List<SubtitleLine> value) : base(path, value)
		{
		}

		public SubtitleData(string path, string key, List<SubtitleLine> value) : base(path, value)
		{
			Key = key;
		}

		public override string ToString()
		{
			return string.Format("{0} {{ \"{1}\" ({2}) }}", Path, Key, Value.Count);
		}
	}
}
