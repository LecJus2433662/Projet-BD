using BD_ProjetBlazor.Authentication;
using BD_ProjetBlazor.Components.Pages;
using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
using BD_ProjetBlazor.Partials;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
namespace BD_ProjetBlazor.Services
{
    public class Requete_Reservation
    {

        private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;
        private readonly UserSessionService _session;
        public Requete_Reservation(IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory, UserSessionService session)
        {
            _dbContextFactory = dbContextFactory;
            _session = session;
        }

        // Les fonctions

        public async Task<int> GetTotalParkingSpacesAsync()
        {
            using var _context = _dbContextFactory.CreateDbContext();
            var totalPlaces = await _context.Stationnements.SumAsync(s => s.NombrePlaceMax);
            return totalPlaces;
        }
        public async Task<int> GetTotalReservationsAsync(int stationnementId)
        {
            using var _context = _dbContextFactory.CreateDbContext();
            var now = DateOnly.FromDateTime(DateTime.Now);

            // Compte uniquement les réservations qui sont en cours actuellement
            var reservationsActives = await _context.StationnementEntreeSorties
                .Where(r =>
                    r.NumStationnement == stationnementId &&
                    r.Reservation == true &&
                    r.DateEntree <= now &&
                    r.DateSortie >= now
                )
                .ToListAsync();

            return reservationsActives.Count;
        }
        public async Task<int> GetAvailableSpacesAsync(int stationnementId)
        {
            var totalPlaces = await GetTotalParkingSpacesAsync();
            var totalReservations = await GetTotalReservationsAsync(stationnementId);
            return totalPlaces - totalReservations;
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
            using var _context = _dbContextFactory.CreateDbContext();

            // Vérifie que le stationnement existe
            var stationnement = await _context.Stationnements
                .FirstOrDefaultAsync(n => n.NumStationnement == form.NumStationnement);

            if (stationnement == null)
                return null;

            int placeMax = stationnement.NombrePlaceMax;

            // Compte le nombre de réservations qui **chevauchent exactement la période demandée**
            int totalReservations = await _context.StationnementEntreeSorties
                .CountAsync(r =>
                    r.NumStationnement == form.NumStationnement &&
                    r.Reservation == true &&
                    r.DateEntree < form.dateSortie &&
                    r.DateSortie > form.dateEntree
                );

            // Si le nombre de réservations chevauchantes atteint la capacité maximale, pas de place
            if (totalReservations >= placeMax)
                return null;

            // Sinon, la réservation est possible
            return stationnement;
        }



        public async Task<bool> AjouterReservationAsync(StationnementEntreeSortie reservation)
        {
            using var _context = _dbContextFactory.CreateDbContext();
            int? userId = await _session.GetUserIdAsync();
            if (userId == null)
                return false;

            var reservations = await _context.StationnementEntreeSorties
                .Where(r => r.NumStationnement == reservation.NumStationnement)
    .           ToListAsync();

            // Récupérer le stationnement correspondant pour accéder au nombre maximum de places
            var stationnement = await _context.Stationnements
                .FirstOrDefaultAsync(s => s.NumStationnement == reservation.NumStationnement);

            if (stationnement == null)
            {
                // Stationnement inexistant
                return false;
            }
            // Vérifie si le nombre de réservations dépasse la capacité
            if (reservations.Count >= stationnement.NombrePlaceMax)
            {
                // Plus de places disponibles
                return false;
            }
            try
            {
                var paramIdUtilisateur = new SqlParameter("@numUtilisateur", userId);
                var paramNumStationnement = new SqlParameter("@numStationnement", reservation.NumStationnement);
                var paramDateEntree = new SqlParameter("@dateEntree", reservation.DateEntree);
                var paramDateSortie = new SqlParameter("@dateSortie", reservation.DateSortie);
                var paramPaiementSortie = new SqlParameter("@paiementSortie", reservation.PaiementSortie);
                var paramReservation = new SqlParameter("@reservation", reservation.Reservation);
                var paramPaiementRecu = new SqlParameter("@paiementRecu", false);

                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    @"INSERT INTO stationnementEntreeSortie (numUtilisateur, numStationnement, dateEntree, dateSortie, reservation, paiementSortie, paiementRecu) 
                    VALUES (@numUtilisateur ,@numStationnement, @dateEntree, @dateSortie, @reservation, @paiementSortie, @paiementRecu)",
                    paramIdUtilisateur, paramNumStationnement, paramDateEntree, paramDateSortie, paramReservation, paramPaiementSortie, paramPaiementRecu);


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



