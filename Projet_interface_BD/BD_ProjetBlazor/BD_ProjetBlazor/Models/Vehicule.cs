using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BD_ProjetBlazor.Models;

[Table("vehicule")]
[Index("PlaqueImmatriculation", Name = "unique_plaque", IsUnique = true)]
public partial class Vehicule
{
    [Key]
    [Column("numVehicule")]
    public int NumVehicule { get; set; }

    [Column("marque")]
    [StringLength(30)]
    [Unicode(false)]
    public string Marque { get; set; } = null!;

    [Column("modele")]
    [StringLength(30)]
    [Unicode(false)]
    public string Modele { get; set; } = null!;

    [Column("plaqueImmatriculation")]
    [StringLength(30)]
    [Unicode(false)]
    public string PlaqueImmatriculation { get; set; } = null!;

    [Column("couleur")]
    [StringLength(30)]
    [Unicode(false)]
    public string Couleur { get; set; } = null!;

    [InverseProperty("NumVehiculeNavigation")]
    public virtual ICollection<StationnementEntreeSortie> StationnementEntreeSorties { get; set; } = new List<StationnementEntreeSortie>();
}
