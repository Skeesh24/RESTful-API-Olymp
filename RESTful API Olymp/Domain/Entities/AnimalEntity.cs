using System.ComponentModel;

namespace RESTful_API_Olymp.Domain.Entities
{
    public class AnimalEntity : BaseEntity
    {
        public long[] AnimalTypes { get; set; }

        public float Weight { get; set; }

        public float Length { get; set; }

        public float Height { get; set; }

        public string Gender { get; set; }

        public string LifeStatus { get; set; }

        public DateTime ChippingDateTime { get; set; }

        public int ChipperId { get; set; }

        public long ChippingLocationId { get; set; }

        public long[] VisitingLocations { get; set; }

        public DateTime? DeathDateTime { get; set; }
    }
}

