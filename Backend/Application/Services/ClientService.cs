using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System;

namespace Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ClientService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
        {
            var clients = await _unitOfWork.Clients.GetAllAsync();
            return _mapper.Map<IEnumerable<ClientDto>>(clients);
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsByStateAsync(int state)
        {
            var clients = await _unitOfWork.Clients.GetAllByStateAsync((State)state);
            return _mapper.Map<IEnumerable<ClientDto>>(clients);
        }

        public async Task<ClientDto?> GetClientByIdAsync(Guid id)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(id);
            return _mapper.Map<ClientDto>(client);
        }

        public async Task<Guid> CreateClientAsync(ClientCreateDto clientDto)
        {
            var client = _mapper.Map<Client>(clientDto);
            if (!await _unitOfWork.Clients.ExistsByNameAsync(client.Name))
            {
                client.State = State.Active;
                await _unitOfWork.Clients.AddAsync(client);
                await _unitOfWork.CommitAsync();
                return client.Id;
            }
            else
                throw new Exception("Имя уже существует");
        }

        public async Task UpdateClientAsync(ClientUpdateDto clientDto)
        {
            var client = _mapper.Map<Client>(clientDto);
            if (client == null) return;
            if (string.IsNullOrWhiteSpace(client.Name))
                throw new Exception("Имя не должно быть пустым");
            if (await _unitOfWork.Clients.ExistsByNameAndIdAsync(client.Name, client.Id))
                throw new Exception("Такое имя уже существует");
            _unitOfWork.Clients.Update(client);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteClientAsync(Guid id)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(id);
            if (client == null) return;
            if (await _unitOfWork.Clients.IsClientUsedAsync(id))
                throw new Exception("Клиент используется, его нельзя удалить");
            _unitOfWork.Clients.Delete(client);
            await _unitOfWork.CommitAsync();
        }
    }
}