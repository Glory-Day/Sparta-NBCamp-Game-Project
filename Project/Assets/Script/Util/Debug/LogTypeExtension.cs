using System;

namespace Backend.Util.Debug
{
    public static class LogTypeExtension
    {
        /// <summary>
        /// Get the name of <see cref="LogType"/> in all upper case
        /// </summary>
        public static string GetName(this LogType type)
        {
            return Enum.GetName(typeof(LogType), type)?.ToUpper();
        }
    }
}
