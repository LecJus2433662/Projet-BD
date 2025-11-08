using Microsoft.EntityFrameworkCore;
using System.Globalization;
using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
namespace BD_ProjetBlazor.Services
{
    public class Requete_inscriptions
    {
        private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;
        public Requete_inscriptions(IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task<List<Utilisateur>> GetInscriptions()
        {
             var _context = _dbContextFactory.CreateDbContextAsync().Result;
            var user =  from u in _context.Utilisateurs
                       select u;

            return user.ToListAsync().Result;
        }
        public async Task AjouterUtilisateurAsync(Utilisateur utilisateur)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            context.Utilisateurs.Add(utilisateur);
            await context.SaveChangesAsync();
        }
    }
}
