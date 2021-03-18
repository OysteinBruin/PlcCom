using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PlcComUI
{
    public class SignalModelTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is SignalDisplayModel dataSignalModel) || !(container is FrameworkElement containerFrameworkElement))
                return null;

            if (dataSignalModel.DataTypeStr.Contains("Bool"))
            {
                return FindDataTemplate(containerFrameworkElement, "DataSignalModelBoolTemplate");
            }
            else
            {
                return FindDataTemplate(containerFrameworkElement, "DataSignalModelNumericTemplate");
            }
        }

        private static DataTemplate FindDataTemplate(FrameworkElement frameworkElement, string key)
        {
            return (DataTemplate)frameworkElement.FindResource(key);
        }
    }
}
