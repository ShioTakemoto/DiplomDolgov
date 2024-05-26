using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace DiplomDolgov.ClassFolder
{
    internal class ElementsToolsClass
    {
        public static void ClearAllControls(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is TextBox)
                {
                    ((TextBox)child).Clear();
                }
                else if (child is ComboBox)
                {
                    ((ComboBox)child).SelectedIndex = -1;
                }
                else if (child is PasswordBox)
                {
                    ((PasswordBox)child).Clear();
                }
                ClearAllControls(child); //рекурсивный вызов для очистки дочерних элементов
            }
        }

        public static bool AllFieldsFilled(DependencyObject parent)
        {
            foreach (object child in LogicalTreeHelper.GetChildren(parent))
            {
                if (child is TextBox textBox)
                {
                    // проверка заполненности TextBox
                    if (textBox.Tag != null && textBox.Tag.ToString() == "Optional")
                    {
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        return false;
                    }
                }
                else if (child is ComboBox comboBox)
                {
                    // проверка заполненности ComboBox
                    if (comboBox.Tag != null && comboBox.Tag.ToString() == "Optional")
                    {
                        continue;
                    }
                    if (comboBox.SelectedItem == null)
                    {
                        return false;
                    }
                }
                else if (child is DatePicker datePicker)
                {
                    // проверка заполненности DatePicker
                    if (datePicker.Tag != null && datePicker.Tag.ToString() == "Optional")
                    {
                        continue;
                    }
                    if (datePicker.SelectedDate == null)
                    {
                        return false;
                    }
                }
                else if (child is TimePicker timePicker)
                {
                    // проверка заполненности TimePicker
                    if (timePicker.Tag != null && timePicker.Tag.ToString() == "Optional")
                    {
                        continue;
                    }
                    if (timePicker.SelectedTime == null)
                    {
                        return false;
                    }
                }
                else if (child is DependencyObject dependencyObject)
                {
                    if (!AllFieldsFilled(dependencyObject))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
