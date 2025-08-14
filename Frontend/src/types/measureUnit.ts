export interface MeasureUnitCreateDto {
    name: string;
}

export interface MeasureUnitReadDto {
    id: string,
    name: string;
}

export interface MeasureUnitUpdateDto {
    id: string;
    name: string;
    stateName: string;
    state: number;
}