﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Printers_Toolkit.Infrastructure
{
    /// <summary>
    /// Реализация INPC.
    /// </summary>
    public abstract class BaseVm : INotifyPropertyChanged
    {
        /// <summary>
        /// Событие изменения свойства.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Реализация INPC.
        /// </summary>
        /// <param name="prop"></param>
        public void RaisePropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
