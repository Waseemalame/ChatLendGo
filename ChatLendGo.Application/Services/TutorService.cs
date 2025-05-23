using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatLendGo.Application.DTOs;
using ChatLendGo.Application.Interfaces;
using ChatLendGo.Domain.Entities;
using ChatLendGo.Domain.Interfaces;

namespace ChatLendGo.Application.Services;

public class TutorService : ITutorService
{
    private readonly ITutorRepository _tutorRepository;

    public TutorService(ITutorRepository tutorRepository)
    {
        _tutorRepository = tutorRepository;
    }

    public async Task<IEnumerable<TutorDto>> GetAllTutorsAsync()
    {
        var tutors = await _tutorRepository.GetAllTutorsAsync();
        return tutors.Select(MapToDto);
    }

    public async Task<TutorDto> GetTutorByIdAsync(string id)
    {
        var tutor = await _tutorRepository.GetTutorByIdAsync(Guid.Parse(id));
        return tutor != null ? MapToDto(tutor) : null;
    }

    public async Task<TutorDto> CreateTutorAsync(TutorDto tutorDto)
    {
        var tutor = new Tutor
        {
            Id = Guid.NewGuid(),
            Name = tutorDto.Name,
            Description = tutorDto.Description,
            Specialty = Enum.Parse<Domain.Enums.TutorSpecialty>(tutorDto.Specialty),
            KnowledgeAreas = tutorDto.KnowledgeAreas.ToList(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdTutor = await _tutorRepository.CreateTutorAsync(tutor);
        return MapToDto(createdTutor);
    }

    public async Task UpdateTutorAsync(string id, TutorDto tutorDto)
    {
        var existingTutor = await _tutorRepository.GetTutorByIdAsync(Guid.Parse(id));
        if (existingTutor == null)
            throw new KeyNotFoundException($"Tutor with ID {id} not found");

        existingTutor.Name = tutorDto.Name;
        existingTutor.Description = tutorDto.Description;
        existingTutor.Specialty = Enum.Parse<Domain.Enums.TutorSpecialty>(tutorDto.Specialty);
        existingTutor.KnowledgeAreas = tutorDto.KnowledgeAreas.ToList();
        existingTutor.UpdatedAt = DateTime.UtcNow;

        await _tutorRepository.UpdateTutorAsync(existingTutor);
    }

    public async Task DeleteTutorAsync(string id)
    {
        await _tutorRepository.DeleteTutorAsync(Guid.Parse(id));
    }

    private TutorDto MapToDto(Tutor tutor)
    {
        return new TutorDto
        {
            Id = tutor.Id.ToString(),
            Name = tutor.Name,
            Description = tutor.Description,
            Specialty = tutor.Specialty.ToString(),
            KnowledgeAreas = tutor.KnowledgeAreas.ToArray()
        };
    }
}
