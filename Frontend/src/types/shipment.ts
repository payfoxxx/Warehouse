export interface ShipmentDocumentCreateDto {
    num: string;
    date: string;
    clientId: string;
    shipmentResources: ShipmentResourceCreateDto[];
}

export interface ShipmentResourceCreateDto {
    resourceId: string;
    measureUnitId: string;
    count: number;
}


export interface ShipmentDocumentReadDto {
    id: string;
    num: string;
    date: string;
    clientName: string;
    clientId: string;
    status: number;
    statusName: string;
    shipmentResources: ShipmentResourceReadDto[];
}

export interface ShipmentResourceReadDto {
    id: string;
    resourceName: string;
    resourceId: string;
    measureUnitName: string;
    measureUnitId: string;
    count: number;
}


export interface ShipmentDocumentUpdateDto {
    id: string;
    num: string;
    date: string;
    clientId: string;
    statusId: number;
    shipmentResources: ShipmentResourceUpdateDto[];
}

export interface ShipmentResourceUpdateDto {
    id: string;
    resourceId: string;
    measureUnitId: string;
    count: number;
}


export interface ShipmentDocumentFilter {
    dateFrom?: string;
    dateTo?: string;
    numId?: string[];
    clientId?: string[];
    resourceId?: string[];
    measureUnitId?: string[];
}