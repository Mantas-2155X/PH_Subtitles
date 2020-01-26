using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
	public class IniFile
	{
		public IniFile(string path = null)
		{
			name = Process.GetCurrentProcess().ProcessName;
			if (string.IsNullOrEmpty(path))
			{
				this.path = name + ".ini";
			}
			this.path = Path.GetFullPath(path);
			Load();
		}

		private void Load()
		{
			ini = new Dictionary<string, Dictionary<string, string>>();
			ini.Add(name, new Dictionary<string, string>());
			if (!File.Exists(path))
			{
				return;
			}
			try
			{
				using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
				{
					string text = name;
					while (!streamReader.EndOfStream)
					{
						string text2 = streamReader.ReadLine().Trim();
						if (text2.Length != 0)
						{
							if (text2.StartsWith("[") && text2.EndsWith("]"))
							{
								text = text2.Substring(1, text2.Length - 2);
								if (text.Length == 0)
								{
									text = name;
								}
								if (!ini.ContainsKey(text))
								{
									ini.Add(text, new Dictionary<string, string>());
								}
							}
							else if (!text2.StartsWith(";"))
							{
								string[] array = text2.Split(new[]
								{
									'='
								}, 2);
								array[0] = array[0].Trim();
								Dictionary<string, string> dictionary;
								if (!string.IsNullOrEmpty(array[0]) && ini.TryGetValue(text, out dictionary))
								{
									dictionary.Remove(array[0]);
									if (array.Length == 2)
									{
										string[] array2 = array[1].Split(new[]
										{
											';'
										}, 2);
										array2[0] = array2[0].Trim();
										dictionary.Add(array[0], array2[0]);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public string[] GetSections()
		{
			string[] array = new string[ini.Keys.Count];
			ini.Keys.CopyTo(array, 0);
			return array;
		}

		public string[] GetKeysInSection(string section = null)
		{
			if (string.IsNullOrEmpty(section))
			{
				section = name;
			}
			Dictionary<string, string> dictionary;
			if (ini.TryGetValue(section, out dictionary))
			{
				string[] array = new string[dictionary.Keys.Count];
				dictionary.Keys.CopyTo(array, 0);
				return array;
			}
			return new string[0];
		}

		public string GetValue(string section = null, string key = null, object @default = null)
		{
			if (string.IsNullOrEmpty(section))
			{
				section = name;
			}
			if (string.IsNullOrEmpty(key))
			{
				key = name;
			}
			Dictionary<string, string> dictionary;
			string result;
			if (ini.TryGetValue(section, out dictionary) && dictionary.TryGetValue(key, out result))
			{
				return result;
			}
			if (@default != null)
			{
				return @default.ToString();
			}
			return null;
		}

		public void WriteValue(string section = null, string key = null, object value = null)
		{
			if (string.IsNullOrEmpty(section))
			{
				section = name;
			}
			if (string.IsNullOrEmpty(key))
			{
				key = name;
			}
			string value2 = (value == null) ? string.Empty : value.ToString();
			if (!NativeMethods.WritePrivateProfileString(section, key, value2, path))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			Dictionary<string, string> dictionary;
			if (ini.TryGetValue(section, out dictionary))
			{
				dictionary.Remove(key);
				dictionary.Add(key, value2);
				return;
			}
			dictionary = new Dictionary<string, string>();
			dictionary.Add(key, value2);
			ini.Add(section, dictionary);
		}

		private readonly string path;

		private readonly string name;

		private Dictionary<string, Dictionary<string, string>> ini;

		private class NativeMethods
		{
			[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern bool WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

			[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int GetPrivateProfileString(string Section, string Key, string Default, string Result, int Size, string FilePath);
		}
	}
}
