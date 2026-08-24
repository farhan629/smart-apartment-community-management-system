/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BookingResponseDto } from './BookingResponseDto';
import type { PaginationDto } from './PaginationDto';
import type { ReportFiltersDto } from './ReportFiltersDto';
import type { ReportSummaryDto } from './ReportSummaryDto';
export type ReportResponseDto = {
    filters?: ReportFiltersDto;
    summary?: ReportSummaryDto;
    bookings?: Array<BookingResponseDto> | null;
    pagination?: PaginationDto;
};

