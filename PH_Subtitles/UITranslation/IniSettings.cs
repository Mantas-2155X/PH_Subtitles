using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace UnityEngine.UI.Translation
{
	internal static class IniSettings
	{
		internal static event Action<IniFile> LoadSettings;

		internal static event Action<bool> DebugModeChanged;

		internal static bool DebugMode
		{
			get => debugmode;
			private set
			{
				if (value != debugmode)
				{
					debugmode = value;
					if (DebugModeChanged != null && initialized)
					{
						DebugModeChanged(value);
					}
				}
			}
		}

		internal static event Action<string> LanguageChanged;

		internal static event Action<string> LanguageDirChanged;

		internal static string Language
		{
			get
			{
				if (language == null)
				{
					language = string.Empty;
				}
				return language;
			}
			private set
			{
				char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
				if (value == null)
				{
					value = string.Empty;
				}
				else if (value != string.Empty)
				{
					value = value.Trim();
					if (value != string.Empty)
					{
						if (value.Length > 5)
						{
							value = value.Substring(0, 5);
						}
						if (value.IndexOfAny(invalidFileNameChars) != -1)
						{
							value = string.Empty;
						}
					}
				}
				if (value != language)
				{
					language = value;
					if (LanguageChanged != null && initialized)
					{
						LanguageChanged(value);
					}
					languagedir = value;
					if (!string.IsNullOrEmpty(value))
					{
						languagedir += "\\";
					}
					if (LanguageDirChanged != null && initialized)
					{
						LanguageDirChanged(value);
					}
				}
			}
		}

		internal static string LanguageDir
		{
			get
			{
				if (languagedir == null)
				{
					if (string.IsNullOrEmpty(Language))
					{
						languagedir = string.Empty;
					}
					else
					{
						languagedir = Language + "\\";
					}
				}
				return languagedir;
			}
		}

		internal static event Action<bool> FindImageChanged;

		internal static bool FindImage
		{
			get => findimage;
			private set
			{
				if (value != findimage)
				{
					findimage = value;
					if (FindImageChanged != null && initialized)
					{
						FindImageChanged(value);
					}
				}
			}
		}

		internal static event Action<bool> FindAudioChanged;

		internal static bool FindAudio
		{
			get => findaudio;
			private set
			{
				if (value != findaudio)
				{
					findaudio = value;
					if (FindAudioChanged != null && initialized)
					{
						FindAudioChanged(value);
					}
				}
			}
		}

		internal static event Action<bool> DumpAudioByLevelChanged;

		internal static bool DumpAudioByLevel
		{
			get => dumpaudiobylevel;
			private set
			{
				if (value != dumpaudiobylevel)
				{
					dumpaudiobylevel = value;
					if (DumpAudioByLevelChanged != null && initialized)
					{
						DumpAudioByLevelChanged(value);
					}
				}
			}
		}

		internal static event Action<bool> FindTextChanged;

		internal static bool FindText
		{
			get => findtext;
			private set
			{
				if (value != findtext)
				{
					findtext = value;
					if (FindTextChanged != null && initialized)
					{
						FindTextChanged(value);
					}
				}
			}
		}

		internal static event Action<bool> DumpTextByLevelChanged;

		internal static bool DumpTextByLevel
		{
			get => dumptextbylevel;
			private set
			{
				if (value != dumptextbylevel)
				{
					dumptextbylevel = value;
					if (DumpTextByLevelChanged != null && initialized)
					{
						DumpTextByLevelChanged(value);
					}
				}
			}
		}

		internal static event Action<bool> UseRegExChanged;

		internal static bool UseRegEx
		{
			get => useregex;
			private set
			{
				if (value != useregex)
				{
					useregex = value;
					if (UseRegExChanged != null && initialized)
					{
						UseRegExChanged(value);
					}
				}
			}
		}

		internal static event Action<bool> UseTextPredictionChanged;

		internal static bool UseTextPrediction
		{
			get => usetextprediction;
			private set
			{
				if (value != usetextprediction)
				{
					usetextprediction = value;
					if (UseTextPredictionChanged != null && initialized)
					{
						UseTextPredictionChanged(value);
					}
				}
			}
		}

		internal static event Action<bool> UseCopy2ClipboardChanged;

		internal static bool UseCopy2Clipboard
		{
			get => usecopy2clipboard;
			private set
			{
				if (value != usecopy2clipboard)
				{
					usecopy2clipboard = value;
					if (UseCopy2ClipboardChanged != null && initialized)
					{
						UseCopy2ClipboardChanged(value);
					}
				}
			}
		}

		internal static event Action<int> Copy2ClipboardTimeChanged;

		internal static int Copy2ClipboardTime
		{
			get => copy2clipboardtime;
			private set
			{
				if (value != copy2clipboardtime)
				{
					copy2clipboardtime = value;
					if (Copy2ClipboardTimeChanged != null && initialized)
					{
						Copy2ClipboardTimeChanged(value);
					}
				}
			}
		}

		internal static event Action<string> ProcessPathChanged;

		internal static event Action<string> ProcessPathDirChanged;

		internal static string ProcessPath
		{
			get
			{
				if (processpath == null)
				{
					ProcessPath = processname;
				}
				return processpath;
			}
			private set
			{
				char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
				if (value == null)
				{
					value = processname;
				}
				else if (value != string.Empty)
				{
					value = value.Trim();
					if (value != string.Empty && value.IndexOfAny(invalidFileNameChars) != -1)
					{
						value = processname;
					}
				}
				if (value != processpath)
				{
					processpath = value;
					if (ProcessPathChanged != null && initialized)
					{
						ProcessPathChanged(value);
					}
					processpathdir = value;
					if (!string.IsNullOrEmpty(value))
					{
						processpathdir += "\\";
					}
					if (ProcessPathDirChanged != null && initialized)
					{
						ProcessPathDirChanged(value);
					}
				}
			}
		}

		internal static string ProcessPathDir
		{
			get
			{
				if (processpathdir == null)
				{
					if (string.IsNullOrEmpty(ProcessPath))
					{
						processpathdir = string.Empty;
					}
					else
					{
						processpathdir = ProcessPath + "\\";
					}
				}
				return processpathdir;
			}
		}

		internal static string ProcessName => processname;

		internal static string ProcessFile => processfile;

		internal static string PluginDir => ".\\Plugins\\";

		internal static string MainDir => PluginDir + "UITranslation\\";

		internal static string LogFileDir => MainDir;

		internal static string LogFileName => "Translation.log";

		internal static string LogFilePath => LogFileDir + LogFileName;

		internal static string SettingsFileDir => MainDir;

		internal static string SettingsFileName => "Translation.ini";

		internal static string SettingsFilePath => SettingsFileDir + SettingsFileName;

		internal static int LogWriterTime
		{
			get => writetime;
			set
			{
				if (value < 1)
				{
					value = 1;
				}
				writetime = value;
			}
		}

		static IniSettings()
		{
			processname = Process.GetCurrentProcess().ProcessName;
			processfile = processname + ".exe";
			PROCESSPATHKEY = processname + "_Folder";
			sb = new StringBuilder();
			timer = new Timer(TimeSpan.FromSeconds(LogWriterTime).TotalMilliseconds);
			timer.AutoReset = false;
			timer.Elapsed += timer_Elapsed;
			try
			{
				if (File.Exists(LogFilePath))
				{
					File.Delete(LogFilePath);
				}
			}
			catch (Exception ex)
			{
				Error("IniSettings:\n" + ex);
			}
			Load();
			WatchTextFiles();
		}

		private static void WatchTextFiles()
		{
			try
			{
				if (iniw == null && Directory.Exists(SettingsFileDir))
				{
					iniw = new FileSystemWatcher(SettingsFileDir, SettingsFileName);
					iniw.NotifyFilter = (NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite);
					iniw.IncludeSubdirectories = false;
					iniw.Changed += WatcherNotice;
					iniw.Created += WatcherNotice;
					iniw.Error += delegate(object sender, ErrorEventArgs e)
					{
						Error(e.GetException().ToString());
					};
					iniw.EnableRaisingEvents = true;
				}
			}
			catch (Exception ex)
			{
				Error("WatchTextFiles:\n" + ex);
			}
		}

		private static void WatcherNotice(object sender, FileSystemEventArgs e)
		{
			if (lastraisedfile == e.FullPath && DateTime.Now < lastraisedtime)
			{
				return;
			}
			lastraisedfile = e.FullPath;
			lastraisedtime = DateTime.Now.AddSeconds(1.0);
			Load();
		}

		internal static IniFile GetINIFile()
		{
			return new IniFile(SettingsFilePath);
		}

		internal static void Load()
		{
			object loadLock = LoadLock;
			lock (loadLock)
			{
				try
				{
					if (iniw != null)
					{
						iniw.Dispose();
						iniw = null;
					}
					if (!Directory.Exists(SettingsFileDir))
					{
						Directory.CreateDirectory(SettingsFileDir);
					}
					IniFile inifile = GetINIFile();
					string key = "bDebugMode";
					string value = inifile.GetValue("Translation", key);
					bool flag;
					if (value == null || !bool.TryParse(value, out flag))
					{
						flag = false;
						inifile.WriteValue("Translation", key, flag);
					}
					DebugMode = flag;
					key = "sLanguage";
					value = inifile.GetValue("Translation", key);
					Language = value;
					if (value != Language)
					{
						inifile.WriteValue("Translation", key, Language);
					}
					key = "bFindImage";
					value = inifile.GetValue("Translation", key);
					if (value == null || !bool.TryParse(value, out flag))
					{
						flag = false;
						inifile.WriteValue("Translation", key, flag);
					}
					FindImage = flag;
					key = "bFindAudio";
					value = inifile.GetValue("Translation", key);
					if (value == null || !bool.TryParse(value, out flag))
					{
						flag = false;
						inifile.WriteValue("Translation", key, flag);
					}
					FindAudio = flag;
					key = "bDumpAudioByLevel";
					value = inifile.GetValue("Translation", key);
					if (value == null || !bool.TryParse(value, out flag))
					{
						flag = true;
						inifile.WriteValue("Translation", key, flag);
					}
					DumpAudioByLevel = flag;
					key = "bFindText";
					value = inifile.GetValue("Translation", key);
					if (value == null || !bool.TryParse(value, out flag))
					{
						flag = false;
						inifile.WriteValue("Translation", key, flag);
					}
					FindText = flag;
					key = "bDumpTextByLevel";
					value = inifile.GetValue("Translation", key);
					if (value == null || !bool.TryParse(value, out flag))
					{
						flag = true;
						inifile.WriteValue("Translation", key, flag);
					}
					DumpTextByLevel = flag;
					key = "bUseRegEx";
					value = inifile.GetValue("Translation", key);
					if (value == null || !bool.TryParse(value, out flag))
					{
						flag = true;
						inifile.WriteValue("Translation", key, flag);
					}
					UseRegEx = flag;
					key = "bUseTextPrediction";
					value = inifile.GetValue("Translation", key);
					if (value == null || !bool.TryParse(value, out flag))
					{
						flag = true;
						inifile.WriteValue("Translation", key, flag);
					}
					UseTextPrediction = flag;
					key = "bUseCopy2Clipboard";
					value = inifile.GetValue("Translation", key);
					if (value == null || !bool.TryParse(value, out flag))
					{
						flag = false;
						inifile.WriteValue("Translation", key, flag);
					}
					UseCopy2Clipboard = flag;
					key = "iCopy2ClipboardTime(ms)";
					value = inifile.GetValue("Translation", key);
					int num;
					if (value == null || !int.TryParse(value, out num))
					{
						num = 250;
						inifile.WriteValue("Translation", key, num);
					}
					Copy2ClipboardTime = num;
					key = PROCESSPATHKEY;
					value = inifile.GetValue("Translation", key);
					ProcessPath = value;
					if (value != ProcessPath)
					{
						inifile.WriteValue("Translation", key, ProcessPath);
					}
					initialized = true;
					try
					{
						Action<IniFile> loadSettings = LoadSettings;
						if (loadSettings != null)
						{
							loadSettings(inifile);
						}
					}
					catch (Exception ex)
					{
						Error("LoadSettings:\n" + ex);
					}
					WatchTextFiles();
				}
				catch (Exception ex2)
				{
					Error("LoadSettings:\n" + ex2);
				}
			}
		}

		private static void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			object logLock = LogLock;
			lock (logLock)
			{
				try
				{
					if (!Directory.Exists(LogFileDir))
					{
						Directory.CreateDirectory(LogFileDir);
					}
					using (StreamWriter streamWriter = new StreamWriter(LogFilePath, true, Encoding.UTF8))
					{
						streamWriter.Write(sb.ToString());
						sb.Length = 0;
					}
				}
				catch
				{
				}
			}
		}

		internal static void Log(object obj = null)
		{
			object logLock = LogLock;
			lock (logLock)
			{
				if (obj == null)
				{
					obj = "null";
				}
				sb.AppendLine(obj.ToString());
				timer.Start();
			}
		}

		internal static void Error(object obj = null)
		{
			if (DebugMode)
			{
				Log(obj);
			}
		}

		internal const string DIR1 = ".\\Plugins\\";

		internal const string DIR2 = "UITranslation\\";

		internal const string DIR3 = "Text\\";

		internal const string DIR4 = "Image\\";

		internal const string DIR5 = "Audio\\";

		private const string INI = "Translation.ini";

		private const string LOG = "Translation.log";

		internal const string SECTION = "Translation";

		private const string DEBUGMODEKEY = "bDebugMode";

		private const string FINDTEXTKEY = "bFindText";

		private const string DUMPTEXTBYLEVELKEY = "bDumpTextByLevel";

		private const string FINDAUDIOKEY = "bFindAudio";

		private const string DUMPAUDIOBYLEVELKEY = "bDumpAudioByLevel";

		private const string FINDIMAGEKEY = "bFindImage";

		private const string LANGUAGEKEY = "sLanguage";

		private const string USEREGEXKEY = "bUseRegEx";

		private const string USETEXTPREDICTIONKEY = "bUseTextPrediction";

		private const string USECOPY2CLIPBOARDKEY = "bUseCopy2Clipboard";

		private const string COPY2CLIPBOARDTIMEKEY = "iCopy2ClipboardTime(ms)";

		private static bool debugmode;

		private static string language;

		private static string languagedir;

		private static bool findimage;

		private static bool findaudio;

		private static bool dumpaudiobylevel;

		private static bool findtext;

		private static bool dumptextbylevel;

		private static bool useregex;

		private static bool usetextprediction;

		private static bool usecopy2clipboard;

		private static int copy2clipboardtime;

		private static string processpath;

		private static string processpathdir;

		private static string PROCESSPATHKEY;

		private static string processname;

		private static string processfile;

		private static bool initialized;

		private static string lastraisedfile;

		private static DateTime lastraisedtime;

		private static FileSystemWatcher iniw;

		private static StringBuilder sb;

		private static Timer timer;

		private static int writetime = 3;

		private static readonly object LoadLock = new object();

		private static readonly object LogLock = new object();

		private class NativeMethods
		{
			[DllImport("kernel32.dll", SetLastError = true)]
			internal static extern bool AttachConsole(int dwProcessId);

			[DllImport("kernel32.dll", SetLastError = true)]
			internal static extern bool AllocConsole();

			internal const int ATTACH_PARENT_PROCESS = -1;
		}
	}
}
