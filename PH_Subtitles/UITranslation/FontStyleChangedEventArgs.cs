using System;

namespace UnityEngine.UI.Translation
{
	internal class FontStyleChangedEventArgs : EventArgs
	{
		public bool Bold => _Bold;

		public bool Italic => _Italic;

		public FontStyleChangedEventArgs(bool bold, bool italic)
		{
			_Bold = bold;
			_Italic = italic;
		}

		private readonly bool _Bold;

		private readonly bool _Italic;
	}
}
