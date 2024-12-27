using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using demo2.Models;

namespace demo2;

public partial class Appointment : Window
{
    ObservableCollection<Client> clients; 
    ObservableCollection<string> clientsNames;
    private int _serviceId;
    public Appointment(int idService)
    {
        InitializeComponent();
        using var context = new PostgresContext();
        var dataSourceClients = context.Clients.ToList();
        clients = new ObservableCollection<Client>(dataSourceClients);
        clientsNames = new ObservableCollection<string>();
        foreach (var item in clients)
        {
            clientsNames.Add(item.Lastname);
        }
        ClientsBox.ItemsSource = clientsNames;
        _serviceId = idService;
    }


    private void AddAppointment_OnClick(object? sender, RoutedEventArgs e)
    {
        using var context = new PostgresContext();
        var clientsNames2 = ClientsBox.SelectedItem as string;
        Client client = context.Clients.Where(x => x.Lastname == clientsNames2).FirstOrDefault();
        Console.WriteLine(client.Id);
        var data = StartTime.SelectedDate.ToString();
        var dataMinSec = TimePerHours.Text;
        Console.WriteLine(data);
        Console.WriteLine(dataMinSec);
        string resultString = data[6].ToString() + data[7] + data[8] + data[9] + "-" + data[3] + data[4] + "-" + data[0] + data[1] + " " +
                 dataMinSec;
        DateTime result = DateTime.Parse(resultString);
        Clientservice clientServicePresenter = new Clientservice
        {
            Clientid = client.Id,
            Serviceid = _serviceId,
            Starttime = result,
        };
        
        context.Clientservices.Add(clientServicePresenter);
        context.SaveChanges();
        Close(clientServicePresenter);
    }
}