namespace AmenityBookingService.Application.Constants;

/// <summary>
/// Contains constant error message strings used across the application.
/// </summary>
public static class ErrorMessages
{
    /// <summary>User ID not found in token.</summary>
    public const string UserNotFoundInToken = "User ID not found in token";

    /// <summary>Amenity with ID '{0}' not found.</summary>
    public const string AmenityNotFound = "Amenity with ID '{0}' not found";

    /// <summary>Amenity with name '{0}' already exists.</summary>
    public const string AmenityNameAlreadyExists = "Amenity with name '{0}' already exists";

    /// <summary>Cannot delete amenity with existing slots or booking history.</summary>
    public const string AmenityHasSlotsOrBookings =
        "Cannot delete amenity with existing slots or booking history";

    /// <summary>Failed to delete amenity.</summary>
    public const string AmenityDeleteFailed = "Failed to delete amenity";

    /// <summary>Slot type with ID '{0}' not found.</summary>
    public const string SlotTypeNotFound = "Slot type with ID '{0}' not found";

    /// <summary>Status with ID '{0}' not found.</summary>
    public const string StatusNotFound = "Status with ID '{0}' not found";

    /// <summary>File type '{0}' is not supported. Allowed: {1}.</summary>
    public const string FileTypeNotSupported = "File type '{0}' is not supported. Allowed: {1}";

    /// <summary>Time slot capacity validation - max capacity must be 1 for TIME slot type.</summary>
    public const string TimeSlotMaxCapacity =
        "For TIME slot type, max capacity must be 1 for slot: {0}";

    /// <summary>Duplicate slot exists for the given date and time.</summary>
    public const string SlotAlreadyExists = "A slot already exists for date {0:yyyy-MM-dd} at {1}";

    /// <summary>Cannot delete slot with existing bookings.</summary>
    public const string SlotHasBookings = "Cannot delete slot with existing bookings";

    /// <summary>Amenity not found.</summary>
    public const string AmenitiesNotFound = "Amenity not found";

    /// <summary>Cannot reduce capacity below current bookings.</summary>
    public const string CapacityBelowBookings =
        "Cannot reduce capacity below current bookings ({0})";

    /// <summary>For TIME slot type, max capacity must be 1.</summary>
    public const string TimeSlotMaxCapacit = "For TIME slot type, max capacity must be 1";

    /// <summary>End time must be after start time.</summary>
    public const string EndTimeValidation = "End time must be after start time";

    /// <summary>Name is required.</summary>
    public const string NameRequired = "Name is required";

    /// <summary>Name must not exceed 255 characters.</summary>
    public const string NameLimitError = "Name must not exceed 255 characters";

    /// <summary>SlotTypeId is required.</summary>
    public const string SlotTypeRequiredError = "SlotTypeId is required";

    /// <summary>StatusId is required.</summary>
    public const string StatusIdRequired = "StatusId is required";

    /// <summary>Location is required.</summary>
    public const string LocationRequired = "Location is required";

    /// <summary>Location must not exceed 255 characters.</summary>
    public const string LocationStringLimit = "Location must not exceed 255 characters";

    /// <summary>Rules must not exceed 2000 characters.</summary>
    public const string RuleStringLimit = "Rules must not exceed 2000 characters";

    /// <summary>ImageUrl must not exceed 500 characters.</summary>
    public const string ImageStringLimit = "ImageUrl must not exceed 500 characters";

    /// <summary>SlotLabel is required.</summary>
    public const string SlotLableValidation = "SlotLabel is required";

    /// <summary>SlotLabel must not exceed 100 characters.</summary>
    public const string SlotLableLimit = "SlotLabel must not exceed 100 characters";

    /// <summary>SlotDate is required.</summary>
    public const string SlotDateValidation = "SlotDate is required";

    /// <summary>SlotDate must be today or a future date.</summary>
    public const string SlotDateValue = "SlotDate must be today or a future date";

    /// <summary>StartTime is required.</summary>
    public const string StartTimeRequired = "StartTime is required";

    /// <summary>MaxCapacity must be greater than 0.</summary>
    public const string MaxCapacityValidation = "MaxCapacity must be greater than 0";

    /// <summary>MaxCapacity must not exceed 999.</summary>
    public const string MaxCapacityValue = "MaxCapacity must not exceed 999";

