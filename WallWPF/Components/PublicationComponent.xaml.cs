using System.Windows.Controls;
using System.Windows.Input;

namespace WallWPF.Components;

public partial class PublicationComponent : UserControl
{
    public PublicationComponent()
    {
        InitializeComponent();
    }
    
    public PublicationComponent(string username,  string message, DateTime? time = null)
    {
        InitializeComponent();
        UsernameLabel.Content = username + ':';
            MessageTextBox.Text = message;
        TimeLabel.Content = time == null ? DateTime.Now.ToString() : time.ToString();
    }
    
}