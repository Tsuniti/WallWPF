using System.Windows.Controls;
using WallWPF.Entities;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using Microsoft.AspNetCore.SignalR.Client;

namespace WallWPF.Forms;

public partial class AuthorizationForm : Window
{

    public MainForm ParentForm;

    public AuthorizationForm()
    {
        InitializeComponent();

        SubmitButton.Click += LogIn;
        ChangeFormButton.Click += SignUpPaint;
    }
    public AuthorizationForm(string str)
    {
        InitializeComponent();
        
        SubmitButton.Click += LogIn;
        ChangeFormButton.Click += SignUpPaint;
        
        if (str == "Log in") LogInPaint(null, new RoutedEventArgs());

        else if (str == "Sign up") SignUpPaint(null, new RoutedEventArgs());

    }

    private void LogIn(object sender, RoutedEventArgs e)
    {
        ErrorLabel.Foreground = Brushes.Red;
        bool isFieldsEmpty = false;
        if (UsernameTextBox.Text == string.Empty)
        {
            UsernameTextBox.BorderBrush = Brushes.Red;

            isFieldsEmpty = true;
        }
        if (PasswordTextBox.Password == string.Empty)
        {
            PasswordTextBox.BorderBrush = Brushes.Red;
            isFieldsEmpty = true;

        }
        if (isFieldsEmpty)
        {
            ErrorLabel.Content = "Fields can't be empty";
            return;
        }
        

        
        TryToLogIn();

    }

