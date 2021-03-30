namespace Printers_Toolkit.Infrastructure
{
    /// <summary>
    /// Типы сообщений.
    /// </summary>
    enum LogMessageType : int
    {
        Notification,
        Warning,
        Error
    }

    /// <summary>
    /// Сообщения для логгирования.
    /// </summary>
    sealed class LogMessage
    {
        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Тип сообщения.
        /// </summary>
        public LogMessageType Type { get; set; }

        /// <summary>
        /// Конструктор с выбором типа сообщения.
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="type">Тип сообщения</param>
        public LogMessage(string message, LogMessageType type = LogMessageType.Notification)
        {
            Message = message;
            Type = type;
        }
    }
}
