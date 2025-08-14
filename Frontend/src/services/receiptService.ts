import type { ReceiptDocumentCreateDto, 
    ReceiptDocumentFilter, 
    ReceiptDocumentReadDto,
    ReceiptDocumentUpdateDto } from "../types/receipt";
import api from "../utils/api";

export const receiptService = {
    fetchAll: (filters: ReceiptDocumentFilter) => {
        const params = new URLSearchParams();
        
        if (filters.dateFrom)
            params.append('dateFrom', filters.dateFrom);

        if (filters.dateTo)
            params.append('dateTo', filters.dateTo);

        if (filters.numId)
            filters.numId.forEach(id => params.append('numId', id));
        
        if (filters.resourceId)
            filters.resourceId.forEach(id => params.append('resourceId', id));

        if (filters.measureUnitId)
            filters.measureUnitId.forEach(id => params.append('measureUnitId', id));
        
        return api.get<ReceiptDocumentReadDto[]>(`/receipt`, { params: params });
    },

    create: (receiptDocument: Omit<ReceiptDocumentCreateDto, 'id'>) => 
        api.post<ReceiptDocumentCreateDto>('/receipt', receiptDocument),

    update: (id: string, receiptDocument: Partial<ReceiptDocumentUpdateDto>) => 
        api.put(`/receipt/${id}`, receiptDocument),

    getById: (id: string) => 
        api.get<ReceiptDocumentUpdateDto>(`/receipt/${id}`),

    delete: (id: string) =>
        api.delete(`/receipt/${id}`)
}