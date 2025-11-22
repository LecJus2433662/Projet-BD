using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Globalization;

namespace BD_ProjetBlazor.Services
{
    public class Requete_EntreeSortieService
    {
        private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;


        public Requete_EntreeSortieService(IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<int> GetTotalParkingSpacesAsync()
        {
            using var _context = _dbContextFactory.CreateDbContext();
            var stationnement = await _context.Stationnements.FirstOrDefaultAsync();

            if (stationnement != null)
            {
                return stationnement.NombrePlaceMax;
            }
            else
            {
                return 0;
            }
        }

        public async Task<int> GetTotalReservationsAsync()
        {
            using var _context = _dbContextFactory.CreateDbContext();
            return await Task.FromResult(_context.StationnementEntreeSorties.Count(r => r.Reservation == true));
        }

        public async Task<int> GetAvailableSpacesAsync()
        {
            var totalSpaces = await GetTotalParkingSpacesAsync();
            var reservedSpaces = await GetTotalReservationsAsync();
            return totalSpaces - reservedSpaces;
        }

        public async Task<double> GetOccupiedPercentageAsync()
        {
            var totalSpaces = await GetTotalParkingSpacesAsync();
            var availableSpaces = await GetAvailableSpacesAsync();
            return ((totalSpaces - availableSpaces) / (double)totalSpaces) * 100;
        }

        public async Task<decimal?> GetTarifActuel()
        {
            using var _context = _dbContextFactory.CreateDbContext();
            var stationnement = await _context.Stationnements.FirstOrDefaultAsync();

            if (stationnement != null)
            {
                return (decimal)stationnement.Tarif;  
            }
            else
            {
                return 0.0m;
            }
        }


    }
    public interface StatEntreeSortie
    {
        Task<int> GetTotalParkingSpacesAsync();
        Task<int> GetTotalReservationsAsync();
        Task<int> GetAvailableSpacesAsync();
        Task<double> GetOccupiedPercentageAsync();
        Task<decimal> GetTarifActuel();
    }
}
