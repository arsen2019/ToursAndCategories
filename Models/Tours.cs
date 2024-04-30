namespace ToursAndCategories.Models
{
    public class Tour
    {
        private int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; }= DateTime.Now.AddDays(10);
    }
}
