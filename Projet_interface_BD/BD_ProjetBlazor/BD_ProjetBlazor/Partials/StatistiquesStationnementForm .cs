namespace BD_ProjetBlazor.Partials
{
    public class StatistiquesStationnementForm
    {
        public int StationnementId { get; set; }

        public int TotalParkingSpaces { get; set; }

        public int TotalReservations { get; set; }

        public int AvailableSpaces { get; set; }

        public double OccupiedPercentage { get; set; }

        public decimal TarifActuel { get; set; }
    }
}
