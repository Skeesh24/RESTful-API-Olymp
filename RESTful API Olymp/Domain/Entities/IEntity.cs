namespace RESTful_API_Olymp.Domain.Entities
{
    public interface IEntity
    {
        Guid Id { get; set; }

        DateTime CreatedDate { get; set; }

        bool IsActive { get; set; }
    }
}