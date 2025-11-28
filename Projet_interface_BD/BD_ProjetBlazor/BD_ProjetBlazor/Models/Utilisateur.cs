using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BD_ProjetBlazor.Models;

[Table("utilisateur")]
[Index("Email", Name = "unique_email", IsUnique = true)]
public partial class Utilisateur
{
    [Key]
    [Column("noUtilisateur")]
    public int NoUtilisateur { get; set; }

    [Column("nom")]
    [StringLength(30)]
    [Unicode(false)]
    public string Nom { get; set; } = null!;

    [Column("prenom")]
    [StringLength(30)]
    [Unicode(false)]
    public string Prenom { get; set; } = null!;

    [Column("ville")]
    [StringLength(30)]
    [Unicode(false)]
    public string Ville { get; set; } = null!;

    [Column("pays")]
    [StringLength(30)]
    [Unicode(false)]
    public string Pays { get; set; } = null!;

    [Column("email")]
    [StringLength(255)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Column("motDePasse")]
    [MaxLength(64)]
    public byte[] MotDePasse { get; set; } = null!;

    [Column("sel")]
    public Guid Sel { get; set; }

    public bool IsAdmin { get; set; }
}
