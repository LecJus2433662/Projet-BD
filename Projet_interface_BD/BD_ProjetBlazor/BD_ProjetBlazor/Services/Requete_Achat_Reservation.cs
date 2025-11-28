using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

public class Requete_Achat_Reservation
{
    private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;
    private readonly AuthenticationStateProvider _auth;
    private readonly string stripeKey = "sk_test_xxxxx"; // remplace par ta clé Stripe

    public Requete_Achat_Reservation(
        IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory,
        AuthenticationStateProvider auth)
    {
        _dbContextFactory = dbContextFactory;
        _auth = auth;
        StripeConfiguration.ApiKey = stripeKey;
    }

    // -------------------------
    // Helpers : récupérer userId et admin
    // -------------------------
    private int GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var claim = user.FindFirst("noUtilisateur")?.Value
                 ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(claim, out int id))
            return id;

        return 0; // fallback
    }

    private bool IsUserAdmin(ClaimsPrincipal user)
    {
        var isAdminClaim = user.FindFirst("IsAdmin")?.Value;
        if (!string.IsNullOrEmpty(isAdminClaim))
        {
            if (bool.TryParse(isAdminClaim, out bool b)) return b;
            if (int.TryParse(isAdminClaim, out int i)) return i != 0;
        }
        return false;
    }

    // -------------------------
    // Récupérer les réservations
    // -------------------------
    public async Task<List<ReservationDTO>> GetReservationsForUser()
    {
        var state = await _auth.GetAuthenticationStateAsync();
        var user = state.User;

        bool isAdmin = IsUserAdmin(user);
        int userId = GetUserIdFromClaims(user);

        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var query = db.StationnementEntreeSorties.AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(r => r.NumUtilisateur == userId && r.Reservation == true);
        }
        else
        {
            query = query.Where(r => r.Reservation == true);
        }

        return await query
            .OrderByDescending(r => r.DateEntree)
            .Select(r => new ReservationDTO
            {
                ReservationId = r.EntreSortieStationnement,
                DateEntree = r.DateEntree,
                DateSortie = r.DateSortie,
                Montant = r.PaiementSortie,
                NumStationnement = r.NumStationnement ?? 0,
                UtilisateurId = r.NumUtilisateur ?? 0
            })
            .ToListAsync();
    }

    // -------------------------
    // Récupérer une réservation par ID
    // -------------------------
    public async Task<ReservationDTO?> GetReservationFromDb(int reservationId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.StationnementEntreeSorties
            .Where(r => r.EntreSortieStationnement == reservationId)
            .Select(r => new ReservationDTO
            {
                ReservationId = r.EntreSortieStationnement,
                DateEntree = r.DateEntree,
                DateSortie = r.DateSortie,
                Montant = r.PaiementSortie,
                NumStationnement = r.NumStationnement ?? 0,
                UtilisateurId = r.NumUtilisateur ?? 0
            })
            .FirstOrDefaultAsync();
    }

    // -------------------------
    // Supprimer réservation
    // -------------------------
    public async Task<bool> DeleteReservation(int reservationId)
    {
        var state = await _auth.GetAuthenticationStateAsync();
        var user = state.User;

        bool isAdmin = IsUserAdmin(user);
        int userId = GetUserIdFromClaims(user);

        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var reservation = await db.StationnementEntreeSorties
            .FirstOrDefaultAsync(r => r. EntreSortieStationnement == reservationId);

        if (reservation == null)
            return false;

        if (!isAdmin && (reservation.NumUtilisateur == null || reservation.NumUtilisateur != userId))
            return false;

        db.StationnementEntreeSorties.Remove(reservation);
        await db.SaveChangesAsync();
        return true;
    }

    // -------------------------
    // Stripe Checkout
    // -------------------------
    public async Task<string> CreateStripeCheckout(int reservationId)
    {
        var reservation = await GetReservationFromDb(reservationId);

        if (reservation == null)
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
                        UnitAmountDecimal = reservation.Montant * 100,
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

    // -------------------------
    // Enregistrer le paiement
    // -------------------------
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

    // -------------------------
    // Mettre à jour le statut paiement
    // -------------------------
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
}
