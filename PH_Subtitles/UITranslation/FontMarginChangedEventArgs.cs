using System;

namespace UnityEngine.UI.Translation
{
	internal class FontMarginChangedEventArgs : EventArgs
	{
		public float MarginLeft => _MarginLeft;

		public float MarginTop => _MarginTop;

		public float MarginRight => _MarginRight;

		public float MarginBottom => _MarginBottom;

		public FontMarginChangedEventArgs(float left, float top, float right, float bottom)
		{
			_MarginLeft = left;
			_MarginTop = top;
			_MarginRight = right;
			_MarginBottom = bottom;
		}

		private readonly float _MarginLeft;

		private readonly float _MarginTop;

		private readonly float _MarginRight;

		private readonly float _MarginBottom;
	}
}
