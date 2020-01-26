using System;

namespace UnityEngine.UI.Translation
{
	internal class FontBorderChangedEventArgs : EventArgs
	{
		public int BorderWidth => _BorderWidth;

		public FontBorderChangedEventArgs(int borderWidth)
		{
			_BorderWidth = borderWidth;
		}

		private readonly int _BorderWidth;
	}
}
