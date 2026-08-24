/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { AmenityListResponseDto } from '../models/AmenityListResponseDto';
import type { AmenityResponseDto } from '../models/AmenityResponseDto';
import type { CreateAmenityRequestDto } from '../models/CreateAmenityRequestDto';
import type { IdResponseDto } from '../models/IdResponseDto';
import type { MessageResponseDto } from '../models/MessageResponseDto';
import type { UpdateAmenityRequestDto } from '../models/UpdateAmenityRequestDto';
import type { UploadImageResponseDto } from '../models/UploadImageResponseDto';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
    providedIn: 'root',
})
export class AmenityService {
    constructor(public readonly http: HttpClient) {}
    /**
     * @param pageNumber
     * @param pageSize
     * @param searchName
     * @param slotType
     * @returns AmenityListResponseDto Success
     * @throws ApiError
     */
    public getApiAmenity(
        pageNumber: number = 1,
        pageSize: number = 10,
        searchName?: string,
        slotType?: string,
    ): Observable<AmenityListResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/amenity',
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
                'searchName': searchName,
                'slotType': slotType,
            },
        });
    }
    /**
     * @param requestBody
     * @returns IdResponseDto Success
     * @throws ApiError
     */
    public postApiAmenity(
        requestBody?: CreateAmenityRequestDto,
    ): Observable<IdResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'POST',
            url: '/amenity',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @returns AmenityResponseDto Success
     * @throws ApiError
     */
    public getApiAmenity1(
        id: string,
    ): Observable<AmenityResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/amenity/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns MessageResponseDto Success
     * @throws ApiError
     */
    public putApiAmenity(
        id: string,
        requestBody?: UpdateAmenityRequestDto,
    ): Observable<MessageResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'PUT',
            url: '/amenity/{id}',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @returns MessageResponseDto Success
     * @throws ApiError
     */
    public deleteApiAmenity(
        id: string,
    ): Observable<MessageResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'DELETE',
            url: '/amenity/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param formData
     * @returns UploadImageResponseDto Success
     * @throws ApiError
     */
    public postApiAmenityUpload(
        formData?: {
            file?: Blob;
        },
    ): Observable<UploadImageResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'POST',
            url: '/amenity/upload',
            formData: formData,
            mediaType: 'multipart/form-data',
        });
    }
}
