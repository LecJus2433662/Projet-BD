//using BD_ProjetBlazor.Data;
//using Microsoft.EntityFrameworkCore;

//public class UserAccountService
//{
//    private readonly ProgA25BdProjetProgContext _context;

//    public UserAccountService(ProgA25BdProjetProgContext context)
//    {
//        _context = context;
//    }

//    public async Task<string?> GetUsernameAsync(string courriel)
//    {
//        return await _context.Utilisateurs
//            .Where(u => u.Email == courriel)
//            .Select(u => u.Prenom)
//            .FirstOrDefaultAsync();
//    }
//}
