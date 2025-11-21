using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BD_ProjetBlazor.Models;

[Table("barriere")]
public partial class Barriere
{
    [Key]
    [Column("numBarriere")]
    public int NumBarriere { get; set; }

    [Column("dureeAttente")]
    public int DureeAttente { get; set; }

    [Column("noBarriereOuverture")]
    public int? NoBarriereOuverture { get; set; }

    [Column("tempsOuverture", TypeName = "datetime")]
    public DateTime? TempsOuverture { get; set; }

    [Column("numeroCapteur")]
    public int NumeroCapteur { get; set; }

    [ForeignKey("NumeroCapteur")]
    [InverseProperty("Barrieres")]
    public virtual Capteur NumeroCapteurNavigation { get; set; } = null!;

    [InverseProperty("NumBarriereNavigation")]
    public virtual ICollection<StationnementEntreeSortie> StationnementEntreeSorties { get; set; } = new List<StationnementEntreeSortie>();
}
