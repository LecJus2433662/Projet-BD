using System;
using System.Collections.Generic;
using BD_ProjetBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace BD_ProjetBlazor.Data;

public partial class ProgA25BdProjetProgContext : DbContext
{
    public ProgA25BdProjetProgContext()
    {
    }

    public ProgA25BdProjetProgContext(DbContextOptions<ProgA25BdProjetProgContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Barriere> Barrieres { get; set; }

    public virtual DbSet<Capteur> Capteurs { get; set; }

    public virtual DbSet<Stationnement> Stationnements { get; set; }

    public virtual DbSet<StationnementEntreeSortie> StationnementEntreeSorties { get; set; }

    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }

    public virtual DbSet<Vehicule> Vehicules { get; set; }

    public virtual DbSet<VueBarrieresCapteur> VueBarrieresCapteurs { get; set; }

    public virtual DbSet<VueStationnementsVehicule> VueStationnementsVehicules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Barriere>(entity =>
        {
            entity.HasKey(e => e.NumBarriere).HasName("PK__barriere__5418E0DD7936B5ED");

            entity.HasOne(d => d.NumeroCapteurNavigation).WithMany(p => p.Barrieres)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_numCapteur");
        });

        modelBuilder.Entity<Capteur>(entity =>
        {
            entity.HasKey(e => e.NumCapteur).HasName("PK__capteur__B87DDAAA97190C17");
        });

        modelBuilder.Entity<Stationnement>(entity =>
        {
            entity.HasKey(e => e.NumStationnement).HasName("PK__stationn__E316D911F68BE1B4");

            entity.HasOne(d => d.EntreSortieStationnementNavigation).WithMany(p => p.Stationnements).HasConstraintName("fk_entreSortieStationnement");
        });

        modelBuilder.Entity<StationnementEntreeSortie>(entity =>
        {
            entity.HasKey(e => e.EntreSortieStationnement).HasName("PK__stationn__64A2E70706E3536B");

            entity.ToTable("stationnementEntreeSortie", tb => tb.HasTrigger("trg_VerifierPlaceStationnement"));

            entity.Property(e => e.DateEntree).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Reservation).HasDefaultValue(false);

            entity.HasOne(d => d.NumBarriereNavigation).WithMany(p => p.StationnementEntreeSorties).HasConstraintName("fk_numBarriere");

            entity.HasOne(d => d.NumStationnementNavigation).WithMany(p => p.StationnementEntreeSorties).HasConstraintName("fk_numStationnement");

            entity.HasOne(d => d.NumVehiculeNavigation).WithMany(p => p.StationnementEntreeSorties).HasConstraintName("fk_numVehicule");
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.NoUtilisateur).HasName("PK__utilisat__CB66E30B3ECCE949");

            entity.Property(e => e.MotDePasse).IsFixedLength();
        });

        modelBuilder.Entity<Vehicule>(entity =>
        {
            entity.HasKey(e => e.NumVehicule).HasName("PK__vehicule__1F9286B10B7BE12D");
        });

        modelBuilder.Entity<VueBarrieresCapteur>(entity =>
        {
            entity.ToView("vue_Barrieres_Capteurs");
        });

        modelBuilder.Entity<VueStationnementsVehicule>(entity =>
        {
            entity.ToView("vue_Stationnements_Vehicules");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
