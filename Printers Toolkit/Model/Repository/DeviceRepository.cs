using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Printers_Toolkit.Infrastructure;
using Printers_Toolkit.Model.Entity;

namespace Printers_Toolkit.Model.Repository
{
    /// <summary>
    /// Репозиторий устройств.
    /// </summary>
    class DeviceRepository : BaseVm
    {
        /// <summary>
        /// Сводка по моделям.
        /// </summary>
        public ObservableCollection<SimpleSummary> ModelSummaries { get; private set; }

        /// <summary>
        /// Сводка по подсетям.
        /// </summary>
        public ObservableCollection<SimpleSummary> SubnetSummaries { get; private set; }

        /// <summary>
        /// Список подсетей с найденными устройствами.
        /// </summary>
        public ObservableCollection<Subnet> Subnets { get; private set; }

        /// <summary>
        /// Список найденных принтеров.
        /// </summary>
        public ObservableCollection<Printer> Printers { get; private set; }

        /// <summary>
        /// Количество подсетей.
        /// </summary>
        public int SubnetCount
        {
            get { return Subnets.Where(x => x.Printers.Count > 0).Count(); }
            private set { RaisePropertyChanged(nameof(SubnetCount)); }
        }

        /// <summary>
        /// Количество обнаруженных устройств.
        /// </summary>
        public int DeviceCount
        {
            get { return Printers.Count; }
            private set { RaisePropertyChanged(nameof(DeviceCount)); }
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        protected DeviceRepository()
        {
            Subnets = new ObservableCollection<Subnet>();
            Printers = new ObservableCollection<Printer>();
            ModelSummaries = new ObservableCollection<SimpleSummary>();
            SubnetSummaries = new ObservableCollection<SimpleSummary>();
        }


        /// <summary>
        /// Добавляет подсеть в список подсетей репозитория.
        /// </summary>
        /// <param name="subnet">Подсеть</param>
        public void AddSubnet(Subnet subnet)
        {
            if (subnet != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Subnets.Add(subnet);
                    if (subnet.Printers.Count > 0)
                    {
                        SimpleSummary summary = SubnetSummaries.FirstOrDefault(x => x.Parameter == subnet.Address);
                        if (summary is null)
                        {
                            var newSummary = new SimpleSummary(subnet.Address, subnet.Printers.Count);
                            SubnetSummaries.Add(newSummary);
                        }
                        else
                        {
                            summary.Count += subnet.Printers.Count;
                        }
                        SubnetSummaries.Sort();
                    }
                });
                SubnetCount = Subnets.Where(x => x.Printers.Count > 0).Count();
            }
        }

        /// <summary>
        /// Добавляет принтер в общий список устройств репозитория.
        /// </summary>
        /// <param name="printer">Принтер</param>
        public void AddPrinter(Printer printer)
        {
            if (printer != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Printers.Add(printer);
                    SimpleSummary summary = ModelSummaries.FirstOrDefault(x => x.Parameter == printer.Model);
                    if (summary is null)
                    {
                        var newSummary = new SimpleSummary(printer.Model, 1);
                        ModelSummaries.Add(newSummary);
                    }
                    else
                    {
                        summary.Count++;
                    }
                    ModelSummaries.Sort();
                });
                DeviceCount++;
            }
        }

        /// <summary>
        /// Очищает репозиторий.
        /// </summary>
        public void Clear()
        {
            Subnets.ToList().ForEach(x => x.Printers.Clear());
            Subnets.Clear();
            Printers.Clear();
            ModelSummaries.Clear();
            SubnetSummaries.Clear();
            DeviceCount = 0;
            SubnetCount = 0;
        }

        /// <summary>
        /// Статический экземпляр класса.
        /// </summary>
        private static readonly DeviceRepository instance = new DeviceRepository();

        /// <summary>
        /// Возвращает репозиторий подсетей.
        /// </summary>
        /// <returns>Репозиторий подсетей</returns>
        public static DeviceRepository GetRepository()
        {
            return instance;
        }
    }
}