    private async void TryToLogIn()
    {
        try
        {
            ChatUser newUser =
                await MyHubConnection.Connection.InvokeAsync<ChatUser>("LogIn", UsernameTextBox.Text, PasswordTextBox.Password);

            if (newUser is null)
            {
                ErrorLabel.Content = "Username or password does not match";
                UsernameTextBox.BorderBrush = Brushes.Red;
                PasswordTextBox.BorderBrush = Brushes.Red;
                return;
            }

            ParentForm.AuthorizedUser = newUser;


                Window.GetWindow(this).Close();
                ParentForm.UserLogIn(newUser.Username);

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }


    private async void SignUp(object sender, RoutedEventArgs e)
    {
        ErrorLabel.BorderBrush = Brushes.Red;

        bool IsFieldsEmpty = false;
        if (UsernameTextBox.Text == string.Empty)
        {
            UsernameTextBox.BorderBrush = Brushes.Red;
            IsFieldsEmpty = true;
        }
        if (PasswordTextBox.Password == string.Empty)
        {
            PasswordTextBox.BorderBrush = Brushes.Red;
            IsFieldsEmpty = true;

        }
        if (PasswordRepeatTextBox.Password == string.Empty)
        {
            PasswordRepeatTextBox.BorderBrush = Brushes.Red;
            IsFieldsEmpty = true;

        }
        if (IsFieldsEmpty)
        {
            ErrorLabel.Content = "Fields can't be empty";
            return;
        }
        if (UsernameTextBox.Text.Length <= 3)
        {
            ErrorLabel.Content = "Username must be longer than 2 characters";
            return;
        }

            try
            {
                if (await MyHubConnection.Connection.InvokeAsync<bool>("CheckIsUsernameAvailable", UsernameTextBox.Text))
                {
                    ErrorLabel.Content = "This username already taken";
                    UsernameTextBox.BorderBrush = Brushes.Red;
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        
        if (PasswordTextBox.Password.Length <= 4)
        {
            ErrorLabel.Content = "Password must be longer than 3 characters";
            PasswordTextBox.BorderBrush = Brushes.Red;

            return;
        }
        if (PasswordTextBox.Password != PasswordRepeatTextBox.Password)
        {
            ErrorLabel.Content = "Password mismatch";
            PasswordTextBox.BorderBrush = Brushes.Red;
            PasswordRepeatTextBox.BorderBrush = Brushes.Red;
            return;
        }

        try
        {
            await MyHubConnection.Connection.InvokeAsync("RegisterNewUser", UsernameTextBox.Text, PasswordTextBox.Password);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        
        TryToLogIn();



    }

    private void LogInPaint(object? sender, RoutedEventArgs e)
    {
        HidePasswordRepeatTextBox(sender == null ? 0 : 0.3);

        UsernameTextBox.SetResourceReference(Control.BorderBrushProperty, "Border");
        PasswordTextBox.SetResourceReference(Control.BorderBrushProperty, "Border");
        PasswordRepeatTextBox.SetResourceReference(Control.BorderBrushProperty, "Border");
        ErrorLabel.Content = String.Empty;

        WelcomeLabel.Content = "Welcome back!";
        SuggestionLabel.Content = "Log in to your existing account";

        SubmitButton.Click -= SignUp;
        SubmitButton.Click += LogIn;
        SubmitButton.Content = "LOG IN";

        ChangeFormButton.Click -= LogInPaint;
        ChangeFormButton.Click += SignUpPaint;
        ChangeFormButton.Content = "Create account";
        
        if (PasswordRepeatTextBox.IsFocused)
        {
            PasswordTextBox.Focus();
        }

    }


    private void SignUpPaint(object? sender, RoutedEventArgs e)
    {
        UsernameTextBox.SetResourceReference(Control.BorderBrushProperty, "Border");
        PasswordTextBox.SetResourceReference(Control.BorderBrushProperty, "Border");
        PasswordRepeatTextBox.SetResourceReference(Control.BorderBrushProperty, "Border");
        ErrorLabel.Content = String.Empty;
        WelcomeLabel.Content = "Welcome!";
        SuggestionLabel.Content = "Create a new account";

        ShowPasswordRepeatTextBox(sender == null ? 0 : 0.3);
        SubmitButton.Click -= LogIn;
        SubmitButton.Click += SignUp;
        SubmitButton.Content = "SIGN UP";


        ChangeFormButton.Click -= SignUpPaint;
        ChangeFormButton.Click += LogInPaint;
        ChangeFormButton.Content = "Log in";


    }

    private async void ShowPasswordRepeatTextBox(double delay)
    {
        var opacityAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(delay)
        };
        var marginAnimation = new ThicknessAnimation
        {
            From = new Thickness(0, -58, 0, 0),
            To = new Thickness(0, 20, 0, 0),
            Duration = TimeSpan.FromSeconds(delay)
        };

        PasswordRepeatTextBox.BeginAnimation(System.Windows.Controls.Control.MarginProperty, marginAnimation);

        PasswordRepeatTextBox.BeginAnimation(System.Windows.Controls.Control.OpacityProperty, opacityAnimation);


        await Task.Delay(TimeSpan.FromSeconds(delay));
        PasswordRepeatTextBox.IsEnabled = true;

    }
    private async void HidePasswordRepeatTextBox(double delay)
    {
        PasswordRepeatTextBox.IsEnabled = false;

        DoubleAnimation opacityAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromSeconds(delay)
        };
        var marginAnimation = new ThicknessAnimation
        {
            From = new Thickness(0, 20, 0, 0),
            To = new Thickness(0, -58, 0, 0),
            Duration = TimeSpan.FromSeconds(delay)
        };


        PasswordRepeatTextBox.BeginAnimation(System.Windows.Controls.Control.MarginProperty, marginAnimation);


        PasswordRepeatTextBox.BeginAnimation(System.Windows.Controls.Control.OpacityProperty, opacityAnimation);

        await Task.Delay(TimeSpan.FromSeconds(delay));

        PasswordRepeatTextBox.Password = String.Empty;


    }

    private void ChangeTextBoxesBorderColorToDefault()
    {
        if (UsernameTextBox.BorderBrush == (Brush)App.Current.FindResource("Border") &&
            PasswordTextBox.BorderBrush == (Brush)App.Current.FindResource("Border") &&
            PasswordRepeatTextBox.BorderBrush == (Brush)App.Current.FindResource("Border"))
        {
            ErrorLabel.Content = String.Empty;
        }
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
            TextBox textBox = sender as TextBox;
            if (textBox.BorderBrush == Brushes.Red)
            {
                textBox.SetResourceReference(Control.BorderBrushProperty, "Border");
            }


        ChangeTextBoxesBorderColorToDefault();
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        PasswordBox passwordBox = sender as PasswordBox;
        if (passwordBox.BorderBrush == Brushes.Red)
        {
            passwordBox.SetResourceReference(Control.BorderBrushProperty, "Border");
        }
        ChangeTextBoxesBorderColorToDefault();
    }

    private void UsernameTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            PasswordTextBox.Focus();
        }
    }

    private void PasswordTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            if (PasswordRepeatTextBox.IsEnabled)
            {
                PasswordRepeatTextBox.Focus();
            }
            else
            {
                LogIn(this, new RoutedEventArgs());
            }
        }
            
    }

    private void PasswordRepeatTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            SignUp(this, new RoutedEventArgs());
        }
    }

    private void AuthorizationForm_OnLoaded(object sender, RoutedEventArgs e)
    {
        UsernameTextBox.Focus();
    }
}