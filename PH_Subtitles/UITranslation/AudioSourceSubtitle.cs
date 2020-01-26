using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace UnityEngine.UI.Translation
{
	public class AudioSourceSubtitle : MonoBehaviour
	{
		public static AudioSourceSubtitle Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = FindObjectOfType<AudioSourceSubtitle>();
					if (_Instance == null)
					{
						GameObject gameObject = new GameObject("AudioSourceSubtitle");
						_Instance = gameObject.AddComponent<AudioSourceSubtitle>();
						DontDestroyOnLoad(gameObject);
					}
				}
				return _Instance;
			}
		}

		static AudioSourceSubtitle()
		{
			SubtitleSettings.FontNameChanged += delegate(string value)
			{
				SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.FontName = value;
			};
			SubtitleSettings.FontSizeChanged += delegate(int value)
			{
				SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.FontSize = value;
			};
			SubtitleSettings.FontColorChanged += delegate(Color value)
			{
				SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.FontColor = value;
			};
			SubtitleSettings.BoldChanged += delegate(bool value)
			{
				SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.Bold = value;
			};
			SubtitleSettings.ItalicChanged += delegate(bool value)
			{
				SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.Italic = value;
			};
			SubtitleSettings.BorderWidthChanged += delegate(int value)
			{
				SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.BorderWidth = value;
			};
			SubtitleSettings.ShadowOffsetChanged += delegate(int value)
			{
				SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.ShadowOffset = value;
			};
			SubtitleSettings.MarginLeftChanged += delegate(int value)
			{
				SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.MarginLeft = value;
			};
			SubtitleSettings.MarginTopChanged += delegate(int value)
			{
				SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.MarginTop = value;
			};
			SubtitleSettings.MarginRightChanged += delegate(int value)
			{
				SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.MarginRight = value;
			};
			SubtitleSettings.MarginBottomChanged += delegate(int value)
			{
				SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.MarginBottom = value;
			};
		}

		public AudioSourceSubtitle()
		{
			subtitles = new OrderedDictionary();
			content = new Dictionary<TextPosition, StringBuilder>();
		}

		public void Reload()
		{
			if (subtitles.Count > 0)
			{
				reloadsubtitles = true;
			}
		}

		public void Add(AudioSource source)
		{
			try
			{
				subtitles.Remove(source);
				subtitles.Insert(0, source, new Subtitle(SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.Anchors, source));
			}
			catch (Exception ex)
			{
				IniSettings.Error("AudioSourceSubtitle::Load:\n" + ex);
			}
		}

		private void Awake()
		{
			if (this != Instance)
			{
				DestroyImmediate(this);
			}
			foreach (TextPosition key in SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.Anchors)
			{
				content.Add(key, new StringBuilder(512));
			}
			SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.FontName = SubtitleSettings.FontName;
			SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.FontSize = SubtitleSettings.FontSize;
			SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.FontColor = SubtitleSettings.FontColor;
			SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.Bold = SubtitleSettings.Bold;
			SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.Italic = SubtitleSettings.Italic;
			SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.BorderWidth = SubtitleSettings.BorderWidth;
			SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.ShadowOffset = SubtitleSettings.ShadowOffset;
			SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.MarginLeft = SubtitleSettings.MarginLeft;
			SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.MarginTop = SubtitleSettings.MarginTop;
			SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.MarginRight = SubtitleSettings.MarginRight;
			SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.MarginBottom = SubtitleSettings.MarginBottom;
		}

		private void LateUpdate()
		{
			try
			{
				if (subtitles.Count != 0)
				{
					foreach (KeyValuePair<TextPosition, StringBuilder> keyValuePair in content)
					{
						if (keyValuePair.Value.Length > 0)
						{
							keyValuePair.Value.Length = 0;
						}
					}
					for (int i = subtitles.Count - 1; i >= 0; i--)
					{
						Subtitle subtitle = subtitles[i] as Subtitle;
						if (subtitle == null)
						{
							subtitles.RemoveAt(i);
						}
						else if (subtitle.Source == null)
						{
							subtitles.RemoveAt(i);
						}
						else
						{
							if (reloadsubtitles)
							{
								subtitle.Reload();
							}
							subtitle.LateUpdate();
							foreach (TextPosition textPosition in SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.Anchors)
							{
								string text = subtitle[textPosition];
								if (text.Length > 0)
								{
									if (content[textPosition].Length > 0)
									{
										content[textPosition].Append('\n');
									}
									content[textPosition].Append(text);
								}
							}
						}
					}
					reloadsubtitles = false;
					foreach (TextPosition textPosition2 in SubtitleUserInterfaceBase<SubtitleCanvas>.Instance.Anchors)
					{
						SubtitleUserInterfaceBase<SubtitleCanvas>.Instance[textPosition2] = content[textPosition2].ToString();
					}
				}
			}
			catch (Exception ex)
			{
				IniSettings.Error("AudioSourceSubtitle::LateUpdate:\n" + ex);
			}
		}

		private static AudioSourceSubtitle _Instance;

		private bool reloadsubtitles;

		private OrderedDictionary subtitles;

		private Dictionary<TextPosition, StringBuilder> content;
	}
}
