using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TextRecognition.FileTasks;

namespace TextRecognition
{
    class FileTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TiffFileTemplate { get; set; }

        public DataTemplate UnsupportedTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ImageOcrTask)
            {
                return TiffFileTemplate;
            }

            return UnsupportedTemplate;
        }
    }
}
