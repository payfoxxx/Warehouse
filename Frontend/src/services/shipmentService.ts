import type { ShipmentDocumentCreateDto, 
    ShipmentDocumentFilter, 
    ShipmentDocumentReadDto,
    ShipmentDocumentUpdateDto } from "../types/shipment";
import api from "../utils/api";

export const shipmentService = {
    fetchAll: (filters: ShipmentDocumentFilter) => {
        const params = new URLSearchParams();

        if (filters.dateFrom)
            params.append('dateFrom', filters.dateFrom);

        if (filters.dateTo)
            params.append('dateTo', filters.dateTo);

        if (filters.numId)
            filters.numId.forEach(id => params.append('numId', id));
        
        if (filters.clientId)
            filters.clientId.forEach(id => params.append('clientId', id));
        
        if (filters.resourceId)
            filters.resourceId.forEach(id => params.append('resourceId', id));

        if (filters.measureUnitId)
            filters.measureUnitId.forEach(id => params.append('measureUnitId', id));

        return api.get<ShipmentDocumentReadDto[]>(`/shipment`, { params: params });
    },

    create: (shipmentDocument: Omit<ShipmentDocumentCreateDto, 'id'>) => 
        api.post<ShipmentDocumentCreateDto>('/shipment', shipmentDocument),

    update: (id: string, shipmentDocument: Partial<ShipmentDocumentUpdateDto>) => 
        api.put(`/shipment/${id}`, shipmentDocument),

    getById: (id: string) => 
        api.get<ShipmentDocumentUpdateDto>(`/shipment/${id}`),

    delete: (id: string) =>
        api.delete(`/shipment/${id}`),

    sign: (id: string, shipmentDocument: Partial<ShipmentDocumentUpdateDto>) => 
        api.post(`/shipment/${id}/sign`, shipmentDocument),

    revoke: (id: string) => 
        api.post(`/shipment/${id}/revoke`)
}