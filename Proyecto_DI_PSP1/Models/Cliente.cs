using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proyecto_DI_PSP1.Models;

[Table("Clientes")]
public class Cliente
{
    [Key, Column("id_clientes")]
    public int IdCliente { get; set; }

    [Column("nombre"), MaxLength(100)]
    public string Nombre { get; set; } = "";

    [Column("telefono"), MaxLength(20)]
    public string? Telefono { get; set; }

    [Column("email"), MaxLength(100)]
    public string? Email { get; set; }
}
