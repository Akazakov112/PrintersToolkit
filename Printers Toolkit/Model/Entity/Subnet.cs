using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Windows;
using Printers_Toolkit.Infrastructure;

namespace Printers_Toolkit.Model.Entity
{
    /// <summary>
    /// Подсеть.
    /// </summary>
    class Subnet : BaseVm, IValidatableObject
    {
        private bool isSelected;

        /// <summary>
        /// Адрес подсети.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Префикс.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// ОСП.
        /// </summary>
        public string Osp { get; set; }

        /// <summary>
        /// Адрес расположения.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Метка выбора подсети.
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; RaisePropertyChanged(nameof(IsSelected)); }
        }

        /// <summary>
        /// Список принтеров подсети.
        /// </summary>
        public ObservableCollection<Printer> Printers { get; set; }

        /// <summary>
        /// Список адресов подсети.
        /// </summary>
        public List<DhcpClient> Clients { get; set; }

        /// <summary>
        /// Конструктор без параметров.
        /// </summary>
        public Subnet()
        {
            Address = string.Empty;
            Prefix = string.Empty;
            Osp = string.Empty;
            Location = string.Empty;
            Printers = new ObservableCollection<Printer>();
        }

        /// <summary>
        /// Конструктор с параметрами.
        /// </summary>
        /// <param name="address">Адрес</param>
        /// <param name="prefix">Префикс</param>
        /// <param name="osp">Название ОСП</param>
        /// <param name="location">Местоположение ОСП</param>
        public Subnet(string address, string prefix, string osp, string location)
        {
            Address = address;
            Prefix = prefix;
            Osp = osp;
            Location = location;
            Printers = new ObservableCollection<Printer>();
            Clients = new List<DhcpClient>();
        }


        /// <summary>
        /// Проводит валидацию подсети.
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string subnetPattern = @"^(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[0-9]{2}|[0-9])(\.(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[0-9]{2}|[0-9])){2}.[0]{1}$";

            List<ValidationResult> errors = new List<ValidationResult>();

            // Проверка на пустое значение адреса.
            if (string.IsNullOrWhiteSpace(Prefix))
                errors.Add(new ValidationResult("Введите префикс подсети."));

            // Проверка на пустое значение адреса.
            if (string.IsNullOrWhiteSpace(Address))
                errors.Add(new ValidationResult("Введите адрес подсети."));

            // Проверка на корректность введенного адреса.
            if (!Regex.IsMatch(Address, subnetPattern))
                errors.Add(new ValidationResult("Введен некорректный адрес подсети."));

            return errors;
        }

        /// <summary>
        /// Добавляет новый принтер в список подсети.
        /// </summary>
        /// <param name="newPrinter">Новый принтер</param>
        public void AddPrinter(Printer newPrinter)
        {
            if (newPrinter != null)
                Application.Current.Dispatcher.Invoke(() => Printers.Add(newPrinter));
        }
    }
}
