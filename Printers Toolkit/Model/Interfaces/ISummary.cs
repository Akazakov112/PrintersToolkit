using System;

namespace Printers_Toolkit.Model.Interfaces
{
    /// <summary>
    /// Интерфейс для классов сводки результатов поиска.
    /// </summary>
    interface ISummary : IComparable<ISummary>, IEquatable<ISummary>
    {
        /// <summary>
        /// Параметр.
        /// </summary>
        string Parameter { get; }

        /// <summary>
        /// Количество.
        /// </summary>
        int Count { get; set; }
    }
}
