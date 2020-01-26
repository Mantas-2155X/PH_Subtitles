using System;
using System.Collections.Generic;
using System.Text;

namespace UnityEngine.UI.Translation
{
	internal class Subtitle : ISubtitle
	{
		public AudioClip Clip { get; private set; }

		public AudioSource Source { get; private set; }

		public Subtitle(IEnumerable<TextPosition> anchors, AudioSource source)
		{
			this.anchors = anchors;
			content = new Dictionary<TextPosition, StringBuilder>();
			display = new Dictionary<TextPosition, HashSet<SubtitleLine>>();
			foreach (TextPosition key in this.anchors)
			{
				content.Add(key, new StringBuilder(512));
				display.Add(key, new HashSet<SubtitleLine>());
			}
			Source = source;
			Load();
		}

		private void Load()
		{
			if (Source != null && Source.clip != null)
			{
				Clip = Source.clip;
				LoadSubtitles();
				invalid = true;
				loaded = true;
			}
		}

		private void LoadSubtitles()
		{
			if (Clip == null || string.IsNullOrEmpty(Clip.name))
			{
				subtitles = new LineData[0];
				return;
			}
			foreach (TextPosition key in anchors)
			{
				display[key].Clear();
				ClearContent(content[key]);
			}
			SubtitleLine[] array;
			if (SubtitleTranslator.Translate(Clip.name, out array))
			{
				subtitles = new LineData[array.Length];
				for (int i = 0; i < subtitles.Length; i++)
				{
					HashSet<SubtitleLine> anchor;
					if (display.TryGetValue(array[i].Position, out anchor))
					{
						subtitles[i] = new LineData(array[i], anchor);
					}
				}
				return;
			}
			subtitles = new LineData[0];
		}

		private void Unload()
		{
			Clip = null;
			Source = null;
			subtitles = null;
			foreach (TextPosition key in display.Keys)
			{
				display[key].Clear();
				ClearContent(content[key]);
			}
			invalid = false;
			loaded = false;
		}

		private StringBuilder ClearContent(StringBuilder sb)
		{
			if (sb.Length > 0)
			{
				sb.Length = 0;
			}
			return sb;
		}

		public void Reload()
		{
			LoadSubtitles();
		}

		public void LateUpdate()
		{
			try
			{
				if (Source != null && Clip != null && Source.clip == Clip)
				{
					int num = (Clip.frequency == 0) ? 44100 : Clip.frequency;
					float num2 = Source.timeSamples * (1f / num);
					foreach (LineData lineData in subtitles)
					{
						if (lineData != null)
						{
							if (Source.isPlaying && (lineData.Line.EndTime == 0f || num2 < lineData.Line.EndTime) && num2 >= lineData.Line.StartTime)
							{
								invalid |= lineData.Show();
							}
							else
							{
								invalid |= lineData.Hide();
							}
						}
					}
					if (invalid)
					{
						foreach (KeyValuePair<TextPosition, HashSet<SubtitleLine>> keyValuePair in display)
						{
							StringBuilder stringBuilder = ClearContent(content[keyValuePair.Key]);
							foreach (SubtitleLine subtitleLine in keyValuePair.Value)
							{
								if (stringBuilder.Length > 0)
								{
									stringBuilder.Append('\n');
								}
								stringBuilder.Append(subtitleLine.Text);
							}
						}
						invalid = false;
					}
				}
				else if (loaded)
				{
					Unload();
				}
			}
			catch (Exception ex)
			{
				IniSettings.Error("Subtitle::LateUpdate:\n" + ex);
			}
		}

		public string this[TextPosition anchor] => content[anchor].ToString();

		private bool loaded;

		private bool invalid;

		private LineData[] subtitles;

		private IEnumerable<TextPosition> anchors;

		private Dictionary<TextPosition, StringBuilder> content;

		private Dictionary<TextPosition, HashSet<SubtitleLine>> display;

		private class LineData
		{
			public bool Visible { get; private set; }

			public SubtitleLine Line { get; private set; }

			public bool Show()
			{
				bool flag = false;
				if (!Visible)
				{
					flag = anchor.Add(Line);
					Visible = flag;
				}
				return flag;
			}

			public bool Hide()
			{
				bool flag = false;
				if (Visible)
				{
					flag = anchor.Remove(Line);
					Visible = !flag;
				}
				return flag;
			}

			public LineData(SubtitleLine line, HashSet<SubtitleLine> anchor)
			{
				Line = line;
				Visible = false;
				this.anchor = anchor;
			}

			private readonly HashSet<SubtitleLine> anchor;
		}
	}
}
