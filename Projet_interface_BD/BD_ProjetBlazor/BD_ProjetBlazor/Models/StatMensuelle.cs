namespace BD_ProjetBlazor.Models
{
    public class StatMensuelle
    {
        public int Annee { get; set; }
        public int Mois { get; set; }
        public string NomMois { get; set; }
        public decimal TotalArgent { get; set; }
        public int NbJoursActifs { get; set; }
        public decimal MoyenneArgentParJour { get; set; }
        public int TotalPersonnes { get; set; }
        public double MoyennePersonnesParJour { get; set; }
    }
}
