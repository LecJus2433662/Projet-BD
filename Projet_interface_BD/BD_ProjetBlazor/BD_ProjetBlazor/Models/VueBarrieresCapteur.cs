using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BD_ProjetBlazor.Models;

[Keyless]
public partial class VueBarrieresCapteur
{
    [Column("numBarriere")]
    public int NumBarriere { get; set; }

    [Column("dureeAttente")]
    public int DureeAttente { get; set; }

    [Column("tempsOuverture")]
    public TimeOnly TempsOuverture { get; set; }

    [Column("numCapteur")]
    public int NumCapteur { get; set; }

    [Column("mouvement", TypeName = "decimal(6, 2)")]
    public decimal Mouvement { get; set; }

    [Column("dateCapteur")]
    public DateOnly DateCapteur { get; set; }
}
