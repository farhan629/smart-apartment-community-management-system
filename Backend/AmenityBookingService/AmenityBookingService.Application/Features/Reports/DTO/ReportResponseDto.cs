using AmenityBookingService.Application.Features.Bookings.DTO;
using Shared.SharedLibrary.DTO.Common;

namespace AmenityBookingService.Application.Features.Reports.DTO;

/// <summary>
/// Request DTO containing filters for generating an amenity booking report.
/// </summary>
public class ReportFiltersDto
{
    /// <summary>Gets or sets the amenity ID to filter bookings.</summary>
    public Guid? AmenityId { get; set; }

    /// <summary>Gets or sets the amenity name to filter bookings.</summary>
    public string? AmenityName { get; set; }

    /// <summary>Gets or sets the slot type to filter bookings.</summary>
    public string? SlotType { get; set; }

    /// <summary>Gets or sets the start date of the reporting period.</summary>
    public DateTime? FromDate { get; set; }

    /// <summary>Gets or sets the end date of the reporting period.</summary>
    public DateTime? ToDate { get; set; }
}

/// <summary>
/// Response DTO containing summary statistics for an amenity booking report.
/// </summary>
public class ReportSummaryDto
{
    /// <summary>Gets or sets the total number of bookings.</summary>
    public int TotalBookings { get; set; }

    /// <summary>Gets or sets the total number of people across all bookings.</summary>
    public int TotalPeople { get; set; }

    /// <summary>Gets or sets the total number of active bookings.</summary>
    public int ActiveBookings { get; set; }

    /// <summary>Gets or sets the total number of cancelled bookings.</summary>
    public int CancelledBookings { get; set; }

    /// <summary>Gets or sets the total number of completed bookings.</summary>
    public int CompletedBookings { get; set; }

    /// <summary>Gets or sets the amenity utilization rate as a percentage.</summary>
    public double UtilizationRate { get; set; }
}

/// <summary>
/// Response DTO containing the generated booking report details.
/// </summary>
public class ReportResponseDto
{
    /// <summary>Gets or sets the filters applied to generate the report.</summary>
    public ReportFiltersDto Filters { get; set; } = new();

    /// <summary>Gets or sets the summary statistics for the report.</summary>
    public ReportSummaryDto Summary { get; set; } = new();

    /// <summary>Gets or sets the list of bookings included in the report.</summary>
    public List<BookingResponseDto> Bookings { get; set; } = new();

    /// <summary>Gets or sets the pagination details for the report results.</summary>
    public PaginationDto Pagination { get; set; } = new();
}
