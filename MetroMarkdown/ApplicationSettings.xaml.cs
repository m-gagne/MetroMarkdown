﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MetroMarkdown
{
    public sealed partial class ApplicationSettings : UserControl
    {
        private Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public ApplicationSettings()
        {
            this.InitializeComponent();

            this.DataContext = UserPreferences.Instance;
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
            }

            SettingsPane.Show();
        }

        private void PrettifyCodeToggled(object sender, RoutedEventArgs e)
        {
            UserPreferences.Instance.PrettyPrint = PrettyPrintToggler.IsOn;
        }
    }
}
