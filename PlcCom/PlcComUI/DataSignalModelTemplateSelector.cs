using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static PlcComLibrary.Common.Enums;

namespace PlcComUI
{
    public class DataSignalModelTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is SignalDisplayModel dataSignalModel) || !(container is FrameworkElement containerFrameworkElement))
                return null;

            switch (dataSignalModel.DataType)
            {
                case DataType.Bit:
                    return FindDataTemplate(containerFrameworkElement, "DataSignalModelBoolTemplate");
                case DataType.Int:
                case DataType.Real:
                case DataType.Byte:
                case DataType.DInt:
                case DataType.DWord:
                case DataType.Word:
                    return FindDataTemplate(containerFrameworkElement, "DataSignalModelNumericTemplate");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void EditSignal()
        {

        }

        private static DataTemplate FindDataTemplate(FrameworkElement frameworkElement, string key)
        {
            return (DataTemplate)frameworkElement.FindResource(key);
        }
    }
}
