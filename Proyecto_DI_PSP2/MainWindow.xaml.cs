using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Proyecto_DI_PSP2;

public partial class MainWindow : Window
{
    private readonly CancellationTokenSource cancelacion = new();
    private readonly string archivo = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Proyecto_DI_PSP", "registro.json");

    public ObservableCollection<RegistroPeticion> Registros { get; } = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private async void AlAbrir(object sender, RoutedEventArgs e)
    {
        try
        {
            List<RegistroPeticion> historial = File.Exists(archivo)
                ? JsonSerializer.Deserialize<List<RegistroPeticion>>(await File.ReadAllTextAsync(archivo, cancelacion.Token))
                    ?? new List<RegistroPeticion>()
                : new List<RegistroPeticion>();

            foreach (var registro in historial)
                Registros.Add(registro);

            var servidor = new ServidorIncentivos(archivo, historial);
            servidor.Registrada += registro => Dispatcher.InvokeAsync(() => Registros.Add(registro));
            EstadoServidor.Text = "Esperando peticiones de la aplicación de gestión.";
            await Task.Run(() => servidor.EscucharAsync(cancelacion.Token));
        }
        catch (OperationCanceledException) when (cancelacion.IsCancellationRequested)
        {
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            EstadoServidor.Text = "No se pudo iniciar o continuar la escucha. " + ex.Message;
        }
        finally
        {
            cancelacion.Dispose();
        }
    }

    private void AlCerrar(object? sender, EventArgs e)
    {
        // La espera del pipe y la lectura aceptan cancelación.
        if (EstadoServidor.Text.StartsWith("Esperando", StringComparison.Ordinal))
            cancelacion.Cancel();
    }

    private void MostrarAutores(object sender, RoutedEventArgs e) =>
        MessageBox.Show("Procesador de incentivos del taller\nDiego González y David Cordón\n12/11/2026 · Versión 1.0",
            "Acerca de", MessageBoxButton.OK, MessageBoxImage.Information);
}
