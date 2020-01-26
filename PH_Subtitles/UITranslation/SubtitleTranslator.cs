using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

namespace UnityEngine.UI.Translation
{
	internal static class SubtitleTranslator
	{
		internal static bool Initialized { get; private set; }

		internal static string GlobalSubtitleDir => IniSettings.MainDir + IniSettings.LanguageDir + "Audio\\";

		internal static string GlobalSubtitleDirFiles => "*" + ".txt";

		internal static string SubtitleDir => IniSettings.MainDir + IniSettings.ProcessPathDir + IniSettings.LanguageDir + "Audio\\";

		internal static string SubtitleDirFiles => "*" + ".txt";

		internal static string FileDir => SubtitleDir;

		internal static string FileName => string.Format("{0}.txt", "Subtitle");

		internal static string FilePath => Path.Combine(FileDir, FileName);

		internal static string LvFileDir => SubtitleDir;

		internal static string LvFileName
		{
			get
			{
				string text = Application.loadedLevelName;
				if (string.IsNullOrEmpty(text))
				{
					text = "Subtitle";
				}
				return string.Format("{0}.{1}.txt", text, Application.loadedLevel);
			}
		}

		internal static string LvFilePath => Path.Combine(LvFileDir, LvFileName);

		static SubtitleTranslator()
		{
			Initialize();
		}

		internal static void Initialize()
		{
			try
			{
				if (!Initialized)
				{
					double totalMilliseconds = TimeSpan.FromSeconds(IniSettings.LogWriterTime).TotalMilliseconds;
					writerdata = new Dictionary<string, StringBuilder>();
					writertimer = new Timer(totalMilliseconds);
					writertimer.AutoReset = false;
					writertimer.Elapsed += WriterTimerElapsed;
					Load();
					SubtitleSettings.AnchorChanged += delegate
					{
						Load();
						AudioSourceSubtitle.Instance.Reload();
					};
					IniSettings.LanguageDirChanged += delegate
					{
						Load();
						AudioSourceSubtitle.Instance.Reload();
					};
					IniSettings.ProcessPathDirChanged += delegate
					{
						Load();
						AudioSourceSubtitle.Instance.Reload();
					};
					IniSettings.FindAudioChanged += delegate
					{
						AudioSourceSubtitle.Instance.Reload();
					};
					Initialized = true;
				}
			}
			catch (Exception ex)
			{
				Initialized = false;
				IniSettings.Error("SubtitleTranslator::Initialize:\n" + ex);
			}
		}

		private static void WriterTimerElapsed(object sender, ElapsedEventArgs e)
		{
			object writerLock = WriterLock;
			lock (writerLock)
			{
				StopWatchSubtitleFiles();
				try
				{
					foreach (KeyValuePair<string, StringBuilder> keyValuePair in writerdata)
					{
						string key = keyValuePair.Key;
						string directoryName = Path.GetDirectoryName(key);
						if (!Directory.Exists(directoryName))
						{
							Directory.CreateDirectory(directoryName);
						}
						using (StreamWriter streamWriter = new StreamWriter(key, true, Encoding.UTF8))
						{
							streamWriter.Write(keyValuePair.Value.ToString());
						}
					}
				}
				catch (Exception ex)
				{
					IniSettings.Error("SubtitleTranslator::DumpText:\n" + ex);
				}
				writerdata.Clear();
				WatchSubtitleFiles();
			}
		}

		private static void StopWatchSubtitleFiles()
		{
			if (gfsw != null)
			{
				gfsw.Dispose();
				gfsw = null;
			}
			if (sfsw != null)
			{
				sfsw.Dispose();
				sfsw = null;
			}
		}

