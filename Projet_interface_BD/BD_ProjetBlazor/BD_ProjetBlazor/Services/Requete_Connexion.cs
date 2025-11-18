using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

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
}
