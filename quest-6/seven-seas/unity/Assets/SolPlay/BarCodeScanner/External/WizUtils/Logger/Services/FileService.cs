using System;
using System.IO;

namespace Wizcorp.Utils.Logger.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class FileService : ILogService
	{
		private readonly LogLevel minLevel;
		private readonly string pathLog;

		public FileService(string path, LogLevel level = LogLevel.Info, bool overwrite = false)
		{
			minLevel = level;

			try
			{
				if (overwrite)
				{
					File.Delete(path);
				}
				pathLog = path;
			}
			catch (Exception e)
			{
				Log.Warning("Can't init FileService", e);
			}
		}

		public void AddLog(LogLevel level, object message, object data = null)
		{
			if (level < minLevel)
			{
				return;
			}

			if (string.IsNullOrEmpty(pathLog))
			{
				return;
			}

			using (StreamWriter sw = File.AppendText(pathLog))
			{
				sw.WriteLine(Format(level, message, data));
			}
		}

		private string Format(LogLevel level, object message, object data = null)
		{
			string time = string.Format("{0:T}", DateTime.Now);
			if (data == null)
			{
				return string.Format("{0} - {1} - {2}", time, level, message);
			}

			return string.Format("{0} - {1} - {2} - {3}", time, level, message, data);
		}
	}
}
