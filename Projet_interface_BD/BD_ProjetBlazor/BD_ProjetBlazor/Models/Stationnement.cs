using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BD_ProjetBlazor.Models;

[Table("stationnement")]
public partial class Stationnement
{
    [Key]
    [Column("numStationnement")]
    public int NumStationnement { get; set; }

    [Column("nombrePlaceMax")]
    public int NombrePlaceMax { get; set; }

    [Column("dureeMaxStationnement")]
    public TimeOnly DureeMaxStationnement { get; set; }

    [Column("entreSortieStationnement")]
    public int? EntreSortieStationnement { get; set; }

    [Column("tarif", TypeName = "decimal(6, 2)")]
    public decimal Tarif { get; set; }

    [Column("estPlein")]
    public bool? EstPlein { get; set; }

    [ForeignKey("EntreSortieStationnement")]
    [InverseProperty("Stationnements")]
    public virtual StationnementEntreeSortie? EntreSortieStationnementNavigation { get; set; }

    [InverseProperty("NumStationnementNavigation")]
    public virtual ICollection<StationnementEntreeSortie> StationnementEntreeSorties { get; set; } = new List<StationnementEntreeSortie>();
}
