using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Printers_Toolkit.Model.Interfaces;

namespace Printers_Toolkit.Model
{
    /// <summary>
    /// Статический класс методов расширения.
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// Сортирует наблюдаемую коллекцию по параметру сводки.
        /// </summary>
        /// <typeparam name="T">Класс параметра сводки</typeparam>
        /// <param name="collection">Соритруемая коллекция</param>
        public static void Sort<T>(this ObservableCollection<T> collection) where T : ISummary
        {
            List<T> sorted = collection.OrderByDescending(x => x.Count).ToList();
            int ptr = 0;
            while (ptr < sorted.Count - 1)
            {
                if (!collection[ptr].Equals(sorted[ptr]))
                {
                    int idx = Search(collection, ptr + 1, sorted[ptr]);
                    collection.Move(idx, ptr);
                }
                ptr++;
            }
        }

        /// <summary>
        /// Проводит поиск объекта по коллекции с указанного индекса.
        /// </summary>
        /// <typeparam name="T">Тип коллекции</typeparam>
        /// <param name="collection">Коллекция</param>
        /// <param name="startIndex">Начальный индекс</param>
        /// <param name="other">Объект</param>
        /// <returns></returns>
        private static int Search<T>(ObservableCollection<T> collection, int startIndex, T other)
        {
            for (int i = startIndex; i < collection.Count; i++)
            {
                if (other.Equals(collection[i]))
                    return i;
            }
            return -1;
        }
    }
}
