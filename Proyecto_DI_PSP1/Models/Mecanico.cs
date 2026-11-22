using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proyecto_DI_PSP1.Models;

[Table("Mecanicos")]
public class Mecanico
{
    [Key, Column("id_mecanico")]
    public int IdMecanico { get; set; }

    [Column("nombre"), MaxLength(100)]
    public string Nombre { get; set; } = "";

    [Column("especialidad"), MaxLength(100)]
    public string Especialidad { get; set; } = "";
}
