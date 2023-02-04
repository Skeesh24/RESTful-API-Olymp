namespace RESTful_API_Olymp.Domain.Entities
{
    public class AnimalEntity : BaseEntity
    {
        public string? Name { get; set; }

        public int Age { get; set; }

        public DateTime ChipedDate { get; set; }
    }
}