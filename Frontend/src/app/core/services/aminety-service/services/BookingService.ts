/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { BookingListResponseDto } from '../models/BookingListResponseDto';
import type { CreateBookingRequestDto } from '../models/CreateBookingRequestDto';
import type { IdResponseDto } from '../models/IdResponseDto';
import type { MessageResponseDto } from '../models/MessageResponseDto';
import type { ReportResponseDto } from '../models/ReportResponseDto';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
    providedIn: 'root',
})
export class BookingService {
    constructor(public readonly http: HttpClient) {}
    /**
     * @param status
     * @param fromDate
     * @param toDate
     * @param pageNumber
     * @param pageSize
     * @returns BookingListResponseDto Success  
     * @throws ApiError
     */
    public getApiBooking(
        status?: string,
        fromDate?: string,
        toDate?: string,
        pageNumber: number = 1,
        pageSize: number = 10,
    ): Observable<BookingListResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/booking',
            query: {
                'status': status,
                'fromDate': fromDate,
                'toDate': toDate,
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
        });
    }
    /**
     * @param requestBody
     * @returns IdResponseDto Success
     * @throws ApiError
     */
    public postApiBooking(
        requestBody?: CreateBookingRequestDto,
    ): Observable<IdResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'POST',
            url: '/booking',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param bookingId
     * @param cancellationReason
     * @returns MessageResponseDto Success
     * @throws ApiError
     */
    public deleteApiBooking(
        bookingId: string,
        cancellationReason?: string,
    ): Observable<MessageResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'DELETE',
            url: '/booking/{bookingId}',
            path: {
                'bookingId': bookingId,
            },
            query: {
                'cancellationReason': cancellationReason,
            },
        });
    }
    /**
     * @param amenityId
     * @param slotType
     * @param fromDate
     * @param toDate
     * @param pageNumber
     * @param pageSize
     * @returns ReportResponseDto Success
     * @throws ApiError
     */
    public getApiBookingReport(
        amenityId?: string,
        slotType?: string,
        fromDate?: string,
        toDate?: string,
        pageNumber: number = 1,
        pageSize: number = 10,
    ): Observable<ReportResponseDto> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/booking/report',
            query: {
                'amenityId': amenityId,
                'slotType': slotType,
                'fromDate': fromDate,
                'toDate': toDate,
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
        });
    }
}