		private static void WatchSubtitleFiles()
		{
			try
			{
				if (GlobalSubtitleDir != SubtitleDir && gfsw == null && Directory.Exists(GlobalSubtitleDir))
				{
					gfsw = new FileSystemWatcher(GlobalSubtitleDir, GlobalSubtitleDirFiles);
					gfsw.NotifyFilter = (NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite);
					gfsw.IncludeSubdirectories = false;
					gfsw.Changed += WatcherNotice;
					gfsw.Created += WatcherNotice;
					gfsw.Error += delegate(object sender, ErrorEventArgs e)
					{
						IniSettings.Error(e.GetException().ToString());
					};
					gfsw.EnableRaisingEvents = true;
				}
				if (sfsw == null && Directory.Exists(SubtitleDir))
				{
					sfsw = new FileSystemWatcher(SubtitleDir, SubtitleDirFiles);
					sfsw.NotifyFilter = (NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite);
					sfsw.IncludeSubdirectories = false;
					sfsw.Changed += WatcherNotice;
					sfsw.Created += WatcherNotice;
					sfsw.Error += delegate(object sender, ErrorEventArgs e)
					{
						IniSettings.Error(e.GetException().ToString());
					};
					sfsw.EnableRaisingEvents = true;
				}
			}
			catch (Exception ex)
			{
				IniSettings.Error("WatchSubtitleFiles:\n" + ex);
			}
		}

		private static void WatcherNotice(object sender, FileSystemEventArgs e)
		{
			object noticeLock = NoticeLock;
			lock (noticeLock)
			{
				if (!(lastraisedfile == e.FullPath) || !(DateTime.Now < lastraisedtime))
				{
					lastraisedfile = e.FullPath;
					lastraisedtime = DateTime.Now.AddSeconds(1.0);
					if (e.FullPath.EndsWith(".txt"))
					{
						LoadTranslations(e.FullPath, true);
					}
					WatchSubtitleFiles();
				}
			}
		}

		private static void Load()
		{
			StopWatchSubtitleFiles();
			translations.Clear();
			translationsLv.Clear();
			if (GlobalSubtitleDir != SubtitleDir)
			{
				LoadAllFromGlobalTranslationDir();
			}
			LoadAllFromTranslationDir();
			WatchSubtitleFiles();
			if (IniSettings.DebugMode || IniSettings.FindAudio)
			{
				int num = 0;
				num += translations.Count;
				foreach (OrderedDictionary orderedDictionary in translationsLv.Values)
				{
					num += orderedDictionary.Count;
				}
				IniSettings.Log(string.Format("Subtitles Loaded: {0}", num));
			}
		}

