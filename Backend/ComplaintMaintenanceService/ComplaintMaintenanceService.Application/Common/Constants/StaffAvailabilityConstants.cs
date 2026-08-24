namespace ComplaintMaintenanceService.Application.Common.Constants;

public static class StaffAvailabilityConstants
{
    public static class Routes
    {
        public const string SlotId = "{slotId:guid}";
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string StaffOrAdmin = "Staff,Admin";
    }

    public static class Messages
    {
        public const string StaffNotFound = "Staff not found.";
        public const string SlotNotFound = "Availability slot not found.";
        public const string SlotAlreadyBooked =
            "This slot is already booked and cannot be deleted.";
        public const string SlotAlreadyCancelled = "This slot is already cancelled.";
        public const string SlotsCreated = "Availability slots created successfully.";
        public const string SlotsFetched = "Availability slots fetched successfully.";
        public const string SlotFetched = "Availability slot fetched successfully.";
        public const string SlotDeleted = "Availability slot deleted successfully.";
    }

    public static class Validation
    {
        public const string DateRequired = "Date is required.";
        public const string DateInvalidFormat = "Date must be in yyyy-MM-dd format.";
        public const string StartTimeRequired = "Start time is required.";
        public const string StartTimeInvalid = "Start time must be in HH:mm format.";
        public const string EndTimeRequired = "End time is required.";
        public const string EndTimeInvalid = "End time must be in HH:mm format.";
        public const string SlotsRequired = "At least one slot is required.";
        public const string TimeRangeInvalid = "End time must be after start time.";
        public const string SlotIdRequired = "Slot is required.";
        public const string StaffIdRequired = "Staff is required.";
        public const string DateRangeInvalid = "To date must be on or after from date.";
        public const string FilterTimeRangeInvalid = "End time must be after start time.";
    }

    public static class DateFormats
    {
        public const string SlotDate = "yyyy-MM-dd";
    }
}
