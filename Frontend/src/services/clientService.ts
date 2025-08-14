import type { ClientCreateDto, ClientReadDto, ClientUpdateDto } from "../types/client";
import api from "../utils/api";

export const clientService = {
    fetchAll: (state: number) =>
        api.get<ClientReadDto[]>(`/client/${state}`),

    create: (client: Omit<ClientCreateDto, 'id'>) => 
        api.post<ClientCreateDto>('/client', client),

    update: (id: string, client: Partial<ClientUpdateDto>) => 
        api.put(`/client/${id}`, client),

    getById: (id: string) => 
        api.get<ClientUpdateDto>(`/client/${id}`),

    delete: (id: string) =>
        api.delete(`/client/${id}`)
}