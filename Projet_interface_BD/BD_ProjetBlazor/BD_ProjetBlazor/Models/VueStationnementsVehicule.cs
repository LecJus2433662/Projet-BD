using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BD_ProjetBlazor.Models;

[Keyless]
public partial class VueStationnementsVehicule
{
    [Column("entreSortieStationnement")]
    public int EntreSortieStationnement { get; set; }

    [Column("dateEntree")]
    public DateOnly DateEntree { get; set; }

    [Column("dateSortie")]
    public DateOnly DateSortie { get; set; }

    [Column("paiementSortie", TypeName = "decimal(6, 2)")]
    public decimal PaiementSortie { get; set; }

    [Column("paiementRecu")]
    public bool PaiementRecu { get; set; }

    [Column("reservation")]
    public bool? Reservation { get; set; }

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

    [Column("numBarriere")]
    public int NumBarriere { get; set; }

    [Column("tempsOuverture")]
    public TimeOnly TempsOuverture { get; set; }
}
