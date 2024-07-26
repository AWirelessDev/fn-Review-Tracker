using Microsoft.Extensions.Logging;

namespace fn_Review_Tracker {
    public class ExceptionLogger {
        public static void LogException(string methodName, Exception ex, ILogger log) {
            log.LogError($"'{ex.GetType()}' while executing {methodName}   | Exception Message: {ex.Message}");
        }

        public static void LogException(string methodName, Exception ex, ILogger log, List<string> details) {

            if (!(details?.Any() ?? false)) {
                LogException(methodName, ex, log);
                return;
            }

            log.LogError($"'{ex.GetType()}' while executing {methodName} for ! | {string.Join(" | ", details)} | Exception Message: {ex.Message}");
        }
    }
}
