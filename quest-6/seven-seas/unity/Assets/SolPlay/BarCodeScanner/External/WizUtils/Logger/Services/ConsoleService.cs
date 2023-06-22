using System;

namespace Wizcorp.Utils.Logger.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class ConsoleService : ILogService
	{
		private readonly LogLevel minLevel;

		public ConsoleService(LogLevel level = LogLevel.Info)
		{
			minLevel = level;
		}

		public void AddLog(LogLevel level, object message, object data = null)
		{
			if (level < minLevel)
			{
				return;
			}

			switch (level)
			{
				case LogLevel.Verbose:
					UnityEngine.Debug.Log(Format(message, data));
					UnityEngine.Debug.Log(Environment.StackTrace);
					break;
				case LogLevel.Info:
				case LogLevel.Debug:
					UnityEngine.Debug.Log(Format(message, data));
					break;
				case LogLevel.Error:
				case LogLevel.Critical:
					UnityEngine.Debug.LogError(Format(message, data));
					break;
				case LogLevel.Warning:
					UnityEngine.Debug.LogWarning(Format(message, data));
					break;
			}
		}

		private string Format(object message, object data = null)
		{
			if (data == null)
			{
				return LogObject(message);
			}

			return string.Format("{0}\n{1}", LogObject(message), LogObject(data));
		}

		private string LogObject(object data)
		{
			// Append Stacktrace to any exception
			Exception exception = data as Exception;
			if (exception != null && !string.IsNullOrEmpty(exception.StackTrace))
			{
				return string.Format("{0} : {1}", exception, exception.StackTrace);
			}

			return data.ToString();
		}
	}
}
