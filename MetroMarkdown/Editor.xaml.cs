using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownSharp;
using MetroMarkdown.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
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
        private Markdown _markdownConverter = new Markdown();
        private int _settingsWidth = 346;
        private Popup _settingsPopup;
        private Rect _windowsBounds;
        private ColumnDefinition _editorColumn;
        private ColumnDefinition _previewColumn;

        public Editor()
        {
            this.InitializeComponent();

            // Record the current window size
            this._windowsBounds = Window.Current.Bounds;

            this._editorColumn = Layout.ColumnDefinitions[0];
            this._previewColumn = Layout.ColumnDefinitions[1];

            // Wire up custom settings
            SettingsPane.GetForCurrentView().CommandsRequested += this.CommandsRequested;

            this.BottomAppBar.Opened += BottomAppBarOpened;
            this.BottomAppBar.Closed += BottomAppBarClosed;

            // Monitor preference changes
            UserPreferences.Instance.PropertyChange += UserPreferencesChange;
        }

        void UserPreferencesChange(object sender, PropertyChangeEventArgs data)
        {
            this.UpdatePreview();
        }

        void BottomAppBarClosed(object sender, object e)
        {
            this.UpdatePreview();
        }

        void BottomAppBarOpened(object sender, object e)
        {
            this.UpdatePreview();
        }

        void CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var command = new SettingsCommand("settings", "Settings", (x) => {
                this._settingsPopup = new Popup();
                this._settingsPopup.Closed += this.SettingsPopupClosed;
                this._settingsPopup.Opened += this.SettingsPopupOpened;
                this._settingsPopup.IsLightDismissEnabled = true;
                this._settingsPopup.Width = this._settingsWidth;
                this._settingsPopup.Height = this._windowsBounds.Height;

                var settingsPane = new ApplicationSettings();

                settingsPane.Width = this._settingsWidth;
                settingsPane.Height = this._windowsBounds.Height;

                this._settingsPopup.Child = settingsPane;
                this._settingsPopup.SetValue(Canvas.LeftProperty, this._windowsBounds.Width - this._settingsWidth);
                this._settingsPopup.SetValue(Canvas.TopProperty, 0);
                this._settingsPopup.IsOpen = true;

                Window.Current.Activated += OnWindowActivated;
            });

            args.Request.ApplicationCommands.Add(command);
        }

        void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            // Hide settings when Window is deactivated
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                this._settingsPopup.IsOpen = false;
            }
        }

        void SettingsPopupOpened(object sender, object e)
        {
            this.UpdatePreview();
        }

        void SettingsPopupClosed(object sender, object e)
        {
            // We remove the event to avoid any reference leaks
            Window.Current.Activated -= OnWindowActivated;

            this.UpdatePreview();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var file = new MarkdownFile();
            file.Title = "testing";
            file.Content = "Header\n========";

            this.DataContext = file;

            this.InitializePreview();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            this.SaveFile();
        }

        private void SaveFile()
        {
            // todo implement async file saving...
        }

        private void ContentTextChanged(object sender, KeyEventArgs e)
        {
            this.UpdatePreview();
        }

        private async void InitializePreview()
        {
            var html = await Windows.Storage.PathIO.ReadTextAsync("ms-appx:///Styles/Default.html");
            Preview.ScriptNotify += PreviewScriptNotify;
            Preview.NavigateToString(html);
        }

        void PreviewScriptNotify(object sender, NotifyEventArgs e)
        {
            switch (e.Value)
            {
                case "loaded":
                    this.UpdatePreview();
                    break;
            }
        }

        void PreviewLoaded(object sender, NavigationEventArgs e)
        { 
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            string content;
            ContentText.Document.GetText(Windows.UI.Text.TextGetOptions.None, out content);
            string body = this._markdownConverter.Transform(content);

            // apply pretty print (if enabled)
            if (UserPreferences.Instance.PrettyPrint)
            {
                body = body.Replace("<pre>", "<pre class='prettyprint'>");
            }

            string[] args = { body };
            Preview.InvokeScript("Update", args);

            bool hidePreview = false;

            if (this._settingsPopup != null && this._settingsPopup.IsOpen)
            {
                hidePreview = true;
            }
            else if (this.BottomAppBar.IsOpen)
            {
                hidePreview = true;
            }

            this.SetPreviewState(hidePreview ? Windows.UI.Xaml.Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible);
        }    

        private void SetPreviewState(Windows.UI.Xaml.Visibility visibility)
        {
            if (visibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                var brush = new WebViewBrush();
                brush.SetSource(Preview);

                PreviewCanvas.Fill = brush;
                Preview.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                Preview.Visibility = Windows.UI.Xaml.Visibility.Visible;
                PreviewCanvas.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);
            }
        }
    }
}
  