namespace ChatLendGo.Domain.Interfaces;

using ChatLendGo.Domain.Entities;

public interface ITutorRepository
{
    Task<List<Tutor>> GetAllTutorsAsync();
    Task<Tutor> GetTutorByIdAsync(Guid id);
    Task<Tutor> CreateTutorAsync(Tutor tutor);
    Task UpdateTutorAsync(Tutor tutor);
    Task DeleteTutorAsync(Guid id);
}
