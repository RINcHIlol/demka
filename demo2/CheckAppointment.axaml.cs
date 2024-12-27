using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using demo2.Models;

namespace demo2;

public partial class CheckAppointment : Window
{
    ObservableCollection<ClientservicePresenter> clientServices = new ObservableCollection<ClientservicePresenter>();
    List<ClientservicePresenter> dataSourceSericeClient;
    List<Client> dataSourceClient;
    List<Service> dataSourceService;
    public CheckAppointment()
    {
        InitializeComponent();
        using var context = new PostgresContext();
        dataSourceClient = context.Clients.ToList();
        dataSourceService = context.Services.ToList();
        DateTime today = DateTime.Today;
        DateTime tomorrow = today.AddDays(1);
        
        dataSourceSericeClient = context.Clientservices.AsEnumerable().Where(clientService => clientService.Starttime.Date == today || clientService.Starttime.Date == tomorrow).Select(clientService => new ClientservicePresenter
        {
            Clientid = clientService.Clientid,
            ClientName = dataSourceClient.FirstOrDefault(c => c.Id == clientService.Clientid).Lastname, // Получаем имя клиента по ID
            Serviceid = clientService.Serviceid,
            ServiceName = dataSourceService.FirstOrDefault(s => s.Id == clientService.Serviceid).Title, // Получаем имя услуги по ID
            Starttime = clientService.Starttime,
        }).ToList();
        clientServices = new ObservableCollection<ClientservicePresenter>(dataSourceSericeClient);
        ProductListBox.ItemsSource = clientServices;
    }

    public class ClientservicePresenter
    {
        public int Clientid { get; set; }
        public string ClientName { get; set; } // Сделано public
        public int Serviceid { get; set; }
        public string ServiceName { get; set; } // Сделано public
        public DateTime Starttime { get; set; }
    }

}