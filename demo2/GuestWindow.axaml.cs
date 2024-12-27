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

namespace demo2
{
    public partial class GuestWindow : Window
    {
        ObservableCollection<ServicePresenter> services = new ObservableCollection<ServicePresenter>();
        List<ServicePresenter> dataSourceServices;
        ObservableCollection<string> clients;

        public GuestWindow()
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
            clients = new ObservableCollection<string>(dataSourceClients);
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
                get { return Cost - Cost * (decimal)(Discount ?? 0); }
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
                get { return $"{FormattedCost} за {DurationMin}"; }
            }
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
            if (description != null) desc = description;
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
}