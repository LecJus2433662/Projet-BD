using BD_ProjetBlazor.Components.Pages;
using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using BD_ProjetBlazor.Partials;

namespace BD_ProjetBlazor.Services
{
    public class Requete_Reservation
    {

        private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;

        public Requete_Reservation(IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        // Les fonctions

        public async Task<int> GetTotalReservationsAsync(int stationnementId)
        {
            using var _context = _dbContextFactory.CreateDbContext();
            var reservations = await _context.StationnementEntreeSorties
                .Where(r => r.NumStationnement == stationnementId && r.Reservation == true)
                .ToListAsync();
            return reservations.Count;
        }
        public int CalculerNombreDeJours(DateOnly dateEntree, DateOnly dateSortie)
        {
            var difference = dateSortie.ToDateTime(new TimeOnly()) - dateEntree.ToDateTime(new TimeOnly());
            return (int)difference.TotalDays;
        }
        public async Task<decimal?> GetTarifJournalierAsync()
        {
            using var _context = _dbContextFactory.CreateDbContext();

            // Lire le tarif global (premier dans la table)
            var stationnement = await _context.Stationnements.FirstOrDefaultAsync();

            return stationnement?.Tarif ?? 0;
        }

        public async Task<decimal> CalculerMontantAPayer(DateOnly dateEntree, DateOnly dateSortie)
        {
            using var _context = _dbContextFactory.CreateDbContext();

            var tarif = await _context.Stationnements
                .Select(s => s.Tarif)
                .FirstOrDefaultAsync();

            if (tarif <= 0) return 0;

            // Calculer le nombre de jours minimum 1
            int nbJours = (dateSortie.ToDateTime(TimeOnly.MinValue) - dateEntree.ToDateTime(TimeOnly.MinValue)).Days + 1;

            if (nbJours < 1)
                nbJours = 1;

            return tarif * nbJours;
        }
        public async Task<Stationnement?> IsParkingAvailableAsync(ReservationForm form)
        {
            int placeMax = 0;
            using var _context = _dbContextFactory.CreateDbContext();
            var stationnement = await _context.Stationnements.FirstOrDefaultAsync();
            var numStationnement = await _context.Stationnements
                .FirstOrDefaultAsync(n => n.NumStationnement == form.NumStationnement);

            if (numStationnement == null)
            {
                return null;
            }
            placeMax = numStationnement.NombrePlaceMax;

            int totalReservations = await GetTotalReservationsAsync(form.NumStationnement);
            if (placeMax > totalReservations)
            {
                var verifierTemps = await _context.StationnementEntreeSorties
                .AnyAsync(d => d.DateEntree > form.dateEntree && d.DateSortie < form.dateSortie);

                if (!verifierTemps)
                {
                    return stationnement;
                }

            }
            return null;
        }

        public async Task<bool> AjouterReservationAsync(StationnementEntreeSortie reservation)
        {
            using var _context = _dbContextFactory.CreateDbContext();
            try
            {
                var paramNumStationnement = new SqlParameter("@numStationnement", reservation.NumStationnement);
                var paramDateEntree = new SqlParameter("@dateEntree", reservation.DateEntree);
                var paramDateSortie = new SqlParameter("@dateSortie", reservation.DateSortie);
                var paramPaiementSortie = new SqlParameter("@paiementSortie", reservation.PaiementSortie);
                var paramReservation = new SqlParameter("@reservation", reservation.Reservation);
                var paramPaiementRecu = new SqlParameter("@paiementRecu", false);

                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    @"INSERT INTO stationnementEntreeSortie (numStationnement, dateEntree, dateSortie, reservation, paiementSortie, paiementRecu) 
                    VALUES (@numStationnement, @dateEntree, @dateSortie, @reservation, @paiementSortie, @paiementRecu)",
                    paramNumStationnement, paramDateEntree, paramDateSortie, paramReservation, paramPaiementSortie, paramPaiementRecu);


                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout de la réservation : {ex.Message}");
                return false;
            }
        }


        public async Task<List<Stationnement>> GetAvailableStationnementsAsync(ReservationForm form)
        {
            using var _context = _dbContextFactory.CreateDbContext();

            // Récupérer tous les stationnements
            var stationnements = await _context.Stationnements.ToListAsync();

            var availableStationnements = new List<Stationnement>();

            foreach (var stationnement in stationnements)
            {
                var numStationnement = await _context.Stationnements
                    .AnyAsync(n => n.NumStationnement == stationnement.NumStationnement);
                // Vérifier si le stationnement est disponible pour les dates demandées
                var reservationExistante = await _context.StationnementEntreeSorties
                    .AnyAsync(r => r.DateEntree < form.dateEntree
                                   && r.DateSortie > form.dateSortie);

                // Si pas de réservation existante, le stationnement est disponible
                if (!reservationExistante && numStationnement)
                {
                    availableStationnements.Add(stationnement);
                }
            }
            Console.WriteLine($"Stationnements disponibles : {availableStationnements.Count}");
            return availableStationnements;
        }
    }
}



