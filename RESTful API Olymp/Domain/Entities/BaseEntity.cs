namespace RESTful_API_Olymp.Domain.Entities
{
    public class BaseEntity : IEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }
    }
}