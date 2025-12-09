using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using Stripe;
using Stripe.Checkout;

public class Requete_Achat_Reservation
{
    private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;
    private readonly AuthenticationStateProvider _auth;
    private readonly string stripeKey = "sk_test_xxxxx"; // Remplace par ta vraie clé

    public Requete_Achat_Reservation(
        IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory,
        AuthenticationStateProvider auth)
    {
        _dbContextFactory = dbContextFactory;
        _auth = auth;
        StripeConfiguration.ApiKey = stripeKey;
    }

    // --------------------------------------------------------
    // GET — Une seule réservation
    // --------------------------------------------------------
    public async Task<StationnementEntreeSortie?> GetReservationFromDb(int reservationId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.StationnementEntreeSorties
            .FirstOrDefaultAsync(r => r.EntreSortieStationnement == reservationId);
    }

    // --------------------------------------------------------
    // GET — Toutes les réservations (admin)
    // --------------------------------------------------------
    public async Task<List<StationnementEntreeSortie>> GetAllReservations()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.StationnementEntreeSorties
            .Where(r => r.Reservation == true)
            .ToListAsync();
    }

    // --------------------------------------------------------
    // GET — Réservations d’un utilisateur
    // --------------------------------------------------------
    public async Task<List<StationnementEntreeSortie>> GetReservationsByUserId(int userId)
    {
        if (userId <= 0)
            return new List<StationnementEntreeSortie>();

        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.StationnementEntreeSorties
            .Where(r => r.NumUtilisateur == userId && r.Reservation == true)
            .ToListAsync();
    }

    // --------------------------------------------------------
    // DELETE — Admin ou propriétaire
    // --------------------------------------------------------
    public async Task<bool> DeleteReservationAs(int reservationId, int userId, bool isAdmin)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var r = await db.StationnementEntreeSorties
            .FirstOrDefaultAsync(x => x.EntreSortieStationnement == reservationId);

        if (r == null)
            return false;

        // Si non admin → doit être le propriétaire
        if (!isAdmin && (r.NumUtilisateur == null || r.NumUtilisateur != userId))
            return false;

        db.StationnementEntreeSorties.Remove(r);
        await db.SaveChangesAsync();
        return true;
    }

    // --------------------------------------------------------
    // Stripe checkout
    // --------------------------------------------------------
    public async Task<string> CreateStripeCheckout(int reservationId)
    {
        var r = await GetReservationFromDb(reservationId);

        if (r == null)
            throw new Exception("Réservation introuvable.");

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },

            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "cad",
                        UnitAmountDecimal = r.PaiementSortie * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Réservation stationnement #{reservationId}"
                        }
                    },
                    Quantity = 1
                }
            },

            Mode = "payment",

            SuccessUrl = $"https://tonsite.ca/paiement/succes/{reservationId}",
            CancelUrl = $"https://tonsite.ca/paiement/erreur/{reservationId}"
        };

        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(options);

        await SavePaiement(reservationId, session.Id);

        return session.Url;
    }

    // --------------------------------------------------------
    // INSERT paiement
    // --------------------------------------------------------
    public async Task SavePaiement(int reservationId, string sessionId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        await db.Database.ExecuteSqlRawAsync(@"
            INSERT INTO paiement (reservationId, montant, stripeSessionId, statut, datePaiement)
            SELECT @Id, paiementSortie, @SessionId, 'pending', NULL
            FROM stationnementEntreeSortie
            WHERE entreSortieStationnement = @Id",
            new Microsoft.Data.SqlClient.SqlParameter("@Id", reservationId),
            new Microsoft.Data.SqlClient.SqlParameter("@SessionId", sessionId)
        );
    }

    // --------------------------------------------------------
    // UPDATE paiement (webhook Stripe)
    // --------------------------------------------------------
    public async Task UpdatePaiementStatus(int reservationId, string statut)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        await db.Database.ExecuteSqlRawAsync(@"
            UPDATE paiement
            SET statut = @Statut,
                datePaiement = CASE WHEN @Statut = 'paid' THEN GETDATE() END
            WHERE reservationId = @Id",
            new Microsoft.Data.SqlClient.SqlParameter("@Id", reservationId),
            new Microsoft.Data.SqlClient.SqlParameter("@Statut", statut)
        );
    }
    public async Task<bool> MarquerPaiementCommeRecu(int reservationId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var reservation = await db.StationnementEntreeSorties
            .FirstOrDefaultAsync(r => r.EntreSortieStationnement == reservationId);

        if (reservation == null)
            return false;

        reservation.PaiementRecu = true;

        await db.SaveChangesAsync();
        return true;
    }

}
