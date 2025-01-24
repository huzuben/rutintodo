namespace todolist.Models
{
    public class GorevIstatistik
    {
        public int ToplamGorev { get; set; }
        public int TamamlananGorev { get; set; }
        public double TamamlanmaOrani { get; set; }
        public int GecikmisTamamlanmayanGorev { get; set; }
        public Dictionary<string, int> OncelikDagilimi { get; set; } = new();
    }
} 