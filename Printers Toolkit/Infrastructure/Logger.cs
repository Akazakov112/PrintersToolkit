using System.Collections.ObjectModel;
using System.Windows;

namespace Printers_Toolkit.Infrastructure
{
    /// <summary>
    /// Предоставляет лог выполнения.
    /// </summary>
    class Logger : BaseVm
    {
        private LogMessage selectedLog;

        /// <summary>
        /// Выбранное сообщение.
        /// </summary>
        public LogMessage SelectedLog
        {
            get { return selectedLog; }
            set { selectedLog = value; RaisePropertyChanged(nameof(SelectedLog)); }
        }

        /// <summary>
        /// Список сообщений.
        /// </summary>
        public ObservableCollection<LogMessage> Log { get; private set; }

        /// <summary>
        /// Записать сообщение в лог выполнения.
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="type">Тип, по умочланию оповещение</param>
        public void AddLog(string message)
        {
            var msg = new LogMessage(message);
            Application.Current.Dispatcher.Invoke(() => Log.Add(msg));
            SelectedLog = msg;
        }

        /// <summary>
        /// Очистить список сообщений.
        /// </summary>
        public void ClearLog()
        {
            // Очистить лог.
            Log.Clear();
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        protected Logger()
        {
            Log = new ObservableCollection<LogMessage>();
        }

        /// <summary>
        /// Статический экземпляр класса.
        /// </summary>
        private static readonly Logger instance = new Logger();

        /// <summary>
        /// Возвращает логгер выполнения.
        /// </summary>
        /// <returns>Логгер</returns>
        public static Logger GetLogger()
        {
            return instance;
        }
    }
}
