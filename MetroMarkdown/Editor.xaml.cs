using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSharp;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MetroMarkdown
{
    /// <summary>
    /// The markdown editor page
    /// </summary>
    public sealed partial class Editor : Page
    {
        private Markdown MarkdownConverter = new Markdown();

        public Editor()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var file = new MetroMarkdown.Data.MarkdownFile();
            file.Title = "testing";
            file.Content = "Header\n========";

            this.DataContext = file;
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private async void SaveFile()
        {
            // doto implement async file saving...
        }

        private void ContentTextChanged(object sender, KeyEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            Preview.Text = MarkdownConverter.Transform(ContentText.Text);
        }
    }
}
