using System.Collections.Generic;

using Wizcorp.Utils.Logger.Service;

namespace Wizcorp.Utils.Logger
{
	/// <summary>
	/// Wizcorp Logger
	/// </summary>
	public sealed class Log
	{
		private static readonly Log Instance = new Log();

		private Log()
		{
			services = new List<ILogService>();

			#if UNITY_EDITOR
			// Console log activate by default in Unity Editor
			services.Add(new ConsoleService());
			#endif
		}

		#region Services

		private readonly List<ILogService> services;

		public static IList<ILogService> Services
		{
			get { return Instance.services; }
		}

		public static void AddService(ILogService service)
		{
			Instance.services.Add(service);
		}

		public static void RemoveService(ILogService service)
		{
			Instance.services.Remove(service);
		}

		public static void ClearServices()
		{
			Instance.services.Clear();
		}

		#endregion

		#region Log

		private void AddLog(LogLevel level, object message, object data = null)
		{
			foreach (var service in services)
			{
				service.AddLog(level, message, data);
			}
		}

		public static void Verbose(object message, object data = null)
		{
			Instance.AddLog(LogLevel.Verbose, message, data);
		}

		public static void Info(object message, object data = null)
		{
			Instance.AddLog(LogLevel.Info, message, data);
		}

		public static void Debug(object message, object data = null)
		{
			Instance.AddLog(LogLevel.Debug, message, data);
		}

		public static void Warning(object message, object data = null)
		{
			Instance.AddLog(LogLevel.Warning, message, data);
		}

		public static void Error(object message, object data = null)
		{
			Instance.AddLog(LogLevel.Error, message, data);
		}

		public static void Critical(object message, object data = null)
		{
			Instance.AddLog(LogLevel.Critical, message, data);
		}

		#endregion

	}
}
