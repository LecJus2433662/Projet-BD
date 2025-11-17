using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Cryptography;
using System.Text;

public class Requete_inscriptions
{
    private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;

    public Requete_inscriptions(IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<Utilisateur>> GetInscriptions(string email)
    {
       var dbContext = _dbContextFactory.CreateDbContextAsync().Result;
        var user = from u in dbContext.Utilisateurs
                   where u.Email == email
                  select u;
        return await dbContext.Utilisateurs.ToListAsync();
    }
    public async Task<List<Utilisateur>> InsertUser()
    {
        var dbContext = _dbContextFactory.CreateDbContextAsync().Result;
        
                   
    }

    
}
