export interface ResourceCreateDto {
    name: string;
}

export interface ResourceReadDto {
    id: string,
    name: string;
}

export interface ResourceUpdateDto {
    id: string;
    name: string;
    stateName: string;
    state: number;
}