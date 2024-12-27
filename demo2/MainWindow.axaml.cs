using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace demo2;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void AdminButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Password.Text == "0000")
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.ShowDialog(this);
        }
        else
        {
            Console.WriteLine("Wrong password");
        }
    }

    private void GuestButton_OnClick(object? sender, RoutedEventArgs e)
    {
        GuestWindow guestWindow = new GuestWindow();
        guestWindow.ShowDialog(this);
    }
}