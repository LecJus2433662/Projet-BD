using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

public class Requete_inscriptions
{
    private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;

    public Requete_inscriptions(IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<Utilisateur>> GetInscriptions()
    {
        await using var _context = await _dbContextFactory.CreateDbContextAsync();
        return await _context.Utilisateurs.ToListAsync();
    }

    public async Task AjouterUtilisateur(Utilisateur utilisateur, string motDePasse)
    {
        await using var _context = await _dbContextFactory.CreateDbContextAsync();

        // Générer un sel aléatoire
        utilisateur.Sel = Guid.NewGuid();

        // Convertir le mot de passe en byte[] (BINARY(64)) avec SHA-256 + sel
        using var sha256 = SHA256.Create();
        byte[] passwordBytes = Encoding.UTF8.GetBytes(motDePasse + utilisateur.Sel.ToString());
        utilisateur.MotDePasse = sha256.ComputeHash(passwordBytes);

        _context.Utilisateurs.Add(utilisateur);
        await _context.SaveChangesAsync();
    }
}
