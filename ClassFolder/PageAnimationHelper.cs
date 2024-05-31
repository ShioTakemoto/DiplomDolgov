using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace DiplomDolgov.ClassFolder
{
    public static class PageAnimationHelper
    {
        public static async Task ClosePageWithFadeOut(Page page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));

            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(fadeOutAnimation);
            Storyboard.SetTarget(fadeOutAnimation, page);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));

            await Task.Delay(1000); // Добавляем небольшую задержку перед анимацией для плавного эффекта
            await Task.Run(() => Application.Current.Dispatcher.Invoke(() => storyboard.Begin(page)));

            // Ждем завершения анимации
            await Task.Delay(1000);
        }
    }
}
