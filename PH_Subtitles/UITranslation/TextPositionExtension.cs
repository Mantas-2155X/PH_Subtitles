using System;

namespace UnityEngine.UI.Translation
{
	internal enum TextPosition
	{
		LowerLeft = 1,
		LowerCenter,
		LowerRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		UpperLeft,
		UpperCenter,
		UpperRight
	}
	
	internal static class TextPositionExtension
	{
		public static TextPosition Parse(this TextPosition tp, string value, TextPosition defaultValue = TextPosition.LowerCenter)
		{
			TextPosition textPosition = 0;
			try
			{
				textPosition = (TextPosition)Enum.Parse(typeof(TextPosition), value, true);
			}
			catch
			{
				// ignored
			}

			if (!Enum.IsDefined(typeof(TextPosition), textPosition))
				textPosition = defaultValue;
			
			return textPosition;
		}
	}
}
