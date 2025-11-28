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

            var raw = await _context.StationnementEntreeSorties
                .GroupBy(s => new
                {
                    Annee = s.DateSortie.Year,
                    Mois = s.DateSortie.Month
                })
                .Select(g => new
                {
                    g.Key.Annee,
                    g.Key.Mois,
                    TotalArgent = g.Sum(x => x.PaiementSortie),
                    TotalPersonnes = g.Count(),
                    JoursActifs = g.Select(x => x.DateSortie).Distinct().Count()
                })
                .OrderBy(x => x.Annee)
                .ThenBy(x => x.Mois)
                .ToListAsync();
            var result = raw.Select(r => new StatMensuelle
            {
                Annee = r.Annee,
                Mois = r.Mois,
                NomMois = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(r.Mois),
                TotalArgent = r.TotalArgent,
                NbJoursActifs = r.JoursActifs,
                MoyenneArgentParJour = r.JoursActifs == 0 ? 0 : r.TotalArgent / r.JoursActifs,
                TotalPersonnes = r.TotalPersonnes,
                MoyennePersonnesParJour = r.JoursActifs == 0 ? 0 : (double)r.TotalPersonnes / r.JoursActifs
            })
           .ToList();

            return result;
        }
    }
}