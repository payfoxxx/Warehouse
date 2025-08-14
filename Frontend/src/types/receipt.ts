export interface ReceiptDocumentCreateDto {
    num: string;
    date: string;
    receiptResources: ReceiptResourceCreateDto[];
}

export interface ReceiptResourceCreateDto {
    resourceId: string;
    measureUnitId: string;
    count: number;
}


export interface ReceiptDocumentReadDto {
    id: string;
    num: string;
    date: string;
    receiptResources: ReceiptResourceReadDto[];
}

export interface ReceiptResourceReadDto {
    id: string;
    resourceName: string;
    resourceId: string;
    measureUnitName: string;
    measureUnitId: string;
    count: number;
}


export interface ReceiptDocumentUpdateDto {
    id: string;
    num: string;
    date: string;
    receiptResources: ReceiptResourceUpdateDto[];
}

export interface ReceiptResourceUpdateDto {
    id: string;
    resourceId: string;
    measureUnitId: string;
    count: number;
}


export interface ReceiptDocumentFilter {
    dateFrom?: string;
    dateTo?: string;
    numId?: string[];
    resourceId?: string[];
    measureUnitId?: string[];
}