		private static void LoadTranslations(string file, bool retranslate = false)
		{
			object translationLock = TranslationLock;
			lock (translationLock)
			{
				try
				{
					if (!(Path.GetExtension(file).ToLower() != ".txt"))
					{
						if (!Path.GetFileName(file).StartsWith("."))
						{
							if (file.StartsWith(Environment.CurrentDirectory))
							{
								file = file.Remove(0, Environment.CurrentDirectory.Length);
								if (!file.StartsWith("\\"))
								{
									file = "\\" + file;
								}
								file = "." + file;
							}
							int fileLevel = GetFileLevel(file);
							bool flag = fileLevel > -1;
							OrderedDictionary orderedDictionary = null;
							if (flag)
							{
								translationsLv.TryGetValue(fileLevel, out orderedDictionary);
								if (orderedDictionary != null)
								{
									RemoveAllTranslation(orderedDictionary, file);
								}
							}
							else
							{
								RemoveAllTranslation(translations, file);
							}
							using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
							{
								bool flag2 = false;
								bool flag3 = true;
								List<SubtitleLine> list = null;
								string text = string.Empty;
								while (!streamReader.EndOfStream)
								{
									string text2 = streamReader.ReadLine();
									if (!text2.StartsWith("//") && (text.Length != 0 || text2.Length != 0))
									{
										Match match = Regex.Match(text2.TrimEnd(), "^#sub[ ]+\"(.+?)\"(?:[ ]+(?:{\\\\a([\\d]+)})?(.*))?$", RegexOptions.IgnoreCase);
										if (match.Success)
										{
											flag2 = false;
											flag3 = true;
											SubtitleDataBase subtitleDataBase = null;
											text = match.Groups[1].Value;
											list = new List<SubtitleLine>();
											if (match.Groups[3].Success)
											{
												string text3 = match.Groups[3].Value.Trim();
												if (text3.Length > 0)
												{
													SubtitleLine item = default;
													item.Position = (match.Groups[2].Success ? item.Position.Parse(match.Groups[2].Value, SubtitleSettings.Anchor) : SubtitleSettings.Anchor);
													item.Text = text3;
													list.Add(item);
												}
											}
											if (flag)
											{
												if (orderedDictionary != null)
												{
													subtitleDataBase = (orderedDictionary[text] as SubtitleDataBase);
												}
											}
											else
											{
												subtitleDataBase = (translations[text] as SubtitleDataBase);
											}
											if (subtitleDataBase != null)
											{
												if (flag)
												{
													orderedDictionary.Remove(text);
												}
												else
												{
													translations.Remove(text);
												}
											}
											subtitleDataBase = new SubtitleDataBase(file, list);
											if (flag)
											{
												if (orderedDictionary == null)
												{
													orderedDictionary = new OrderedDictionary();
													translationsLv.Add(fileLevel, orderedDictionary);
												}
												orderedDictionary.Add(text, subtitleDataBase);
											}
											else
											{
												translations.Add(text, subtitleDataBase);
											}
										}
										else if (text.Length > 0)
										{
											if (!flag2)
											{
												if (text2.Length == 0)
												{
													continue;
												}
												Match match2 = Regex.Match(text2, "^([\\d]*\\.?[\\d]+)[ ]+-->[ ]+([\\d]*\\.?[\\d]+)$", RegexOptions.None);
												if (match2.Success)
												{
													if (!streamReader.EndOfStream)
													{
														flag2 = true;
														list.Add(new SubtitleLine
														{
															StartTime = float.Parse(match2.Groups[1].Value, CultureInfo.InvariantCulture),
															EndTime = float.Parse(match2.Groups[2].Value, CultureInfo.InvariantCulture)
														});
													}
													continue;
												}

												flag2 = true;
												list.Add(default);
											}
											if (flag3)
											{
												int index = list.Count - 1;
												if (text2.Length > 0)
												{
													Match match3 = Regex.Match(text2, "^(?:{\\\\a([\\d]+)})?(.*)$", RegexOptions.None);
													if (match3.Success)
													{
														string text4 = match3.Groups[2].Value.Trim();
														if (text4.Length != 0 || !streamReader.EndOfStream)
														{
															SubtitleLine value = list[index];
															value.Position = (match3.Groups[1].Success ? value.Position.Parse(match3.Groups[1].Value, SubtitleSettings.Anchor) : SubtitleSettings.Anchor);
															value.Text = text4;
															list[index] = value;
															continue;
														}
													}
												}
												flag2 = false;
												flag3 = true;
												list.RemoveAt(index);
											}
											else if (text2.Length > 0)
											{
												string text5 = text2.Trim();
												int index2 = list.Count - 1;
												SubtitleLine value2 = list[index2];
												if (text5.Length > 0)
												{
													if (value2.Text.Length > 0)
													{
														value2.Text += "\n";
													}
													value2.Text += text5;
													list[index2] = value2;
												}
												else if (streamReader.EndOfStream && value2.Text.Length == 0)
												{
													list.RemoveAt(index2);
												}
											}
											else
											{
												flag2 = false;
												flag3 = true;
												int index3 = list.Count - 1;
												if (list[index3].Text.Length == 0)
												{
													list.RemoveAt(index3);
												}
											}
										}
									}
								}
							}
							if (retranslate)
							{
								AudioSourceSubtitle.Instance.Reload();
							}
							if (IniSettings.DebugMode || IniSettings.FindAudio)
							{
								IniSettings.Log("Loaded: " + file);
							}
						}
					}
				}
				catch (Exception ex)
				{
					IniSettings.Error("LoadSubtitles:\n" + ex);
				}
			}
		}

		private static void LoadAllFromGlobalTranslationDir()
		{
			if (!Directory.Exists(GlobalSubtitleDir))
			{
				return;
			}
			LoadAllFromTranslationDir(Directory.GetFiles(GlobalSubtitleDir, GlobalSubtitleDirFiles));
		}

		private static void LoadAllFromTranslationDir()
		{
			if (!Directory.Exists(SubtitleDir))
			{
				return;
			}
			LoadAllFromTranslationDir(Directory.GetFiles(SubtitleDir, SubtitleDirFiles));
		}

		private static void LoadAllFromTranslationDir(string[] files)
		{
			if (files == null || files.Length == 0)
			{
				return;
			}
			for (int i = 0; i < files.Length; i++)
			{
				LoadTranslations(files[i]);
			}
		}

		private static int GetFileLevel(string file)
		{
			file = Path.GetFileName(file);
			if (!file.StartsWith("."))
			{
				string text = null;
				if (file.EndsWith(".txt"))
				{
					text = file.Substring(0, file.Length - ".txt".Length);
				}
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = Path.GetExtension(text);
					if (text2.StartsWith("."))
					{
						text2 = text2.Remove(0, 1);
						int num;
						if (int.TryParse(text2, out num) && num > -1)
						{
							return num;
						}
					}
				}
			}
			return -1;
		}

