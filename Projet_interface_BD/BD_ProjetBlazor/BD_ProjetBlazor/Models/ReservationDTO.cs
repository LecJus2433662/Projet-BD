namespace BD_ProjetBlazor.Models
{
    public class ReservationDTO
    {
        public int ReservationId { get; set; }
        public int NumStationnement { get; set; }
        public DateOnly DateEntree { get; set; }
        public DateOnly DateSortie { get; set; }
        public decimal Montant { get; set; }
    }
}
