using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using demo2.Models;

namespace demo2;

public partial class AddAndEditProduct : Window
{
    public string PathToImage = string.Empty;
    public AdminWindow.ServicePresenter? _service;
    public AdminWindow.ServicePresenter servicePresenter;
    
    //TRY EDIT
    // public AddAndEditProduct(AdminWindow.ServicePresenter service = null)
    // {
    //     using var context = new PostgresContext();
    //     InitializeComponent();
    // }
    
    public AddAndEditProduct()
    {
        using var context = new PostgresContext();
        InitializeComponent();
    }

    private async Task<Bitmap?> SelectAndSaveImage()
    {
        var showDialog = StorageProvider.OpenFilePickerAsync(
            options: new Avalonia.Platform.Storage.FilePickerOpenOptions()
            {
                Title = "Select an image",
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
            });
        var storageFile = await showDialog;
        try
        {
            var bmp = new Bitmap(storageFile.First().TryGetLocalPath());
            string path = $"/Users/rinchi/RiderProjects/demo2/demo2/bin/Debug/net8.0/Услуги школы/{Guid.NewGuid()}.jpg";
            bmp.Save(path);
            PathToImage = path;
            return bmp;
        }
        catch
        {
            return null;
        }
    }

    private async void AddClick(object? sender, RoutedEventArgs e)
    {
        using var context = new PostgresContext();
        if (_service == null)
        {
            servicePresenter = new AdminWindow.ServicePresenter();
        }
        else
        {
            servicePresenter = _service;
        }
    
        if (servicePresenter == _service)
        {
            servicePresenter.Id = _service.Id;
        }
        if (string.IsNullOrEmpty(NameTextBox.Text)) return;
        servicePresenter.Title = NameTextBox.Text;
        if (!decimal.TryParse(CostTextBox.Text, out decimal cost)) return;
        servicePresenter.Cost = cost;
        if (!int.TryParse(DurationTextBox.Text, out int duration)) return;
        servicePresenter.Durationinseconds = duration;
        if (string.IsNullOrEmpty(DescriptionTextBox.Text)) return;
        servicePresenter.Description = DescriptionTextBox.Text;
        if (!double.TryParse(SaleTextBox.Text, out double discount)) return;
        servicePresenter.Discount = discount;
        if (String.IsNullOrEmpty(PathToImage)) return;
        servicePresenter.Mainimagepath = PathToImage;
        Close(servicePresenter);
    }
    
    
    //TRY EDIT
    // private async void AddClick(object? sender, RoutedEventArgs e)
    // {
    //     using var context = new PostgresContext();
    //
    //     var servicePresenter = new AdminWindow.ServicePresenter();
    //
    //     if (string.IsNullOrEmpty(NameTextBox.Text)) return;
    //     servicePresenter.Title = NameTextBox.Text;
    //
    //     if (!decimal.TryParse(CostTextBox.Text, out decimal cost)) return;
    //     servicePresenter.Cost = cost;
    //
    //     if (!int.TryParse(DurationTextBox.Text, out int duration)) return;
    //     servicePresenter.Durationinseconds = duration;
    //
    //     if (string.IsNullOrEmpty(DescriptionTextBox.Text)) return;
    //     servicePresenter.Description = DescriptionTextBox.Text;
    //
    //     if (!double.TryParse(SaleTextBox.Text, out double discount)) return;
    //     servicePresenter.Discount = discount;
    //
    //     if (string.IsNullOrEmpty(PathToImage)) return;
    //     servicePresenter.Mainimagepath = PathToImage;
    //
    //     var newService = new Service
    //     {
    //         Title = servicePresenter.Title,
    //         Cost = servicePresenter.Cost,
    //         Durationinseconds = servicePresenter.Durationinseconds,
    //         Description = servicePresenter.Description,
    //         Discount = servicePresenter.Discount,
    //         Mainimagepath = servicePresenter.Mainimagepath
    //     };
    //
    //     context.Services.Add(newService);
    //
    //     await context.SaveChangesAsync();
    //     Close(servicePresenter);
    // }
    
    private async void SelectImage(object? sender, RoutedEventArgs e)
    {
        ServiceImage.Source = await SelectAndSaveImage();
    }
}