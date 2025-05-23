using ChatLendGo.Application.DTOs;

namespace ChatLendGo.Application.Interfaces;

public interface ITutorService
{
    Task<IEnumerable<TutorDto>> GetAllTutorsAsync();
    Task<TutorDto> GetTutorByIdAsync(string id);
    Task<TutorDto> CreateTutorAsync(TutorDto tutorDto);
    Task UpdateTutorAsync(string id, TutorDto tutorDto);
    Task DeleteTutorAsync(string id);
}
