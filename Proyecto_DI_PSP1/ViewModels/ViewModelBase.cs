using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Proyecto_DI_PSP1.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void AvisarCambio([CallerMemberName] string? propiedad = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
}

public class Comando(Action ejecutar) : ICommand
{
    public event EventHandler? CanExecuteChanged { add { } remove { } }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => ejecutar();
}
