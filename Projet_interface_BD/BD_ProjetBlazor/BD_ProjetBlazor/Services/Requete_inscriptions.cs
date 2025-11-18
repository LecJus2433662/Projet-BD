using Microsoft.EntityFrameworkCore;
using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;

namespace BD_ProjetBlazor.Services
{
    public class Requete_inscriptions
    {
        private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;

        public Requete_inscriptions(
            IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task AjouterUtilisateur(
            string email,
            byte[] motDePasse,
            string prenom,
            string nom,
            string ville,
            string pays)
        {
            var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var utilisateur = new Utilisateur
            {
                Email = email,
                MotDePasse = motDePasse,
                Sel = Guid.NewGuid(),
                Prenom = prenom,
                Nom = nom,
                Ville = ville,
                Pays = pays
            };

            dbContext.Utilisateurs.Add(utilisateur);
            await dbContext.SaveChangesAsync();
        }
    }
}
