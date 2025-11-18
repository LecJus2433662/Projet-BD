using BD_ProjetBlazor.Components.Pages;
using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BD_ProjetBlazor.Services
{
    public class Requete_Reservation
    {
        private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;

        public Requete_Reservation(IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task<int> GetTotalReservationsAsync()
        {
            using var _context = _dbContextFactory.CreateDbContext();
            return await Task.FromResult(_context.StationnementEntreeSorties.Count(r => r.Reservation == true));
        }
        public async Task<Stationnement?> IsParkingAvailableAsync(int stationnementId, DateOnly startDateTime, DateOnly endDateTime)
        {
            int placeMax = 0;
            using var _context = _dbContextFactory.CreateDbContext();
            var stationnement = await _context.Stationnements.FirstOrDefaultAsync();
            var numStationnement = await _context.Stationnements
                .Where(r => r.NumStationnement == stationnementId)
                .Where(p => p.NombrePlaceMax == placeMax)
                .FirstOrDefaultAsync();

            if (numStationnement == null)
            {
                return null;
            }


            int totalReservations = await GetTotalReservationsAsync();
            if (placeMax > totalReservations)
            {
                var verifierTemps = await _context.StationnementEntreeSorties
                .AnyAsync(d => d.DateEntree > startDateTime && d.DateSortie < endDateTime);

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
                // Ajouter la réservation dans la table Stationnement
                _context.StationnementEntreeSorties.Add(reservation);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public class Reservation
        {
            public int NumStationnement { get; set; }
            public DateOnly HeureDebut { get; set; }
            public DateOnly HeureFin { get; set; }
        }
    }
}

