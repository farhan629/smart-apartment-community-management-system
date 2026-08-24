using Shared.SharedLibrary.DTO.Common;

namespace AmenityBookingService.Application.Features.Slots.DTO;

/// <summary>Response DTO for a single slot in the standard slot list.</summary>
public class SlotResponseDto
{
    /// <summary>The slot ID.</summary>
    public Guid Id { get; set; }

    /// <summary>The display label.</summary>
    public string SlotLabel { get; set; } = string.Empty;

    /// <summary>The slot date.</summary>
    public DateTime SlotDate { get; set; }

    /// <summary>The start time.</summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>The end time.</summary>
    public TimeSpan EndTime { get; set; }

    /// <summary>The maximum capacity.</summary>
    public int MaxCapacity { get; set; }

    /// <summary>The current number of active bookings.</summary>
    public int CurrentBookings { get; set; }
}

/// <summary>Response DTO for a single slot in the available-slots view.</summary>
public class AvailableSlotResponseDto
{
    /// <summary>The slot ID.</summary>
    public Guid SlotId { get; set; }

    /// <summary>The display label.</summary>
    public string SlotLabel { get; set; } = string.Empty;

    /// <summary>The slot date.</summary>
    public DateTime SlotDate { get; set; }

    /// <summary>The start time.</summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>The end time.</summary>
    public TimeSpan EndTime { get; set; }

    /// <summary>The maximum capacity.</summary>
    public int MaxCapacity { get; set; }

    /// <summary>The current number of active bookings.</summary>
    public int CurrentBookings { get; set; }

    /// <summary>The remaining bookable spots (MaxCapacity - CurrentBookings).</summary>
    public int AvailableSpots { get; set; }
}

/// <summary>Paginated response DTO for the standard slot list.</summary>
public class SlotListResponseDto
{
    /// <summary>The list of slot DTOs.</summary>
    public List<SlotResponseDto> Data { get; set; } = new();

    /// <summary>Pagination metadata.</summary>
    public PaginationDto Pagination { get; set; } = new();
}

/// <summary>Paginated response DTO for the available-slots view, including amenity context.</summary>
public class AvailableSlotsResponseDto
{
    /// <summary>The amenity ID.</summary>
    public Guid AmenityId { get; set; }

    /// <summary>The amenity name.</summary>
    public string AmenityName { get; set; } = string.Empty;

    /// <summary>The slot type code (e.g. TIME, TIME_COUNT).</summary>
    public string SlotType { get; set; } = string.Empty;

    /// <summary>The amenity location.</summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>The amenity usage rules.</summary>
    public string Rules { get; set; } = string.Empty;

    /// <summary>The amenity image URL.</summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>The list of available slot DTOs.</summary>
    public List<AvailableSlotResponseDto> Slots { get; set; } = new();

    /// <summary>Pagination metadata.</summary>
    public PaginationDto Pagination { get; set; } = new();
}
