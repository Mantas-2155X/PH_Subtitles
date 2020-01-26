using System;

namespace UnityEngine.UI.Translation
{
	internal class FontNameChangedEventArgs : EventArgs
	{
		public string FontName => _FontName;

		public FontNameChangedEventArgs(string name)
		{
			_FontName = name;
		}

		private readonly string _FontName;
	}
}
