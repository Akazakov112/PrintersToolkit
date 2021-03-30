using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Printers_Toolkit.Infrastructure;

namespace Printers_Toolkit.Model.Entity
{
    /// <summary>
    /// Принтер.
    /// </summary>
    class Printer : BaseVm, IValidatableObject
    {
        /// <summary>
        /// Подсеть нахождения принтера.
        /// </summary>
        public Subnet Subnet { get; set; }

        /// <summary>
        /// IP адрес.
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Модель.
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// Серийный номер.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Сетевое имя.
        /// </summary>
        public string NetName { get; }

        /// <summary>
        /// Старое сетевое имя.
        /// </summary>
        public string OldNetName { get; }

        /// <summary>
        /// Возможность переименовать принтер.
        /// </summary>
        public bool CanRenamed { get; set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="subnet">Подсеть</param>
        /// <param name="ip">IP адрес</param>
        /// <param name="model">Модель</param>
        /// <param name="serialNumber">Серийный номер</param>
        /// <param name="netname">Сетевое имя</param>
        /// <param name="canRenamed">Возможность переименовать принтер</param>
        public Printer(Subnet subnet, string ip, string model, string serialNumber, string netname, bool canRenamed = true)
        {
            Subnet = subnet;
            Ip = ip;
            Model = model;
            SerialNumber = serialNumber;
            NetName = netname;
            OldNetName = netname;
            CanRenamed = canRenamed;
        }


        /// <summary>
        /// Проводит валидацию принтера.
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(NetName)
                || string.IsNullOrWhiteSpace(Model)
                || NetName.ToLower().Contains("null")
                || Model.ToLower().Contains("null"))
                errors.Add(new ValidationResult("Объект не является принтером."));

            return errors;
        }
    }
}
