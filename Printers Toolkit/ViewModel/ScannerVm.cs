using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Printers_Toolkit.Infrastructure;
using Printers_Toolkit.Model;
using Printers_Toolkit.Model.Entity;
using Printers_Toolkit.View;

namespace Printers_Toolkit.ViewModel
{
    /// <summary>
    /// ViewModel вкладки сканера.
    /// </summary>
    class ScannerVm : MainViewModel
    {
        private string subnetSearch;
        private ListCollectionView subnetsView;
        private Subnet newSubnet;
        private SnmpScanner scanner;

        /// <summary>
        /// Строка поиска подсети.
        /// </summary>
        public string SubnetSearch
        {
            get { return subnetSearch; }
            set
            {
                subnetSearch = value;
                RaisePropertyChanged(nameof(SubnetSearch));
                SubnetsView.Filter = new Predicate<object>(Filtering);
            }
        }

        /// <summary>
        /// Список подсетей.
        /// </summary>
        public ListCollectionView SubnetsView
        {
            get { return subnetsView; }
            set { subnetsView = value; RaisePropertyChanged(nameof(SubnetsView)); }
        }

        /// <summary>
        /// Исходный список подсетей для представления.
        /// </summary>
        private List<Subnet> SourceSubnetList
        {
            get { return (List<Subnet>)SubnetsView?.SourceCollection; }
        }

        /// <summary>
        /// Добавляемая подсеть.
        /// </summary>
        public Subnet NewSubnet
        {
            get { return newSubnet; }
            private set { newSubnet = value; RaisePropertyChanged(nameof(NewSubnet)); }
        }

        /// <summary>
        /// Сканер подсетей SNMP.
        /// </summary>
        public SnmpScanner Scanner
        {
            get { return scanner; }
            set { scanner = value; RaisePropertyChanged(nameof(Scanner)); }
        }

        /// <summary>
        /// Команда начала сканирования.
        /// </summary>
        public ICommand StartScan => new RelayCommand(async obj =>
        {
            if (DeviceRepository.Subnets.Any())
            {
                MessageBoxResult result = Alert.Show("Результаты текущего сканирования будут удалены.\nПродолжить?", "Сканирование", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    DeviceRepository.Clear();
                    Scanner = new SnmpScanner(SourceSubnetList.Where(x => x.IsSelected));
                    await Scanner.StartAsync();
                }
            }
            else
            {
                Scanner = new SnmpScanner(SourceSubnetList.Where(x => x.IsSelected));
                await Scanner.StartAsync();
            }
        }
        , (canEx) => SourceSubnetList != null && SourceSubnetList.Any(x => x.IsSelected));

        /// <summary>
        /// Команда остановки сканирования.
        /// </summary>
        public ICommand StopScan => new RelayCommand(obj =>
        {
            Scanner.Stop();
        }
        // Доступна, когда запущено сканирование.
        , (canEx) => Scanner != null && !Scanner.Stopped);

        /// <summary>
        /// Команда очистки списка устройств.
        /// </summary>
        public ICommand ClearRepository => new RelayCommand(obj =>
        {
            MessageBoxResult result = Alert.Show("Результаты текущего сканирования будут удалены.\nПродолжить?", "Сканирование", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DeviceRepository.Clear();
            }
        });

        /// <summary>
        /// Команда выбора всех подсетей в списке.
        /// </summary>
        public ICommand CheckAll => new RelayCommand(obj =>
        {
            foreach (Subnet subnet in SourceSubnetList)
            {
                subnet.IsSelected = true;
            }
        });

        /// <summary>
        /// Команда выбора отображаемых подсетей в списке.
        /// </summary>
        public ICommand CheckSelected => new RelayCommand(obj =>
        {
            foreach (Subnet subnet in SubnetsView)
            {
                subnet.IsSelected = true;
            }
        });

        /// <summary>
        /// Команда снятия отметок со всех подсетей в списке.
        /// </summary>
        public ICommand UnCheckAll => new RelayCommand(obj =>
        {
            foreach (Subnet subnet in SourceSubnetList)
            {
                subnet.IsSelected = false;
            }
        });

        /// <summary>
        /// Команда снятия отметок с отображаемых подсетей в списке.
        /// </summary>
        public ICommand UnCheckSelected => new RelayCommand(obj =>
        {
            foreach (Subnet subnet in SubnetsView)
            {
                subnet.IsSelected = false;
            }
        });

        /// <summary>
        /// Команда добавления подсети в список.
        /// </summary>
        public ICommand AddSubnet => new RelayCommand(obj =>
        {
            // Валидация и добавление в список подсетей.
            List<ValidationResult> results = NewSubnet.Validate(new ValidationContext(NewSubnet)).ToList();
            if (results.Any())
            {
                foreach (var error in results)
                {
                    Alert.Show(error.ErrorMessage, "Ошибка!", MessageBoxButton.OK);
                }
            }
            else
            {
                if (SourceSubnetList.Any(x => x.Address == NewSubnet.Address))
                {
                    Alert.Show("Подсеть уже есть в списке.", "Добавление подсети", MessageBoxButton.OK);
                }
                else
                {
                    SourceSubnetList.Insert(0, NewSubnet);
                    SubnetsView.Refresh();
                    Logger.AddLog("Подсеть добавлена");
                    NewSubnet = new Subnet();
                }
            }
        });

        /// <summary>
        /// Команда удаления подсети из списка.
        /// </summary>
        public ICommand DeleteCheckedSubnets => new RelayCommand(obj =>
        {
            SourceSubnetList.RemoveAll(x => x.IsSelected);
            SubnetsView.Refresh();
        }
        , (canEx) => SourceSubnetList != null && SourceSubnetList.Any(x => x.IsSelected));

        /// <summary>
        /// Команда обновления списка подсетей.
        /// </summary>
        public ICommand UpdateSubnets => new RelayCommand(obj =>
        {
            InitSubnetsList();
        });

        /// <summary>
        /// Конструктор.
        /// </summary>
        public ScannerVm()
        {
            InitSubnetsList();
            NewSubnet = new Subnet();
        }


        /// <summary>
        /// Инициализация параметров сканирования.
        /// </summary>
        private async void InitSubnetsList()
        {
            Logger.AddLog("Инициализация списка подсетей.");
            try
            {
                string connectionStr = Properties.Settings.Default.DbConnectionString;
                var subnetBuilder = new SqlSubnetListBuilder(connectionStr);
                var subnets = await subnetBuilder.GetSubnetsAsync();
                SubnetsView = new ListCollectionView(subnets);
                Logger.AddLog($"Список подсетей получен.");
            }
            catch (Exception ex)
            {
                string error = ex.InnerException is null ? ex.Message : ex.InnerException.Message;
                Logger.AddLog($"Список подсетей не получен по причине: {error}.");
            }
        }

        /// <summary>
        /// Метод фильтрации.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool Filtering(object obj)
        {
            if (obj is Subnet item)
            {
                return item.Address.ToLower().Contains(SubnetSearch.ToLower())
                    || item.Osp.ToLower().Contains(SubnetSearch.ToLower())
                    || item.Location.ToLower().Contains(SubnetSearch.ToLower())
                    || item.Prefix.ToLower().Contains(SubnetSearch.ToLower());
            }
            else
            {
                return false;
            }
        }
    }
}
