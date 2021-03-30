using System.Windows.Data;

namespace Printers_Toolkit.ViewModel
{
    /// <summary>
    /// ViewModel вкладки отображения сводки по устройствам.
    /// </summary>
    class SummaryViewTabVm : MainViewModel
    {
        private ListCollectionView modelsSumView;
        private ListCollectionView subnetsSumView;

        /// <summary>
        /// Сводка по моделям.
        /// </summary>
        public ListCollectionView ModelsSumView
        {
            get { return modelsSumView; }
            set { modelsSumView = value; RaisePropertyChanged(nameof(ModelsSumView)); }
        }

        /// <summary>
        /// Сводка по подсетям.
        /// </summary>
        public ListCollectionView SubnetsSumView
        {
            get { return subnetsSumView; }
            set { subnetsSumView = value; RaisePropertyChanged(nameof(SubnetsSumView)); }
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        public SummaryViewTabVm()
        {
            ModelsSumView = new ListCollectionView(DeviceRepository.ModelSummaries);
            SubnetsSumView = new ListCollectionView(DeviceRepository.SubnetSummaries);
        }
    }
}
