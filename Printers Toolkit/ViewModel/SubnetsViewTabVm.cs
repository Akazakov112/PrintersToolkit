using System;
using System.Linq;
using System.Windows.Data;
using Printers_Toolkit.Model.Entity;

namespace Printers_Toolkit.ViewModel
{
    /// <summary>
    /// ViewModel вкладки отображения обнаруженных устройств по подсетям.
    /// </summary>
    class SubnetsViewTabVm : MainViewModel
    {
        private ListCollectionView subnetsView;
        private string subnetSearch;
        private bool isEmptyHidden;

        /// <summary>
        /// Список подсетей.
        /// </summary>
        public ListCollectionView SubnetsView
        {
            get { return subnetsView; }
            set { subnetsView = value; RaisePropertyChanged(nameof(SubnetsView)); }
        }

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
        /// Пустые подсети скрыты.
        /// </summary>
        public bool IsEmptyHidden
        {
            get { return isEmptyHidden; }
            set
            {
                isEmptyHidden = value;
                RaisePropertyChanged(nameof(IsEmptyHidden));
                SubnetsView.Filter = new Predicate<object>(Filtering);
            }

        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        public SubnetsViewTabVm()
        {
            SubnetsView = new ListCollectionView(DeviceRepository.Subnets);
            SubnetSearch = string.Empty;
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
                if (IsEmptyHidden)
                {
                    return item.Printers.Any(x =>
                    x.NetName.ToLower().Contains(SubnetSearch.ToLower())
                    || x.Model.ToLower().Contains(SubnetSearch.ToLower())
                    ) && item.Printers.Any();
                }
                else
                {
                    return string.IsNullOrWhiteSpace(SubnetSearch) || item.Printers.Any(x =>
                    x.NetName.ToLower().Contains(SubnetSearch.ToLower())
                    || x.Model.ToLower().Contains(SubnetSearch.ToLower())
                    );
                }
            }
            else
            {
                return false;
            }
        }
    }
}
