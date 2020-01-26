using System;
using System.Globalization;

namespace UnityEngine.UI.Translation
{
	internal static class SubtitleSettings
	{
		public static string ToHex(Color32 color)
		{
			return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		}

		public static Color32 ToColor(string hex)
		{
			if (string.IsNullOrEmpty(hex) || !hex.StartsWith("#") || hex.Length != 7)
			{
				return Color.white;
			}
			byte r = byte.Parse(hex.Substring(1, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(3, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(5, 2), NumberStyles.HexNumber);
			return new Color32(r, g, b, byte.MaxValue);
		}
		
		internal static event Action<bool> EnabledChanged;

		internal static bool Enabled
		{
			get => _Enabled;
			private set
			{
				if (value != _Enabled)
				{
					_Enabled = value;
					Action<bool> enabledChanged = EnabledChanged;
					if (enabledChanged != null && initialized)
					{
						enabledChanged(value);
					}
				}
			}
		}

		internal static event Action<TextPosition> AnchorChanged;

		internal static TextPosition Anchor
		{
			get => _Anchor;
			private set
			{
				if (value != _Anchor)
				{
					_Anchor = value;
					Action<TextPosition> anchorChanged = AnchorChanged;
					if (anchorChanged != null && initialized)
					{
						anchorChanged(value);
					}
				}
			}
		}

		internal static event Action<string> FontNameChanged;

		internal static string FontName
		{
			get
			{
				if (string.IsNullOrEmpty(_FontName))
				{
					_FontName = "Arial";
				}
				return _FontName;
			}
			private set
			{
				if (string.IsNullOrEmpty(value))
				{
					value = "Arial";
				}
				if (value != _FontName)
				{
					_FontName = value;
					Action<string> fontNameChanged = FontNameChanged;
					if (fontNameChanged != null && initialized)
					{
						fontNameChanged(value);
					}
				}
			}
		}

		internal static event Action<int> FontSizeChanged;

		internal static int FontSize
		{
			get => _FontSize;
			private set
			{
				if (value != _FontSize)
				{
					_FontSize = value;
					Action<int> fontSizeChanged = FontSizeChanged;
					if (fontSizeChanged != null && initialized)
					{
						fontSizeChanged(value);
					}
				}
			}
		}

		internal static event Action<Color> FontColorChanged;

		internal static Color FontColor
		{
			get => _FontColor;
			private set
			{
				if (value != _FontColor)
				{
					_FontColor = value;
					Action<Color> fontColorChanged = FontColorChanged;
					if (fontColorChanged != null && initialized)
					{
						fontColorChanged(value);
					}
				}
			}
		}

		internal static event Action<bool> BoldChanged;

		internal static bool Bold
		{
			get => _Bold;
			private set
			{
				if (value != _Bold)
				{
					_Bold = value;
					Action<bool> boldChanged = BoldChanged;
					if (boldChanged != null && initialized)
					{
						boldChanged(value);
					}
				}
			}
		}

		internal static event Action<bool> ItalicChanged;

		internal static bool Italic
		{
			get => _Italic;
			private set
			{
				if (value != _Italic)
				{
					_Italic = value;
					Action<bool> italicChanged = ItalicChanged;
					if (italicChanged != null && initialized)
					{
						italicChanged(value);
					}
				}
			}
		}

		internal static event Action<int> BorderWidthChanged;

		internal static int BorderWidth
		{
			get => _BorderWidth;
			private set
			{
				if (value != _BorderWidth)
				{
					_BorderWidth = value;
					Action<int> borderWidthChanged = BorderWidthChanged;
					if (borderWidthChanged != null && initialized)
					{
						borderWidthChanged(value);
					}
				}
			}
		}

		internal static event Action<int> ShadowOffsetChanged;

		internal static int ShadowOffset
		{
			get => _ShadowOffset;
			private set
			{
				if (value != _ShadowOffset)
				{
					_ShadowOffset = value;
					Action<int> shadowOffsetChanged = ShadowOffsetChanged;
					if (shadowOffsetChanged != null && initialized)
					{
						shadowOffsetChanged(value);
					}
				}
			}
		}

		internal static event Action<int> MarginLeftChanged;

		internal static int MarginLeft
		{
			get => _MarginLeft;
			private set
			{
				if (value != _MarginLeft)
				{
					_MarginLeft = value;
					Action<int> marginLeftChanged = MarginLeftChanged;
					if (marginLeftChanged != null && initialized)
					{
						marginLeftChanged(value);
					}
				}
			}
		}

		internal static event Action<int> MarginTopChanged;

		internal static int MarginTop
		{
			get => _MarginTop;
			private set
			{
				if (value != _MarginTop)
				{
					_MarginTop = value;
					Action<int> marginTopChanged = MarginTopChanged;
					if (marginTopChanged != null && initialized)
					{
						marginTopChanged(value);
					}
				}
			}
		}

		internal static event Action<int> MarginRightChanged;

		internal static int MarginRight
		{
			get => _MarginRight;
			private set
			{
				if (value != _MarginRight)
				{
					_MarginRight = value;
					Action<int> marginRightChanged = MarginRightChanged;
					if (marginRightChanged != null && initialized)
					{
						marginRightChanged(value);
					}
				}
			}
		}

		internal static event Action<int> MarginBottomChanged;

		internal static int MarginBottom
		{
			get => _MarginBottom;
			private set
			{
				if (value != _MarginBottom)
				{
					_MarginBottom = value;
					Action<int> marginBottomChanged = MarginBottomChanged;
					if (marginBottomChanged != null && initialized)
					{
						marginBottomChanged(value);
					}
				}
			}
		}

		static SubtitleSettings()
		{
			IniSettings.LoadSettings += Load;
			IniSettings.Load();
		}

		private static void Load(IniFile ini)
		{
			string key = "bEnabled";
			string value = ini.GetValue("Subtitles", key);
			bool flag;
			if (value == null || !bool.TryParse(value, out flag))
			{
				flag = true;
				ini.WriteValue("Subtitles", key, flag);
			}
			Enabled = flag;
			key = "iAnchor";
			value = ini.GetValue("Subtitles", key);
			int num;
			if (value == null || !int.TryParse(value, out num))
			{
				num = 2;
				ini.WriteValue("Subtitles", key, num);
			}
			Anchor = Anchor.Parse(num.ToString(), Anchor);
			key = "sFontName";
			value = ini.GetValue("Subtitles", key);
			FontName = value;
			if (value != FontName)
			{
				ini.WriteValue("Subtitles", key, FontName);
			}
			key = "iFontSize";
			value = ini.GetValue("Subtitles", key);
			if (value == null || !int.TryParse(value, out num))
			{
				num = 16;
				ini.WriteValue("Subtitles", key, num);
			}
			FontSize = num;
			key = "sFontColor";
			value = ini.GetValue("Subtitles", key);
			FontColor = ToColor(value);
			string text = ToHex(FontColor);
			if (value != text)
			{
				ini.WriteValue("Subtitles", key, text);
			}
			key = "bBold";
			value = ini.GetValue("Subtitles", key);
			if (value == null || !bool.TryParse(value, out flag))
			{
				flag = true;
				ini.WriteValue("Subtitles", key, flag);
			}
			Bold = flag;
			key = "bItalic";
			value = ini.GetValue("Subtitles", key);
			if (value == null || !bool.TryParse(value, out flag))
			{
				flag = false;
				ini.WriteValue("Subtitles", key, flag);
			}
			Italic = flag;
			key = "iBorderWidth";
			value = ini.GetValue("Subtitles", key);
			if (value == null || !int.TryParse(value, out num))
			{
				num = 2;
				ini.WriteValue("Subtitles", key, num);
			}
			BorderWidth = num;
			key = "iShadowOffset";
			value = ini.GetValue("Subtitles", key);
			if (value == null || !int.TryParse(value, out num))
			{
				num = 3;
				ini.WriteValue("Subtitles", key, num);
			}
			ShadowOffset = num;
			key = "iMarginLeft";
			value = ini.GetValue("Subtitles", key);
			if (value == null || !int.TryParse(value, out num))
			{
				num = 20;
				ini.WriteValue("Subtitles", key, num);
			}
			MarginLeft = num;
			key = "iMarginTop";
			value = ini.GetValue("Subtitles", key);
			if (value == null || !int.TryParse(value, out num))
			{
				num = 20;
				ini.WriteValue("Subtitles", key, num);
			}
			MarginTop = num;
			key = "iMarginRight";
			value = ini.GetValue("Subtitles", key);
			if (value == null || !int.TryParse(value, out num))
			{
				num = 20;
				ini.WriteValue("Subtitles", key, num);
			}
			MarginRight = num;
			key = "iMarginBottom";
			value = ini.GetValue("Subtitles", key);
			if (value == null || !int.TryParse(value, out num))
			{
				num = 20;
				ini.WriteValue("Subtitles", key, num);
			}
			MarginBottom = num;
			initialized = true;
		}

		private const string SECTION = "Subtitles";

		private const string ENABLEDKEY = "bEnabled";

		private const string ANCHORKEY = "iAnchor";

		private const string FONTNAMEKEY = "sFontName";

		private const string FONTSIZEKEY = "iFontSize";

		private const string FONTCOLORKEY = "sFontColor";

		private const string BOLDKEY = "bBold";

		private const string ITALICKEY = "bItalic";

		private const string BORDERWIDTHKEY = "iBorderWidth";

		private const string SHADOWOFFSETKEY = "iShadowOffset";

		private const string MARGINLEFTKEY = "iMarginLeft";

		private const string MARGINTOPKEY = "iMarginTop";

		private const string MARGINRIGHTKEY = "iMarginRight";

		private const string MARGINBOTTOMKEY = "iMarginBottom";

		private static bool _Enabled;

		private static TextPosition _Anchor;

		private static string _FontName;

		private static int _FontSize;

		private static Color _FontColor;

		private static bool _Bold;

		private static bool _Italic;

		private static int _BorderWidth;

		private static int _ShadowOffset;

		private static int _MarginLeft;

		private static int _MarginTop;

		private static int _MarginRight;

		private static int _MarginBottom;

		private static bool initialized;
	}
}
