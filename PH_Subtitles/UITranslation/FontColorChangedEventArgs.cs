using System;

namespace UnityEngine.UI.Translation
{
	internal class FontColorChangedEventArgs : EventArgs
	{
		public Color FontColor => _FontColor;

		public FontColorChangedEventArgs(Color color)
		{
			_FontColor = color;
		}

		private readonly Color _FontColor;
	}
}
