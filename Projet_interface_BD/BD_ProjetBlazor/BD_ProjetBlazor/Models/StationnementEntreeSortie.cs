using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BD_ProjetBlazor.Models;

[Table("stationnementEntreeSortie")]
public partial class StationnementEntreeSortie
{
    [Key]
    [Column("entreSortieStationnement")]
    public int EntreSortieStationnement { get; set; }

    [Column("dateEntree")]
    public DateOnly DateEntree { get; set; }

    [Column("dateSortie")]
    public DateOnly DateSortie { get; set; }

    [Column("paiementSortie", TypeName = "decimal(4, 2)")]
    public decimal PaiementSortie { get; set; }

    [Column("paiementRecu")]
    public bool PaiementRecu { get; set; }

    [Column("numVehicule")]
    public int? NumVehicule { get; set; }

    [Column("numBarriere")]
    public int? NumBarriere { get; set; }

    [Column("numUtilisateur")]
    public int? NumUtilisateur { get; set; }

    [Column("reservation")]
    public bool? Reservation { get; set; }

    [ForeignKey("NumBarriere")]
    [InverseProperty("StationnementEntreeSorties")]
    public virtual Barriere? NumBarriereNavigation { get; set; }

    [ForeignKey("NumVehicule")]
    [InverseProperty("StationnementEntreeSorties")]
    public virtual Vehicule? NumVehiculeNavigation { get; set; }

    [InverseProperty("EntreSortieStationnementNavigation")]
    public virtual ICollection<Stationnement> Stationnements { get; set; } = new List<Stationnement>();
}
