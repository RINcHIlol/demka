using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using demo2.Models;
using Microsoft.EntityFrameworkCore;

namespace demo2;

public partial class AdminWindow : Window
{
    ObservableCollection<ServicePresenter> services = new ObservableCollection<ServicePresenter>();
    List<ServicePresenter> dataSourceServices;
    public AdminWindow()
    {
        InitializeComponent();
        using var context = new PostgresContext();
        dataSourceServices = context.Services.Select(service => new ServicePresenter
        {
            Id = service.Id,
            Title = service.Title,
            Cost = service.Cost,
            Durationinseconds = service.Durationinseconds,
            Description = service.Description,
            Discount = service.Discount,
            Mainimagepath = service.Mainimagepath,
        }).ToList();
        var dataSourceClients = context.Clients.Select(it => it.Lastname).ToList();
        ServiceListBox.ItemsSource = services;
        FilterCombobox.SelectedIndex = 0;
        DisplayServices();
    }
    
    public class ServicePresenter() : Service
    {
        Bitmap? Image
        {
            get
            {
                try
                {
                    string absolutePath = Path.Combine(AppContext.BaseDirectory, Mainimagepath);
                    return new Bitmap(absolutePath);
                }
                catch
                {
                    return null;
                }
            }
        }

        string? DurationMin
        {
            get
            {
                if (Durationinseconds >= 60)
                {
                    return (Durationinseconds / 60).ToString() + " min";
                }
                return Durationinseconds.ToString() + " sec";
            }
        }

        public decimal? CostXDiscount
        {
            get
            {
                return Cost - Cost * (decimal)(Discount ?? 0);
            }
        }

        public string? FormattedCost
        {
            get
            {
                if (Durationinseconds > 0)
                {
                    return $"{(CostXDiscount):N0} рублей";
                }

                return null;
            }
        }

        public string? FormattedDiscount
        {
            get
            {
                if (Discount != 0)
                {
                    return $"* скидка {(Discount ?? 0) * 100:N0} %";
                }
                return null;
            }
        }

        public string? CombinedCostDuration
        {
            get
            {
                return $"{FormattedCost} за {DurationMin}";
            }
        }
    }
    
    private async void AddProductButton_OnClick(object? sender, RoutedEventArgs e)
    {
        using var context = new PostgresContext();
        var newService = await new AddAndEditProduct().ShowDialog<ServicePresenter>(this);
        context.Services.Add(newService);
        if(context.SaveChanges() > 0) dataSourceServices.Add(newService);
    }
    
    private async void EditProduct_OnClick(object? sender, RoutedEventArgs e)
    {
        //TRY EDIT
        // using var context = new PostgresContext();
        // var selectedService = ServiceListBox.SelectedItems as ServicePresenter;
        // var newService = await new AddAndEditProduct(selectedService).ShowDialog<ServicePresenter>(this);
        // if (newService != null)
        // {
        //     // context.Services.Remove(selectedService);
        //     // dataSourceServices.Remove(selectedService);
        //     context.Services.Add(newService);
        //     dataSourceServices.Add(newService);
        //     context.SaveChanges();
        // }
    }

    private void UpcomingPostsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CheckAppointment checkAppointment = new CheckAppointment();
        checkAppointment.ShowDialog(this);
    }

    private void Appointment_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedService = ServiceListBox.SelectedItem as ServicePresenter;
        int serviceId = selectedService.Id;
        Appointment appointment = new Appointment(serviceId);
        appointment.ShowDialog(this);
    }

    private void ServiceRemoveClick(object sender, RoutedEventArgs e)
    {
        using var context = new PostgresContext();
        var service = ServiceListBox.SelectedItem as ServicePresenter;
        if (service == null) return;
        var removeService = context.Services.Include(it => it.Clientservices).First(it => it.Id == service.Id);
        if (removeService == null) return;
        if (removeService.Clientservices.Count() > 0) return;
        context.Services.Remove(removeService);
        if(context.SaveChanges() > 0) services.Remove(service);
    }

    public void DisplayServices()
    {
        var temp = dataSourceServices;
        services.Clear();
        switch (FilterCombobox.SelectedIndex)
        {
            case 0: temp = temp; break;
            case 1: temp = temp.Where(it => it.Discount >= 0 && it.Discount < 0.05).ToList(); break;
            case 2: temp = temp.Where(it => it.Discount >= 0.05 && it.Discount < 0.15).ToList(); break;
            case 3: temp = temp.Where(it => it.Discount >= 0.15 && it.Discount < 0.30).ToList(); break;
            case 4: temp = temp.Where(it => it.Discount >= 0.30 && it.Discount < 0.70).ToList(); break;
            case 5: temp = temp.Where(it => it.Discount >= 0.70 && it.Discount < 0.100).ToList(); break;
            default: break;
        }

        if (!string.IsNullOrEmpty(SearchTextBox.Text))
        {
            var search = SearchTextBox.Text;
            temp = temp.Where(it => IsContains(it.Title, it.Description, search)).ToList();
        }

        switch (SortComboBox.SelectedIndex)
        {
            case 1: temp = temp.OrderBy(it => it.CostXDiscount).ToList(); break;
            case 0: temp = temp.OrderByDescending(it => it.CostXDiscount).ToList(); break;
            default: break;
        }

        foreach (var item in temp)
        {
            services.Add(item);
        }
        
        CountServices.Text = $"{temp.Count}/{dataSourceServices.Count}";
    }

    public bool IsContains(string title, string? description, string search)
    {
        string desc = string.Empty;
        if(description != null) desc = description;
        string message = (title + desc).ToLower();
        search = search.ToLower();
        return message.Contains(search);
    }

    private void SearchTextBox_OnTextChanging(object? sender, TextChangingEventArgs e)
    {
        DisplayServices();
    }

    private void SortComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        DisplayServices();
    }

    private void FilterCombobox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        DisplayServices();
    }
}