    /// <summary>At least one slot is required.</summary>
    public const string CreateSlotBlukMinError = "At least one slot is required";

    /// <summary>Cannot create more than 100 slots at once.</summary>
    public const string CreateSlotBulkMaxError = "Cannot create more than 100 slots at once";

    /// <summary>SlotId is required.</summary>
    public const string SlotIdRequired = "SlotId is required";

    /// <summary>SlotLabel must not exceed 100 characters.</summary>
    public const string SlotLabelLimit = "SlotLabel must not exceed 100 characters";

    /// <summary>MaxCapacity must be greater than 0.</summary>
    public const string MaxCapacityLimit = "MaxCapacity must be greater than 0";

    /// <summary>MaxCapacity must not exceed 999.</summary>
    public const string MaxCapacityMaxLimit = "MaxCapacity must not exceed 999";

    /// <summary>At least one field (SlotLabel, StartTime, EndTime, or MaxCapacity) must be provided for update.</summary>
    public const string AtLeastOneUpdateError =
        "At least one field (SlotLabel, StartTime, EndTime, or MaxCapacity) must be provided for update";

    /// <summary>AmenityId is required.</summary>
    public const string AmenityIdRequired = "AmenityId is required";

    /// <summary>EndTime is required.</summary>
    public const string EndTimeRequired = "EndTime is required";

    /// <summary>Failed to delete slot.</summary>
    public const string SlotDeleteFailed = "Failed to delete slot";

    /// <summary>Slot with ID '{0}' not found for booking.</summary>
    public const string BookingSlotNotFound = "Slot with ID '{0}' not found";

    /// <summary>Cannot book a slot in the past.</summary>
    public const string BookingSlotInPast = "Cannot book a slot in the past";

    /// <summary>Slot type not found for this amenity.</summary>
    public const string BookingSlotTypeNotFound = "Slot type not found for this amenity";

    /// <summary>This slot is already booked.</summary>
    public const string BookingSlotAlreadyBooked = "This slot is already booked";

    /// <summary>Slot type requires exactly 1 person.</summary>
    public const string BookingTimeOnlyOnePerson = "TIME slots only allow 1 person";

    /// <summary>Not enough capacity. Available: {0}, Requested: {1}.</summary>
    public const string BookingNotEnoughCapacity =
        "Not enough capacity. Available: {0}, Requested: {1}";

    /// <summary>Booking status '{0}' not found in reference data.</summary>
    public const string BookingStatusNotFound = "Booking status '{0}' not found";

    /// <summary>Booking with ID '{0}' not found.</summary>
    public const string BookingNotFound = "Booking with ID '{0}' not found";

    /// <summary>User can only cancel their own bookings.</summary>
    public const string BookingNotOwnedByUser = "You can only cancel your own bookings";

    /// <summary>Booking is already cancelled.</summary>
    public const string BookingAlreadyCancelled = "Booking is already cancelled";

    /// <summary>Cannot cancel a booking for a past date.</summary>
    public const string BookingCancelledPastDate = "Cannot cancel a booking for a past date";

    /// <summary>Unknown slot type: {0}.</summary>
    public const string UnknownSlotType = "Unknown slot type: {0}";

    /// <summary>People count must be greater than 0.</summary>
    public const string PeopleCountMinError = "People count must be greater than 0";

    /// <summary>People count must not exceed 999.</summary>
    public const string PeopleCountMaxError = "People count must not exceed 999";
}

/// <summary>
/// Contains constant success message strings used across the application.
/// </summary>
public static class SuccessMessages
{
    /// <summary>Amenity deleted successfully.</summary>
    public const string DeletedMessage = "Amenity deleted successfully";

    /// <summary>Amenity updated successfully.</summary>
    public const string UpdatedMessage = "Amenity updated successfully";

    /// <summary>Bulk slots created successfully with count.</summary>
    public const string SlotsBulkCreated = "{0} slots created successfully";

    /// <summary>Slot deleted successfully.</summary>
    public const string SlotDeleteMessage = "Slot deleted successfully";

    /// <summary>Slot updated successfully.</summary>
    public const string SlotUpdateMessage = "Slot updated successfully";

    /// <summary>Booking cancelled successfully.</summary>
    public const string BookingCancelled = "Booking cancelled successfully";
}
