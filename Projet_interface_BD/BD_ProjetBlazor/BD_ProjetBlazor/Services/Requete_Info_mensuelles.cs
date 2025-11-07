using Microsoft.EntityFrameworkCore;
using System.Globalization;
using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;

namespace BD_ProjetBlazor.Services
{
    public class Requete_Info_mensuelles
    {
        private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;

        public Requete_Info_mensuelles(IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<StatMensuelle>> GetInfo_mensuelles()
        {
            using var _context = _dbContextFactory.CreateDbContext();

            var stats = await _context.StationnementEntreeSorties
                .GroupBy(s => new
                {
                    Annee = s.DateSortie.Year,
                    Mois = s.DateSortie.Month
                })
                .Select(g => new StatMensuelle
                {
                    Annee = g.Key.Annee,
                    Mois = g.Key.Mois,
                    NomMois = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Mois),
                    TotalArgent = g.Sum(x => x.PaiementSortie),
                    NbJoursActifs = g.Select(x => x.DateSortie).Distinct().Count(),
                    MoyenneArgentParJour = g.Sum(x => x.PaiementSortie) / g.Select(x => x.DateSortie).Distinct().Count(),
                    TotalPersonnes = g.Count(),
                    MoyennePersonnesParJour = (double)g.Count() / g.Select(x => x.DateSortie).Distinct().Count()
                })
                .OrderBy(x => x.Annee)
                .ThenBy(x => x.Mois)
                .ToListAsync();

            return stats;
        }
    }

    public class StatMensuelle
    {
        public int Annee { get; set; }
        public int Mois { get; set; }
        public string NomMois { get; set; }
        public decimal TotalArgent { get; set; }
        public int NbJoursActifs { get; set; }
        public decimal MoyenneArgentParJour { get; set; }
        public int TotalPersonnes { get; set; }
        public double MoyennePersonnesParJour { get; set; }
    }
}
