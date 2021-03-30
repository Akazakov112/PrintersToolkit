using Printers_Toolkit.Infrastructure;
using Printers_Toolkit.Model.Repository;

namespace Printers_Toolkit.ViewModel
{
    /// <summary>
    /// Общий ViewModel.
    /// </summary>
    abstract class MainViewModel : BaseVm
    {
        /// <summary>
        /// Логгер выполнения.
        /// </summary>
        public Logger Logger { get; } = Logger.GetLogger();

        /// <summary>
        /// Репозиторий устройств.
        /// </summary>
        public DeviceRepository DeviceRepository { get; } = DeviceRepository.GetRepository();
    }
}
