using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
        public async Task<int> AjouterUtilisateur(
            string email,
            byte[] motDePasse,
            string prenom,
            string nom,
            string ville,
            string pays)
        {
            try
            {
                var db = _dbContextFactory.CreateDbContext();
                var paramNom = new SqlParameter("@nom", nom);
                var paramPrenom = new SqlParameter("@prenom", prenom);
                var paramVille = new SqlParameter("@ville", ville);
                var paramPays = new SqlParameter("@pays", pays);
                var paramEmail = new SqlParameter("@email", email);
                var paramMotDePasse = new SqlParameter("@motDePasseChiffre", motDePasse);

                var paramReponse = new SqlParameter("@reponse", SqlDbType.Int);
                paramReponse.Direction = ParameterDirection.Output;

                await db.Database.ExecuteSqlRawAsync(
                    "exec ajout_utilisateur  @nom, @prenom, @ville, @pays, @email, @motDePasseChiffre, @reponse OUTPUT",
                 
                    paramNom,
                    paramPrenom,
                    paramVille,
                    paramPays,
                    paramEmail,
                    paramMotDePasse,
                    paramReponse
                );
                return (int)paramReponse.Value;
            }catch
            {
                return -1;
            }



        }
    }
}
