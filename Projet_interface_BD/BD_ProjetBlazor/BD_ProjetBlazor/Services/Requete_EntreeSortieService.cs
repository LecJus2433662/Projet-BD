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
                var totalPresent = _context.StationnementEntreeSorties
                    .Count(r => r.NumStationnement == s.NumStationnement
                    && r.DateEntree == DateOnly.FromDateTime(DateTime.Today)
                    && (r.DateSortie == null || r.DateSortie == DateOnly.FromDateTime(DateTime.Today)));


                var available = s.NombrePlaceMax - totalPresent;

                statsList.Add(new StatistiquesStationnementForm
                {
                    StationnementId = s.NumStationnement,
                    TotalParkingSpaces = s.NombrePlaceMax,
                    TotalReservations = totalPresent, // tu peux renommer si tu veux
                    AvailableSpaces = available,
                    OccupiedPercentage = s.NombrePlaceMax == 0 ? 0 :
                        (double)totalPresent / s.NombrePlaceMax * 100,
                    TarifActuel = s.Tarif
                });
            }
            return statsList;
        }

    }

}

