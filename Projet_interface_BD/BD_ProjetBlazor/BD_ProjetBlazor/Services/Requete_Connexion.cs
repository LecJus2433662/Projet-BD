using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BD_ProjetBlazor.Partials;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data;

public class Requete_Connexion
{
    private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;

    public Requete_Connexion(IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Utilisateur?> VerifierConnexion(string courriel, string motDePasse)
    {
        await using var _context = await _dbContextFactory.CreateDbContextAsync();
        // Cherche l'utilisateur par courriel
        var utilisateur = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.Email == courriel);

        if (utilisateur == null)
            return null;
        // Connexion réussie
        return utilisateur;
    }
    public async Task<int> ConnecterUtilisateur(ProgA25BdProjetProgContext db, string email, string motPasse)
    {
        await using var _context = await _dbContextFactory.CreateDbContextAsync();

        var emailParam = new SqlParameter("@Email", email);
        var mdp = new SqlParameter("@MotDePasse", motPasse);
        var reponseParam = new SqlParameter("@Reponse", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        await db.Database.ExecuteSqlRawAsync(
            "EXEC ConnexionUtilisateur @Email, @MotDePasse, @Reponse OUTPUT",
            emailParam, mdp, reponseParam
            );

        return reponseParam.Value == DBNull.Value ? -1 : (int)reponseParam.Value;
    }
}
