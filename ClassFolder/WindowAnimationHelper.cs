using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;

namespace DiplomDolgov.ClassFolder
{
    public static class WindowAnimationHelper
    {
        public static void CloseWindowWithFadeOut(Window window)
        {
            // Находим анимацию затухания по ключу
            var fadeOutAnimation = window.FindResource("WindowFadeOut") as Storyboard;

            // Если анимация найдена, запускаем ее
            if (fadeOutAnimation != null)
            {
                fadeOutAnimation.Completed += (sender, e) =>
                {
                    // По завершении анимации закрываем окно
                    window.Close();
                };

                fadeOutAnimation.Begin(window);
            }
            else
            {
                // Если анимация не найдена, просто закрываем окно
                window.Close();
            }
        }
    }
}
