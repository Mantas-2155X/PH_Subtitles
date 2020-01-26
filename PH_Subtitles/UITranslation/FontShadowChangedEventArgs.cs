using System;

namespace UnityEngine.UI.Translation
{
	internal class FontShadowChangedEventArgs : EventArgs
	{
		public int ShadowOffset => _ShadowOffset;

		public FontShadowChangedEventArgs(int shadowOffset)
		{
			_ShadowOffset = shadowOffset;
		}

		private readonly int _ShadowOffset;
	}
}
