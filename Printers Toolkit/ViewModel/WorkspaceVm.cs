using System.Windows.Input;
using Printers_Toolkit.Infrastructure;

namespace Printers_Toolkit.ViewModel
{
    /// <summary>
    /// ViewModel главной рабочей области.
    /// </summary>
    class WorkspaceVm : MainViewModel
    {
        /// <summary>
        /// ViewModel вкладки сканера.
        /// </summary>
        public ScannerVm ScannerVm { get; }

        /// <summary>
        /// ViewModel вкладки просмотра.
        /// </summary>
        public RenameVm RenameVm { get; }

        /// <summary>
        /// ViewModel вкладки отображения обнаруженных устройств по подсетям.
        /// </summary>
        public SubnetsViewTabVm SubnetsViewTabVm { get; }

        /// <summary>
        /// ViewModel вкладки отображения обнаруженных устройств.
        /// </summary>
        public DevicesViewTabVm DevicesViewTabVm { get; }

        /// <summary>
        /// ViewModel вкладки отображения сводки по устройствам.
        /// </summary>
        public SummaryViewTabVm SummaryViewTabVm { get; }

        /// <summary>
        /// Команда очистки лога.
        /// </summary>
        public ICommand ClearLog => new RelayCommand(obj =>
        {
            Logger.ClearLog();
        });

        /// <summary>
        /// Конструктор.
        /// </summary>
        public WorkspaceVm()
        {
            ScannerVm = new ScannerVm();
            RenameVm = new RenameVm();
            SubnetsViewTabVm = new SubnetsViewTabVm();
            DevicesViewTabVm = new DevicesViewTabVm();
            SummaryViewTabVm = new SummaryViewTabVm();
        }
    }
}
