namespace ComplaintMaintenanceService.Application.Common.Constants;

public static class StaffConstants
{
    public static class Routes
    {
        public const string StaffId = "{staffId:guid}";
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Staff = "Staff";
        public const string StaffOrAdmin = "Staff,Admin";
    }

    public static class Messages
    {
        public const string StaffNotFound = "Staff not found.";
        public const string StaffFetched = "Staff fetched successfully.";
        public const string StaffListFetched = "Staff list fetched successfully.";
        public const string StaffUpdated = "Staff updated successfully.";
        public const string NotAuthorizedToUpdateStaff =
            "You are not authorized to update this staff profile.";
    }

    public static class Validation
    {
        public const string CategoryIdEmpty = "Category ID is required.";
        public const string DescriptionEmpty = "Description is required.";
        public const string DetailsEmpty = "Details are required.";
        public const string DescriptionTooLong = "Description must not exceed 500 characters.";
        public const string DetailsTooLong = "Details must not exceed 1000 characters.";
        public const string StaffIdRequired = "Staff is required.";
    }

    public static class ValidationLimits
    {
        public const int DescriptionMaxLength = 500;
        public const int DetailsMaxLength = 1000;
    }
}
