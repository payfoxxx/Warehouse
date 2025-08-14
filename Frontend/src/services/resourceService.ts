import type { ResourceCreateDto, ResourceReadDto, ResourceUpdateDto } from "../types/resource";
import api from "../utils/api";

export const resourceService = {
    fetchAll: (state: number) =>
        api.get<ResourceReadDto[]>(`/resource/${state}`),

    create: (resource: Omit<ResourceCreateDto, 'id'>) => 
        api.post<ResourceCreateDto>('/resource', resource),

    update: (id: string, resource: Partial<ResourceUpdateDto>) => 
        api.put(`/resource/${id}`, resource),

    getById: (id: string) => 
        api.get<ResourceUpdateDto>(`/resource/${id}`),

    delete: (id: string) =>
        api.delete(`/resource/${id}`)
}