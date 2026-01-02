namespace AedLocator.Models
{
    public class Aed
    {
        public int Id { get; set; }
        public string? Site { get; set; }
        public string? Organization { get; set; }
        public string? Division { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string? Placement { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
    }
}