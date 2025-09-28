namespace BlazorFurniture.Domain.Entities.Behaviours;

public interface ITrackable
{
    DateTime CreatedAt { get; set; }
    DateTime? ModifiedAt { get; set; }
    string CreatedBy { get; set; }
    string? ModifiedBy { get; set; }
}
