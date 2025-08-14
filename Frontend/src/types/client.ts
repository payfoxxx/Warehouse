export interface ClientCreateDto {
    name: string;
    address: string;
}

export interface ClientReadDto {
    id: string,
    name: string;
    address: string;
}

export interface ClientUpdateDto {
    id: string;
    name: string;
    address: string;
    stateName: string;
    state: number;
}