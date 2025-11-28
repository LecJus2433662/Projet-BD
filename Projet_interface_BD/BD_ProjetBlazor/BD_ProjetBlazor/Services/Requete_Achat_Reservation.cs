using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Data;

public class Requete_Achat_Reservation
{
    private readonly IDbContextFactory<ProgA25BdProjetProgContext> _dbContextFactory;
    private readonly string stripeKey = "sk_test_xxxxx";

    public Requete_Achat_Reservation(IDbContextFactory<ProgA25BdProjetProgContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        StripeConfiguration.ApiKey = stripeKey;
    }

    // ---------------------------------------------------------
    // GET RESERVATION FROM DATABASE
    // ---------------------------------------------------------
    public async Task<ReservationDTO?> GetReservationFromDb(int reservationId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.StationnementEntreeSorties
            .Where(r => r.EntreSortieStationnement == reservationId)
            .Select(r => new ReservationDTO
            {
                ReservationId = r.EntreSortieStationnement,
                // Your model DOES NOT have NumStationnement
                // You can use NumVehicule or NumBarriere instead
                NumStationnement = r.NumVehicule ?? 0,

                DateEntree = r.DateEntree,
                DateSortie = r.DateSortie,
                Montant = r.PaiementSortie
            })
            .FirstOrDefaultAsync();
    }
    public async Task<List<ReservationDTO>> GetAllReservations()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.StationnementEntreeSorties
            .Select(r => new ReservationDTO
            {
                ReservationId = r.EntreSortieStationnement,
                NumStationnement = r.NumUtilisateur ?? 0,
                DateEntree = r.DateEntree,
                DateSortie = r.DateSortie,
                Montant = r.PaiementSortie
            })
            .ToListAsync();
    }

    // ---------------------------------------------------------
    // SAVE PAYMENT
    // ---------------------------------------------------------
    public async Task SavePaiement(int reservationId, string sessionId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var pId = new SqlParameter("@Id", reservationId);
        var sId = new SqlParameter("@SessionId", sessionId);

        await db.Database.ExecuteSqlRawAsync(@"
                INSERT INTO paiement (reservationId, montant, stripeSessionId, statut, datePaiement)
                SELECT @Id, paiementSortie, @SessionId, 'pending', NULL
                FROM stationnementEntreeSortie
                WHERE entreSortieStationnement = @Id",
            pId, sId);
    }

    // ---------------------------------------------------------
    // UPDATE PAYMENT STATUS
    // ---------------------------------------------------------
    public async Task UpdatePaiementStatus(int reservationId, string statut)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var pId = new SqlParameter("@Id", reservationId);
        var pStatut = new SqlParameter("@Statut", statut);

        await db.Database.ExecuteSqlRawAsync(@"
                UPDATE paiement
                SET statut = @Statut,
                    datePaiement = CASE WHEN @Statut = 'paid' THEN GETDATE() END
                WHERE reservationId = @Id",
            pId, pStatut);
    }

    // ---------------------------------------------------------
    // CREATE STRIPE CHECKOUT
    // ---------------------------------------------------------
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

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        await SavePaiement(reservationId, session.Id);

        return session.Url;
    }
}
