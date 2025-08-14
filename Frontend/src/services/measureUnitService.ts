import type { MeasureUnitCreateDto, MeasureUnitReadDto, MeasureUnitUpdateDto } from "../types/measureUnit";
import api from "../utils/api";

export const measureUnitService = {
    fetchAll: (state: number) =>
        api.get<MeasureUnitReadDto[]>(`/measureUnit/${state}`),

    create: (measureUnit: Omit<MeasureUnitCreateDto, 'id'>) => 
        api.post<MeasureUnitCreateDto>('/measureUnit', measureUnit),

    update: (id: string, measureUnit: Partial<MeasureUnitUpdateDto>) => 
        api.put(`/measureUnit/${id}`, measureUnit),

    getById: (id: string) => 
        api.get<MeasureUnitUpdateDto>(`/measureUnit/${id}`),

    delete: (id: string) =>
        api.delete(`/measureUnit/${id}`)
}