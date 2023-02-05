namespace RESTful_API_Olymp.Domain.Entities
{
    public class LocationEntity : BaseEntity
    {
        public DateTime DateTimeOfVisitLocationPoint { get; set; }

        public long locationPointId { get; set; }
    }
}