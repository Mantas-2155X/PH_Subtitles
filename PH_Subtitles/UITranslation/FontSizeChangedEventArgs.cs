using System;

namespace UnityEngine.UI.Translation
{
	internal class FontSizeChangedEventArgs : EventArgs
	{
		public int FontSize => _FontSize;

		public FontSizeChangedEventArgs(int size)
		{
			_FontSize = size;
		}

		private readonly int _FontSize;
	}
}
