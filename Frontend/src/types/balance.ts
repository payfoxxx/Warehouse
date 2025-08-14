export interface BalanceReadDto {
    id: string,
    resourceName: string;
    resourceId: string;
    measureUnitName: string;
    measureUnitId: string;
    count: number;
}

export interface BalanceCreateDto {
    ResourceId: string;
    MeasureUnitId: string;
    Count: number;
}

export interface BalanceUpdateDto {
    Id: string;
    ResourceId: string;
    MeasureUnitId: string;
    Count: number;
}

export interface BalanceFilterDto {
    resourceId?: string[];
    measureUnitId?: string[];
}