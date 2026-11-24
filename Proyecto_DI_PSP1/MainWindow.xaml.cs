using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Proyecto_DI_PSP1.ViewModels;

namespace Proyecto_DI_PSP1;

public partial class MainWindow : Window
{
    private readonly TallerViewModel taller = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = taller;
    }

    private void CambiarClave(object sender, RoutedEventArgs e) =>
        taller.Clave = ((PasswordBox)sender).Password;

    private void MostrarModelo(object sender, RoutedEventArgs e)
    {
        var imagen = new Image
        {
            Source = new BitmapImage(new Uri("pack://application:,,,/ModeloRelacional.png")),
            Margin = new Thickness(12)
        };
        new Window
        {
            Title = "Modelo relacional del taller",
            Owner = this,
            Width = 980,
            Height = 530,
            Content = imagen
        }.ShowDialog();
    }
}
