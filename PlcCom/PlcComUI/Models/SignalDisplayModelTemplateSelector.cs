using System.Windows;
using System.Windows.Controls;

namespace PlcComUI.Models
{
    public class SignalDisplayModelTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is SignalDisplayModel) || !(container is FrameworkElement containerFrameworkElement))
                return null;

            if (item is BoolSignalDisplayModel boolSignalModel)
            {
                return FindDataTemplate(containerFrameworkElement, "SignalModelBoolTemplate");
            }
            else
            {
                return FindDataTemplate(containerFrameworkElement, "SignalModelNumericTemplate");
            }
        }

        private static DataTemplate FindDataTemplate(FrameworkElement frameworkElement, string key)
        {
            return (DataTemplate)frameworkElement.FindResource(key);
        }
    }
}
