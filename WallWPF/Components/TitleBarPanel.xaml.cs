using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace WallWPF.Components;

public partial class TitleBarPanel : UserControl
{
    private Window ParentWindow;
    public TitleBarPanel()
    {
        InitializeComponent();
    }


    private void TitleBarPanel_OnLoaded(object sender, RoutedEventArgs e)
    {
        ParentWindow = Window.GetWindow(this) ?? throw new InvalidOperationException();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        if (ParentWindow.WindowState == WindowState.Maximized)
            ParentWindow.WindowState = WindowState.Normal;

        this.ParentWindow.DragMove();
    }
    private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
    {
        this.ParentWindow.WindowState = WindowState.Minimized;
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        this.ParentWindow.Close();
    }
}