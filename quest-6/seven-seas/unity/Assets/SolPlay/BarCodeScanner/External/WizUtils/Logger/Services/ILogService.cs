namespace Wizcorp.Utils.Logger.Service
{
	public interface ILogService
	{
		void AddLog(LogLevel level, object message, object data = null);
	}
}
