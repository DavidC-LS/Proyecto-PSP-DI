using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Proyecto_DI_PSP1.ViewModels;

// Base de todos los <Models>, avisa a la vista cuando cambio un dato 
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // Aviso a la vista que ha cambiado un dato
    protected void AvisarCambio([CallerMemberName] string? propiedad = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
}

public class Comando(Action ejecutar) : ICommand
{
    public event EventHandler? CanExecuteChanged { add { } remove { } }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => ejecutar();
}
