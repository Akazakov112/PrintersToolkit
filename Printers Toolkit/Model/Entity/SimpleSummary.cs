using Printers_Toolkit.Infrastructure;
using Printers_Toolkit.Model.Interfaces;

namespace Printers_Toolkit.Model.Entity
{
    /// <summary>
    /// Сводка по одному параметру.
    /// </summary>
    class SimpleSummary : BaseVm, ISummary
    {
        private int count;

        /// <summary>
        /// Параметр.
        /// </summary>
        public string Parameter { get; }

        /// <summary>
        /// Количество.
        /// </summary>
        public int Count
        {
            get { return count; }
            set { count = value; RaisePropertyChanged(nameof(Count)); }
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="parameter">Модель</param>
        /// <param name="count">Количество</param>
        public SimpleSummary(string parameter, int count)
        {
            Parameter = parameter;
            Count = count;
        }


        /// <summary>
        /// Проводит сравнение для сортировки с другим объектом.
        /// </summary>
        /// <param name="other">Сравниваемый объект</param>
        /// <returns></returns>
        public int CompareTo(ISummary other)
        {
            return Parameter.CompareTo(other.Parameter);
        }

        /// <summary>
        /// Проводит сравнение с другим объектом.
        /// </summary>
        /// <param name="other">Сравниваемый объект</param>
        /// <returns></returns>
        public bool Equals(ISummary other)
        {
            return Parameter == other.Parameter;
        }
    }
}
