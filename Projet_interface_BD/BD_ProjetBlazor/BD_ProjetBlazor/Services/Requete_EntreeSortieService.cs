using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Partials;
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

        public async Task<List<StatistiquesStationnementForm>> GetAllStationnementsStatsAsync()
        {
            using var _context = _dbContextFactory.CreateDbContext();

            var stationnements = await _context.Stationnements.ToListAsync();

            var statsList = new List<StatistiquesStationnementForm>();

            foreach (var s in stationnements)
            {
                var totalReservations = _context.StationnementEntreeSorties
                    .Count(r => r.Reservation == true && r.NumStationnement == s.NumStationnement);

                var availableSpaces = s.NombrePlaceMax - totalReservations;

                statsList.Add(new StatistiquesStationnementForm
                {
                    StationnementId = s.NumStationnement,
                    TotalParkingSpaces = s.NombrePlaceMax,
                    TotalReservations = totalReservations,
                    AvailableSpaces = availableSpaces,
                    OccupiedPercentage = s.NombrePlaceMax == 0 ? 0 :
                        (double)totalReservations / s.NombrePlaceMax * 100,
                    TarifActuel = s.Tarif
                });
            }

            return statsList;
        }

    }
}
