using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomDolgov.ClassFolder
{
    public static class OpenFileDialogHelper
    {
        /// <summary>
        /// Создает диалоговое окно с фильтром под изображения
        /// </summary>
        /// <returns></returns>
        public static OpenFileDialog GetImageDialog()
        {
            // Configure open file dialog box 
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "";

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;

            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                dlg.Filter = String.Format("{0}{1}{2} ({3})|{3}", dlg.Filter, sep, codecName, c.FilenameExtension);
                sep = "|";
            }

            dlg.Filter = String.Format("{0}{1}{2} ({3})|{3}", dlg.Filter, sep, "All Files", ".");

            dlg.DefaultExt = ".png"; // Default file extension 

            return dlg;
        }
    }
}
