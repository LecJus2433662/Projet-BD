namespace BD_ProjetBlazor.Partials
{
    public class ReservationForm
    {
        public int NumStationnement { get; set; }
        public DateOnly dateEntree { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public DateOnly dateSortie { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public decimal? PaiementSortie { get; set; }

    }
}
