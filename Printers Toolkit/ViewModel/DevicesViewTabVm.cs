using System;
using System.Windows.Data;
using Printers_Toolkit.Model.Entity;

namespace Printers_Toolkit.ViewModel
{
    /// <summary>
    /// ViewModel вкладки отображения обнаруженных устройств.
    /// </summary>
    class DevicesViewTabVm : MainViewModel
    {
        private ListCollectionView devicesView;
        private string deviceSearch;

        /// <summary>
        /// Список устройств.
        /// </summary>
        public ListCollectionView DevicesView
        {
            get { return devicesView; }
            set { devicesView = value; RaisePropertyChanged(nameof(DevicesView)); }
        }

        /// <summary>
        /// Строка поиска устройств.
        /// </summary>
        public string DeviceSearch
        {
            get { return deviceSearch; }
            set
            {
                deviceSearch = value;
                RaisePropertyChanged(nameof(DeviceSearch));
                DevicesView.Filter = new Predicate<object>(Filtering);
            }
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        public DevicesViewTabVm()
        {
            DevicesView = new ListCollectionView(DeviceRepository.Printers);
            DeviceSearch = string.Empty;
        }


        /// <summary>
        /// Метод фильтрации.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool Filtering(object obj)
        {
            if (obj is Printer item)
            {
                return string.IsNullOrWhiteSpace(DeviceSearch)
                    || item.Ip.ToLower().Contains(DeviceSearch.ToLower())
                    || item.Model.ToLower().Contains(DeviceSearch.ToLower())
                    || item.NetName.ToLower().Contains(DeviceSearch.ToLower())
                    || item.OldNetName.ToLower().Contains(DeviceSearch.ToLower())
                    || item.SerialNumber.ToString().ToLower().Contains(DeviceSearch.ToLower());
            }
            else
            {
                return false;
            }
        }
    }
}
