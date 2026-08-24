/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { AvailableSlotResponseDto } from './AvailableSlotResponseDto';
import type { PaginationDto } from './PaginationDto';
export type AvailableSlotsResponseDto = {
    amenityId?: string;
    amenityName?: string | null;
    slotType?: string | null;
    location?: string | null;
    rules?: string | null;
    imageUrl?: string | null;
    slots?: Array<AvailableSlotResponseDto> | null;
    pagination?: PaginationDto;
};

