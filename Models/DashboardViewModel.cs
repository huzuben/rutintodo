namespace todolist.Models
{
    public class DashboardViewModel
    {
        public List<Gorev> BekleyenGorevler { get; set; } = new();
        public List<Gorev> DevamEdenGorevler { get; set; } = new();
        public List<Gorev> TamamlananGorevler { get; set; } = new();
        public List<Gorev> GecikmisGorevler { get; set; } = new();
    }
} 