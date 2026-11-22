using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Proyecto_DI_PSP1.Models;

namespace Proyecto_DI_PSP1.ViewModels;

public class CrudViewModel<T>(Func<TallerContext> abrirContexto, Func<T, object[]> clave,
    Func<TallerContext, T, string?> validar, Action alGuardar, Action<string> informar)
    : ViewModelBase where T : class, new()
{
    private T? seleccionado;
    private T formulario = new();

    public ObservableCollection<T> Filas { get; } = new();

    public T? Seleccionado
    {
        get => seleccionado;
        set
        {
            if (ReferenceEquals(seleccionado, value))
                return;
            seleccionado = value;
            Formulario = value is null ? new T() : Copiar(value);
            AvisarCambio();
        }
    }

    public T Formulario
    {
        get => formulario;
        private set
        {
            formulario = value;
            AvisarCambio();
        }
    }

    public Comando Nuevo => new(() => { Seleccionado = null; Formulario = new T(); });
    public Comando Guardar => new(GuardarFila);
    public Comando Borrar => new(BorrarFila);

    public void Cargar(TallerContext db)
    {
        var datos = db.Set<T>().AsNoTracking().ToList();
        Seleccionado = null;
        Formulario = new T();
        Filas.Clear();
        foreach (var fila in datos)
            Filas.Add(fila);
    }

    private void GuardarFila()
    {
        try
        {
            using var db = abrirContexto();
            string? problema = validar(db, Formulario);
            if (problema is not null)
            {
                informar(problema);
                return;
            }

            if (Seleccionado is null)
                db.Add(Formulario);
            else
            {
                if (!clave(Seleccionado).SequenceEqual(clave(Formulario)))
                {
                    informar("Para cambiar la asignación, crea una nueva y borra la anterior.");
                    return;
                }

                var fila = db.Set<T>().Find(clave(Seleccionado))
                    ?? throw new InvalidOperationException("El registro seleccionado ya no existe.");
                db.Entry(fila).CurrentValues.SetValues(Formulario);
            }

            db.SaveChanges();
            informar("Cambios guardados.");
            alGuardar();
        }
        catch (Exception ex) when (ex is DbUpdateException or MySqlConnector.MySqlException or InvalidOperationException)
        {
            informar("No se pudo guardar. Revisa la conexión y que los datos no estén duplicados.");
        }
    }

    private void BorrarFila()
    {
        if (Seleccionado is null)
        {
            informar("Selecciona el registro que quieras borrar.");
            return;
        }

        if (System.Windows.MessageBox.Show("¿Borrar el registro y los datos relacionados en cascada?",
            "Confirmar borrado", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question)
            != System.Windows.MessageBoxResult.Yes)
            return;

        try
        {
            using var db = abrirContexto();
            var fila = db.Set<T>().Find(clave(Seleccionado))
                ?? throw new InvalidOperationException("El registro seleccionado ya no existe.");
            db.Remove(fila);
            db.SaveChanges();
            informar("Registro borrado.");
            alGuardar();
        }
        catch (Exception ex) when (ex is DbUpdateException or MySqlConnector.MySqlException or InvalidOperationException)
        {
            informar("No se pudo borrar el registro. Carga los datos y vuelve a comprobar la selección.");
        }
    }

    private T Copiar(T original)
    {
        using var db = abrirContexto();
        return (T)db.Entry(original).CurrentValues.ToObject();
    }
}
