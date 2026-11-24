using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proyecto_DI_PSP1.Models;

[Table("Facturas")]
public class Factura
{
    [Key, Column("id_factura")]
    public int IdFactura { get; set; }

    [Column("id_reparacion")]
    public int IdReparacion { get; set; }

    [Column("importe"), Precision(5, 2)]
    public decimal Importe { get; set; }

    [Column("fecha", TypeName = "date")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Column("resultado"), MaxLength(30)]
    public string? Resultado { get; set; }
}
