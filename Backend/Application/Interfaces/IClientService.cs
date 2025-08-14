using System;
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllClientsAsync();
        Task<IEnumerable<ClientDto>> GetAllClientsByStateAsync(int state);
        Task<ClientDto?> GetClientByIdAsync(Guid id);
        Task<Guid> CreateClientAsync(ClientCreateDto clientDto);
        Task UpdateClientAsync(ClientUpdateDto clientDto);
        Task DeleteClientAsync(Guid id);
    }
}