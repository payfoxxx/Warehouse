import type { BalanceCreateDto, 
    BalanceFilterDto, 
    BalanceReadDto,
    BalanceUpdateDto } from "../types/balance";
import api from "../utils/api";

export const balanceService = {
    fetchAll: (filters: BalanceFilterDto) => {
        const params = new URLSearchParams();
        if (filters.resourceId)
            filters.resourceId.forEach(id => params.append('resourceId', id));

        if (filters.measureUnitId)
            filters.measureUnitId.forEach(id => params.append('measureUnitId', id));
        return api.get<BalanceReadDto[]>(`/balance`, { params: params });
    },

    create: (balance: Omit<BalanceCreateDto, 'id'>) => 
        api.post<BalanceCreateDto>('/balance', balance),

    update: (id: string, balance: Partial<BalanceUpdateDto>) => 
        api.put(`/balance/${id}`, balance),

    getById: (id: string) => 
        api.get<BalanceReadDto>(`/balance/${id}`),

    delete: (id: string) =>
        api.delete(`/balance/${id}`)
}