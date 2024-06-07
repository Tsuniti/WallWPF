using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.SignalR.Client;
using WallWPF.Entities;
using WallWPF.Components;

namespace WallWPF.Forms;

public partial class MainForm : Window
{
 PublicationComponent? PreviousPublicationComponent;

        AuthorizationForm? AuthorizationForm;
        public ChatUser? AuthorizedUser;
        public bool IsLightTheme = false;

        

        public MainForm()
        {

            InitializeComponent();
            

            

            MyHubConnection.Connection.On<string, string, DateTime>("Receive", (user, message, date) =>
            {

                Task.Run(() =>
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        ShowMessage(user, message, date);
                    }));
                });
            });


        }
    
        private void ShowMessage(string user, string message, DateTime? date = null)
        {
            bool isScrollBottom =
                Math.Abs(AllPublicationsScrollViewer.VerticalOffset - AllPublicationsScrollViewer.ScrollableHeight) < 1;
            
            var publicationComponent =
                 new PublicationComponent(user, message, date);

            AllPublicationsPanel.Children.Add(publicationComponent);

            if (isScrollBottom)
            {
                AllPublicationsScrollViewer.ScrollToBottom();
            }

        }
        
        
        private async void MainForm_Load(object sender, EventArgs e)
        {

            MyHubConnection.Connection.Reconnecting += error =>
            {
                try
                {
                    Dispatcher.Invoke(() => ShowMessage("System", "Trying to reconnect"));
                }
                catch (Exception ex)
                {
                    ShowMessage("System", $"Reconnecting event error: {ex.Message}");
                }
                return Task.CompletedTask;
            };

            MyHubConnection.Connection.Reconnected += connectionId =>
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        AllPublicationsPanel.Children.Clear();
                        ShowMessage("System", "Connection restored");
                        ShowAllMessages();
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Reconnected event error: {ex.Message}");
                }
                return Task.CompletedTask;
            };

            try
            {
                ShowMessage("System", "Loading...");
                await MyHubConnection.Connection.StartAsync();
                ShowMessage("System", "Connection started.");
                SignUpButton.IsEnabled = true;
                LogInButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ShowMessage("System", ex.Message);
            }
            ShowAllMessages();



        }

        private async void ShowAllMessages()
        {
            try
            {

                foreach (var publication in await MyHubConnection.Connection.InvokeAsync<List<Publication>>("ReceiveAll"))
                {
                    ShowMessage(await MyHubConnection.Connection.InvokeAsync<string>("ReceiveUsernameById", publication.UserId), publication.Message, publication.Date);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("System", ex.Message);
            }

            await Task.Delay(1);
            AllPublicationsScrollViewer.ScrollToBottom();
        }
        
        private void MainForm_Shown(object sender, EventArgs e)
        {
            UsernameLabel.Focus();      
        }
        private async void sendButton_Click(object sender, EventArgs e)
        {
            if (NewMessageTextBox.Text != string.Empty)
            {


                try
                {
                    await MyHubConnection.Connection.InvokeAsync("Send", AuthorizedUser, NewMessageTextBox.Text);
                }
                catch (Exception ex)
                {
                    ShowMessage("System", ex.Message);
                }
                AllPublicationsScrollViewer.ScrollToBottom();
                NewMessageTextBox.Text = String.Empty;
            }
        }

        private void LogInButton_Click(object sender, EventArgs e)
        {
            var authorizationForm = new AuthorizationForm("Log in");
            authorizationForm.ParentForm = this;
            authorizationForm.ShowDialog();
        }

        private void SignUpButton_Click(object sender, EventArgs e)
        {
            AuthorizationForm = new AuthorizationForm("Sign up");
            AuthorizationForm.ParentForm = this;
            AuthorizationForm.ShowDialog();
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            AuthorizedUser = null;
            UsernameLabel.Content = String.Empty;
            SignUpButton.Visibility = Visibility.Visible;
            LogInButton.Visibility = Visibility.Visible;
            LogOutButton.Visibility = Visibility.Collapsed;
            NewMessageTextBox.IsEnabled = false;
            SendButton.IsEnabled = false;
        }
        public void UserLogIn(string username)
        {
            UsernameLabel.Content = username;
            NewMessageTextBox.IsEnabled = true;
            NewMessageTextBox.Focus();
            SendButton.IsEnabled = true;
            LogInButton.Visibility = Visibility.Collapsed;
            SignUpButton.Visibility = Visibility.Collapsed;
            LogOutButton.Visibility = Visibility.Visible;
        }


        private void NewMessageTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {

            
            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    if (NewMessageTextBox.LineCount >= NewMessageTextBox.MaxLines)
                    {
                        e.Handled = true;
                        return;

                    }
                    return;

                }
                sendButton_Click(this, EventArgs.Empty);
            }
        }
       

        private void ToggleTheme(object sender, RoutedEventArgs e)
        {
            ResourceDictionary Theme;

            if (IsLightTheme)
            {
                Theme = new ResourceDictionary() { Source = new Uri("/Themes/Dark.xaml", UriKind.Relative) };
                IsLightTheme = false;
            }
            else
            {
                Theme = new ResourceDictionary() { Source = new Uri("/Themes/Light.xaml", UriKind.Relative) };
                IsLightTheme = true;
            } 
            App.Current.Resources.Clear();
            App.Current.Resources.MergedDictionaries.Add(Theme);
        }

        private async void MainForm_OnClosing(object? sender, CancelEventArgs e)
        {
            await MyHubConnection.Connection.StopAsync();
        }



}