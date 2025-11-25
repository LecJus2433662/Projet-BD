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
        var db = await _dbContextFactory.CreateDbContextAsync();
        int? noUtilisateur = await ConnecterUtilisateur(db, courriel, motDePasse);
        return GetUtilisateur(db, noUtilisateur);
    }
    public async Task<int> ConnecterUtilisateur(ProgA25BdProjetProgContext db, string email, string motPasse)
    {
        var emailParam = new SqlParameter("@Email", email);
        var mdp = new SqlParameter("@MotDePasse", motPasse);
        var reponseParam = new SqlParameter("@Resultat", SqlDbType.Int);
        reponseParam.Direction = ParameterDirection.Output;

        await db.Database.ExecuteSqlRawAsync(
            "EXEC ConnexionUtilisateur @Email, @MotDePasse, @Resultat OUTPUT",
            emailParam, mdp, reponseParam
            );

        return reponseParam.Value == DBNull.Value ? -1 : (int)reponseParam.Value;
    }
    private static Utilisateur? GetUtilisateur(ProgA25BdProjetProgContext db, int? noUtilisateur)
    {
        Utilisateur? utilisateurRetour = db.Utilisateurs
            .FirstOrDefault(u => u.NoUtilisateur == noUtilisateur);

        return utilisateurRetour;
    }

}
