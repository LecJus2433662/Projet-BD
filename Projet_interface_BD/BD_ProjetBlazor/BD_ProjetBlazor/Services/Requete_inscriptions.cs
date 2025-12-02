using BD_ProjetBlazor.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BD_ProjetBlazor.Services
{
    public class Requete_inscriptions
    {
        private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;

        public Requete_inscriptions(IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory)
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
                await using var db = await _dbContextFactory.CreateDbContextAsync();

                // SQL parameters
                var paramNom = new SqlParameter("@nom", nom);
                var paramPrenom = new SqlParameter("@prenom", prenom);
                var paramVille = new SqlParameter("@ville", ville);
                var paramPays = new SqlParameter("@pays", pays);
                var paramEmail = new SqlParameter("@email", email);

                var paramMotDePasse = new SqlParameter("@motDePasseChiffre", SqlDbType.VarBinary)
                {
                    Value = motDePasse
                };

                var paramReponse = new SqlParameter
                {
                    ParameterName = "@reponse",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                // Execute stored procedure
                await db.Database.ExecuteSqlRawAsync(
                    @"EXEC ajout_utilisateur 
                        @nom, @prenom, @ville, @pays, 
                        @email, @motDePasseChiffre, @reponse OUTPUT",
                    paramNom,
                    paramPrenom,
                    paramVille,
                    paramPays,
                    paramEmail,
                    paramMotDePasse,
                    paramReponse
                );

                // Return stored procedure output
                if (paramReponse.Value is int valeur)
                    return valeur;

                return -1; // Should never happen
            }
            catch (Exception ex) when (IsUniqueConstraintViolation(ex))
            {
                // Violations of unique email index
                return -2;
            }
            catch
            {
                // Other SQL/server errors
                return -1;
            }
        }

        // Helper: Detect SQL Server unique constraint errors
        private static bool IsUniqueConstraintViolation(Exception ex)
        {
            return ex is SqlException sqlEx &&
                   (sqlEx.Number == 2627 || sqlEx.Number == 2601);
        }
    }
}
