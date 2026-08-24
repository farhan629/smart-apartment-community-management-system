/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { AvailableSlotsResponseDto } from '../models/AvailableSlotsResponseDto';
import type { CreateSlotsBulkRequestDto } from '../models/CreateSlotsBulkRequestDto';
import type { MessageResponseDto } from '../models/MessageResponseDto';
import type { SlotListResponseDto } from '../models/SlotListResponseDto';
import type { SlotsBulkResponseDto } from '../models/SlotsBulkResponseDto';
import type { UpdateSlotRequestDto } from '../models/UpdateSlotRequestDto';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
    providedIn: 'root',
})
export class SlotService {
    constructor(public readonly http: HttpClient) {}
    /**
     * @param amenityId
     * @param pageNumber
     * @param pageSize
     * @returns SlotListResponseDto Success
     * @throws ApiError
     */
    public getApiAmenitiesSlots(
        amenityId: string,
        pageNumber: number = 1,
        pageSize: number = 10,
    ): Observable<SlotListResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/amenities/{amenityId}/slots',
            path: {
                'amenityId': amenityId,
            },
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
        });
    }
    /**
     * @param amenityId
     * @param date
     * @param pageNumber
     * @param pageSize
     * @returns AvailableSlotsResponseDto Success
     * @throws ApiError
     */
    public getApiAmenitiesSlotsAvailable(
        amenityId: string,
        date?: string,
        pageNumber: number = 1,
        pageSize: number = 10,
    ): Observable<AvailableSlotsResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/amenities/{amenityId}/slots/available',
            path: {
                'amenityId': amenityId,
            },
            query: {
                'date': date,
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
        });
    }
    /**
     * @param amenityId
     * @param requestBody
     * @returns SlotsBulkResponseDto Success
     * @throws ApiError
     */
    public postApiAmenitiesSlotsBulk(
        amenityId: string,
        requestBody?: CreateSlotsBulkRequestDto,
    ): Observable<SlotsBulkResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'POST',
            url: '/amenities/{amenityId}/slots/bulk',
            path: {
                'amenityId': amenityId,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param slotId
     * @param requestBody
     * @returns MessageResponseDto Success
     * @throws ApiError
     */
    public putApiSlots(
        slotId: string,
        requestBody?: UpdateSlotRequestDto,
    ): Observable<MessageResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'PUT',
            url: '/slots/{slotId}',
            path: {
                'slotId': slotId,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param slotId
     * @returns MessageResponseDto Success
     * @throws ApiError
     */
    public deleteApiSlots(
        slotId: string,
    ): Observable<MessageResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'DELETE',
            url: '/slots/{slotId}',
            path: {
                'slotId': slotId,
            },
        });
    }
}
