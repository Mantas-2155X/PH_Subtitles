using System.Collections.Generic;

namespace UnityEngine.UI.Translation
{
	internal abstract class SubtitleUserInterfaceBase<T> : MonoBehaviour, ISubtitleUserInterface where T : MonoBehaviour
	{
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<T>();
					if (_instance == null)
					{
						GameObject gameObject = new GameObject("SubtitleUserInterface");
						_instance = gameObject.AddComponent<T>();
						DontDestroyOnLoad(gameObject);
					}
				}
				return _instance;
			}
		}

		public string FontName
		{
			get => _FontName;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					value = "Arial";
				}
				if (value != _FontName)
				{
					_FontName = value;
					OnFontNameChanged(this, new FontNameChangedEventArgs(value));
				}
			}
		}

		public int FontSize
		{
			get => _FontSize;
			set
			{
				value = Mathf.Min(Mathf.Max(value, 8), 72);
				if (value != _FontSize)
				{
					_FontSize = value;
					OnFontSizeChanged(this, new FontSizeChangedEventArgs(value));
				}
			}
		}

		public Color FontColor
		{
			get => _FontColor;
			set
			{
				_FontColor.a = 1f;
				if (value == _FontColor) 
					return;
				
				_FontColor = value;
				OnFontColorChanged(this, new FontColorChangedEventArgs(value));
			}
		}

		public bool Bold
		{
			get => _Bold;
			set
			{
				if (value != _Bold)
				{
					_Bold = value;
					OnFontStyleChanged(this, new FontStyleChangedEventArgs(value, Italic));
				}
			}
		}

		public bool Italic
		{
			get => _Italic;
			set
			{
				if (value != _Italic)
				{
					_Italic = value;
					OnFontStyleChanged(this, new FontStyleChangedEventArgs(Bold, value));
				}
			}
		}

		public float MarginLeft
		{
			get => _MarginLeft;
			set
			{
				if (value != _MarginLeft)
				{
					_MarginLeft = value;
					OnFontMarginChanged(this, new FontMarginChangedEventArgs(value, MarginTop, MarginRight, MarginBottom));
				}
			}
		}

		public float MarginTop
		{
			get => _MarginTop;
			set
			{
				if (value != _MarginTop)
				{
					_MarginTop = value;
					OnFontMarginChanged(this, new FontMarginChangedEventArgs(MarginLeft, value, MarginRight, MarginBottom));
				}
			}
		}

		public float MarginRight
		{
			get => _MarginRight;
			set
			{
				if (value != _MarginRight)
				{
					_MarginRight = value;
					OnFontMarginChanged(this, new FontMarginChangedEventArgs(MarginLeft, MarginTop, value, MarginBottom));
				}
			}
		}

		public float MarginBottom
		{
			get => _MarginBottom;
			set
			{
				if (value != _MarginBottom)
				{
					_MarginBottom = value;
					OnFontMarginChanged(this, new FontMarginChangedEventArgs(MarginLeft, MarginTop, MarginRight, value));
				}
			}
		}

		public abstract IEnumerable<TextPosition> Anchors { get; }

		public abstract string this[TextPosition anchor]
		{
			get;
			set;
		}

		protected SubtitleUserInterfaceBase()
		{
			_FontName = "Arial";
			_FontSize = 16;
			_FontColor = Color.white;
		}

		protected virtual void OnFontNameChanged(object sender, FontNameChangedEventArgs e)
		{
		}

		protected virtual void OnFontSizeChanged(object sender, FontSizeChangedEventArgs e)
		{
		}

		protected virtual void OnFontColorChanged(object sender, FontColorChangedEventArgs e)
		{
		}

		protected virtual void OnFontStyleChanged(object sender, FontStyleChangedEventArgs e)
		{
		}

		protected virtual void OnFontMarginChanged(object sender, FontMarginChangedEventArgs e)
		{
		}

		protected virtual void Awake()
		{
			if (this != Instance)
			{
				DestroyImmediate(this);
			}
		}

		public const float TARGET_DPI = 72f;

		public const int MIN_FONT_SIZE = 8;

		public const int MAX_FONT_SIZE = 72;

		private const string DEFAULT_FONT_NAME = "Arial";

		private const int DEFAULT_FONT_SIZE = 16;

		private static T _instance;

		private string _FontName;

		private int _FontSize;

		private Color _FontColor;

		private bool _Bold;

		private bool _Italic;

		private float _MarginLeft;

		private float _MarginTop;

		private float _MarginRight;

		private float _MarginBottom;
	}
}
