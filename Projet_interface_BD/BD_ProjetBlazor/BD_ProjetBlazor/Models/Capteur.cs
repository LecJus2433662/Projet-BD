using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BD_ProjetBlazor.Models;

[Table("capteur")]
public partial class Capteur
{
    [Key]
    [Column("numCapteur")]
    public int NumCapteur { get; set; }

    [Column("mouvement", TypeName = "decimal(6, 2)")]
    public decimal Mouvement { get; set; }

    [Column("dates")]
    public DateOnly Dates { get; set; }

    [InverseProperty("NumeroCapteurNavigation")]
    public virtual ICollection<Barriere> Barrieres { get; set; } = new List<Barriere>();
}