		private static void RemoveAllTranslation(OrderedDictionary od, string fromfile)
		{
			for (int i = od.Count - 1; i >= 0; i--)
			{
				SubtitleDataBase subtitleDataBase = od[i] as SubtitleDataBase;
				if (subtitleDataBase != null && subtitleDataBase.Path == fromfile)
				{
					od.RemoveAt(i);
				}
			}
		}

		public static bool Translate(string audio, out SubtitleLine[] lines)
		{
			object translationLock = TranslationLock;
			bool result;
			lock (translationLock)
			{
				try
				{
					List<SubtitleLine> list = null;
					OrderedDictionary orderedDictionary;
					if (translationsLv.TryGetValue(Application.loadedLevel, out orderedDictionary))
					{
						SubtitleDataBase subtitleDataBase = orderedDictionary[audio] as SubtitleDataBase;
						if (subtitleDataBase != null)
						{
							list = subtitleDataBase.Value;
							goto IL_5B;
						}
					}
					SubtitleDataBase subtitleDataBase2 = translations[audio] as SubtitleDataBase;
					if (subtitleDataBase2 != null)
					{
						list = subtitleDataBase2.Value;
					}
					IL_5B:
					if (list == null)
					{
						list = new List<SubtitleLine>();
						if (IniSettings.FindAudio)
						{
							if (IniSettings.DumpAudioByLevel)
							{
								OrderedDictionary orderedDictionary2;
								if (!translationsLv.TryGetValue(Application.loadedLevel, out orderedDictionary2))
								{
									orderedDictionary2 = new OrderedDictionary();
									translationsLv.Add(Application.loadedLevel, orderedDictionary2);
								}
								string lvFilePath = LvFilePath;
								orderedDictionary2.Add(audio, new SubtitleDataBase(lvFilePath, list));
								DumpSubtitle(lvFilePath, audio);
							}
							else
							{
								string filePath = FilePath;
								translations.Add(audio, new SubtitleDataBase(filePath, list));
								DumpSubtitle(filePath, audio);
							}
						}
					}
					List<SubtitleLine> list2 = new List<SubtitleLine>();
					foreach (SubtitleLine item in list)
					{
						if (!string.IsNullOrEmpty(item.Text))
						{
							list2.Add(item);
						}
					}
					if (list2.Count == 0 && IniSettings.FindAudio)
					{
						list2.Add(new SubtitleLine
						{
							Position = SubtitleSettings.Anchor,
							Text = audio
						});
					}
					lines = list2.ToArray();
					return list2.Count > 0;
				}
				catch (Exception ex)
				{
					IniSettings.Error("TextTranslator::Translate:\n" + ex);
				}
				lines = null;
				result = false;
			}
			return result;
		}

		private static void DumpSubtitle(string path, string audio)
		{
			object writerLock = WriterLock;
			lock (writerLock)
			{
				if (!(Path.GetDirectoryName(path) + "\\" != SubtitleDir))
				{
					StringBuilder stringBuilder;
					if (!writerdata.TryGetValue(path, out stringBuilder))
					{
						stringBuilder = new StringBuilder();
						writerdata.Add(path, stringBuilder);
					}
					stringBuilder.AppendLine(string.Format("#sub \"{0}\"", audio));
					writertimer.Start();
				}
			}
		}

		private const string EXT = ".txt";

		private const string FILENAME = "Subtitle";

		private const string FILE = "{0}.txt";

		private const string LVFILE = "{0}.{1}.txt";

		private const string IGNORE = ".";

		private const string COMMENT = "//";

		private static readonly object TranslationLock = new object();

		private static Dictionary<int, OrderedDictionary> translationsLv = new Dictionary<int, OrderedDictionary>();

		private static OrderedDictionary translations = new OrderedDictionary();

		private static readonly object NoticeLock = new object();

		private static string lastraisedfile;

		private static DateTime lastraisedtime;

		private static FileSystemWatcher gfsw;

		private static FileSystemWatcher sfsw;

		private static readonly object WriterLock = new object();

		private static Dictionary<string, StringBuilder> writerdata;

		private static Timer writertimer;
	}
}
