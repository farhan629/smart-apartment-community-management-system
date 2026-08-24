namespace AmenityBookingService.Application.Features.Slots.DTO;

/// <summary>Request DTO for creating a single slot.</summary>
public class CreateSlotRequestDto
{
    /// <summary>The display label for the slot.</summary>
    public string SlotLabel { get; set; } = string.Empty;

    /// <summary>The date of the slot.</summary>
    public DateTime SlotDate { get; set; }

    /// <summary>The start time of the slot.</summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>The end time of the slot.</summary>
    public TimeSpan EndTime { get; set; }

    /// <summary>The maximum capacity for the slot.</summary>
    public int MaxCapacity { get; set; }
}

/// <summary>Request DTO for bulk creating slots.</summary>
public class CreateSlotsBulkRequestDto
{
    /// <summary>The list of slots to create.</summary>
    public List<CreateSlotRequestDto> Slots { get; set; } = new();
}

/// <summary>Request DTO for updating an existing slot. All properties are optional.</summary>
public class UpdateSlotRequestDto
{
    /// <summary>The updated display label.</summary>
    public string? SlotLabel { get; set; }

    /// <summary>The updated start time.</summary>
    public TimeSpan? StartTime { get; set; }

    /// <summary>The updated end time.</summary>
    public TimeSpan? EndTime { get; set; }

    /// <summary>The updated max capacity.</summary>
    public int? MaxCapacity { get; set; }
}
