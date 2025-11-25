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
                await using var db = _dbContextFactory.CreateDbContext();

                var paramNom = new SqlParameter("@nom", nom);
                var paramPrenom = new SqlParameter("@prenom", prenom);
                var paramVille = new SqlParameter("@ville", ville);
                var paramPays = new SqlParameter("@pays", pays);
                var paramEmail = new SqlParameter("@email", email);
                var paramMotDePasse = new SqlParameter("@motDePasseChiffre", motDePasse);

                var paramReponse = new SqlParameter
                {
                    ParameterName = "@reponse",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                await db.Database.ExecuteSqlRawAsync(
                    "EXEC ajout_utilisateur @nom, @prenom, @ville, @pays, @email, @motDePasseChiffre, @reponse OUTPUT",
                    paramNom, paramPrenom, paramVille, paramPays, paramEmail, paramMotDePasse, paramReponse
                );

                // Si la procédure retourne NULL (rare mais possible), on traite comme erreur
                return paramReponse.Value is int valeur ? valeur : -1;
            }
            catch (Exception ex) when (IsUniqueConstraintViolation(ex))
            {
                // SQL Server violation d'unicité sur la colonne Email
                return 0; // ← On retourne 0 = email déjà utilisé
            }
            catch
            {
                return -1; // Autre erreur inconnue
            }
        }

        // Méthode helper pour détecter les erreurs de doublon SQL Server
        private static bool IsUniqueConstraintViolation(Exception ex)
        {
            return ex is Microsoft.Data.SqlClient.SqlException sqlEx &&
                   (sqlEx.Number == 2627 || sqlEx.Number == 2601); // 2627 = violation PK/UK, 2601 = violation index unique
        }
    }
}
