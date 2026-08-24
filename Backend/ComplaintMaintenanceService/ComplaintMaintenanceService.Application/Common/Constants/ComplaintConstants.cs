namespace ComplaintMaintenanceService.Application.Common.Constants;

public static class ComplaintConstants
{
    public static class Routes
    {
        public const string ComplaintId = "{complaintId:guid}";
        public const string Cancel = "{complaintId:guid}/cancel";
        public const string Status = "{complaintId:guid}/status";
    }

    public static class Roles
    {
        public const string Resident = "Resident";
        public const string StaffOrAdmin = "Staff,Admin";
        public const string ResidentStaffOrAdmin = "Resident,Staff,Admin";
    }

    public static class FlatLookupMessages
    {
        public const string FlatNotFound = "No approved flat found for resident.";
    }

    public static class RefSetIds
    {
        public static readonly Guid ComplaintType = Guid.Parse(
            "10000000-0000-0000-0000-000000000001"
        );
        public static readonly Guid ComplaintPriority = Guid.Parse(
            "20000000-0000-0000-0000-000000000002"
        );
        public static readonly Guid ComplaintStatus = Guid.Parse(
            "30000000-0000-0000-0000-000000000003"
        );
        public static readonly Guid AssignmentStatus = Guid.Parse(
            "40000000-0000-0000-0000-000000000004"
        );
    }

    public static class StatusCodes
    {
        public const string Open = "Open";
        public const string Assigned = "Assigned";
        public const string InProgress = "InProgress";
        public const string Resolved = "Resolved";
        public const string Closed = "Closed";
        public const string Cancelled = "Cancelled";
        public const string Escalated = "Escalated";
    }

    public static class AssignmentStatusCodes
    {
        public const string PendingAcceptance = "PendingAcceptance";
        public const string Active = "Active";
        public const string Denied = "Denied";
        public const string Reassigned = "Reassigned";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
    }

    public static class Messages
    {
        public const string ComplaintNotFound = "Complaint not found.";
        public const string ComplaintCreated = "Complaint created successfully.";
        public const string ComplaintFetched = "Complaint fetched successfully.";
        public const string ComplaintsFetched = "Complaints fetched successfully.";
        public const string ComplaintCancelled = "Complaint cancelled successfully.";
        public const string StatusUpdated = "Complaint status updated successfully.";
        public const string AlreadyCancelled = "Complaint is already cancelled.";
        public const string CannotUpdateStatus = "Complaint status cannot be updated.";
        public const string CancellationReasonRequired = "Cancellation reason is required.";
        public const string InvalidStatusValue = "Invalid status value.";
        public const string OpenStatusNotConfigured = "Open status ref term is not configured.";
        public const string CategoryNotFound = "Category not found.";
        public const string InvalidRefTerm = "Invalid reference term.";
        public const string ComplaintIdRequired = "Complaint is required.";
        public const string InvalidDateRange = "To date must be on or after from date.";
        public const string ComplaintTypeRequired = "Complaint type is required.";
        public const string CategoryIdRequired = "Category is required.";
        public const string PriorityRequired = "Priority is required.";
        public const string DescriptionRequired = "Description is required.";
        public const string DescriptionMaxLength = "Description must not exceed 1000 characters.";
        public const string PreferredDateRequired = "Preferred date is required.";
        public const string PreferredDateInvalidFormat =
            "Preferred date must be a valid date in YYYY-MM-DD format.";
        public const string PreferredTimeInvalidFormat =
            "Preferred time must be in HH:mm format when provided.";
        public const string CancellationReasonMaxLength =
            "Cancellation reason must not exceed 500 characters.";
        public const string StatusRequired = "Status is required.";
        public const string CategoryIdRequiredForStaff = "CategoryId is required for staff users.";
    }

    public static class GrpcMessages
    {
        public const string InvalidUserId = "Invalid userId format.";
        public const string InvalidCategoryId = "Invalid categoryId format.";
        public const string CategoryNotFound = "Category not found in ComplaintMaintenanceService.";
        public const string StaffCreatedSuccess = "Staff profile created successfully.";
        public const string InternalError =
            "An internal error occurred while creating the staff profile.";
    }

    public static class NotificationTypes
    {
        public const string ComplaintRaised = "COMPLAINT_RAISED";
        public const string ComplaintAssigned = "COMPLAINT_ASSIGNED";
        public const string ComplaintAccepted = "COMPLAINT_ACCEPTED";
        public const string ComplaintDenied = "COMPLAINT_DENIED";
        public const string ComplaintInProgress = "COMPLAINT_IN_PROGRESS";
        public const string ComplaintResolved = "COMPLAINT_RESOLVED";
        public const string ComplaintEscalated = "COMPLAINT_ESCALATED";
        public const string ComplaintRatingDone = "COMPLAINT_RATING_SUBMITTED";
        public const string ComplaintRatingRequest = "COMPLAINT_RATING_REQUEST";
        public const string ComplaintCancelled = "COMPLAINT_CANCELLED";
        public const string AdminComplaintCancelled = "admin_complaint_cancelled";
    }

    public static class NotificationTitles
    {
        public const string ComplaintRaised = "New Complaint Raised";
        public const string ComplaintAssigned = "Complaint Assigned to You";
        public const string ComplaintAccepted = "Staff Accepted Your Complaint";
        public const string ComplaintDenied = "Staff Denied Your Complaint";
        public const string ComplaintInProgress = "Complaint In Progress";
        public const string ComplaintResolved = "Complaint Resolved";
        public const string ComplaintEscalated = "Complaint Escalated";
        public const string ComplaintRatingDone = "Resident Submitted a Rating";
        public const string ComplaintRatingRequest = "Please Rate Your Staff";
        public const string ComplaintCancelled = "Complaint Cancelled";
        public const string ComplaintCancelledAdmin = "Complaint Cancelled by Resident";
    }

    public static class NotificationMessages
    {
        public const string ComplaintRaisedAdmin = "Resident raised complaint #{0}. Category: {1}.";
        public const string ComplaintRaisedStaff =
            "You have been assigned a new complaint #{0}. Category: {1}.";
        public const string ComplaintAssignedStaff =
            "Complaint #{0} has been assigned to you. Please accept or deny.";
        public const string ComplaintAssignedUser =
            "Your complaint #{0} has been assigned to staff {1}.";
        public const string ComplaintAccepted =
            "Staff {0} accepted complaint #{1}. Work will begin shortly.";
        public const string ComplaintDenied =
            "Staff {0} denied complaint #{1}. A reassignment will follow.";
        public const string ComplaintInProgress = "Work has started on your complaint #{0}.";
        public const string ComplaintResolved =
            "Your complaint #{0} has been resolved. Please rate the staff.";
        public const string ComplaintEscalated =
            "Complaint #{0} has been escalated due to no response or no-show.";
        public const string ComplaintRatingDone =
            "Resident rated staff {0} for complaint #{1}. Rating: {2}/5.";
        public const string ComplaintRatingRequest =
            "Your complaint #{0} is resolved. Please rate the staff and leave any comments.";
        public const string ComplaintCancelled =
            "Complaint {0} was cancelled by the resident. Reason: {1}";
    }

    public static class RoleCodes
    {
        public const string Admin = "Admin";
        public const string Staff = "Staff";
        public const string Resident = "Resident";
    }
    
    public static class RoleIds
    {
        public static readonly Guid Admin = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    }

    public static class AssignmentMessages
    {
        public const string AssignmentNotFound = "Assignment not found.";
        public const string AssignmentCreated = "Complaint assigned successfully.";
        public const string AssignmentAccepted = "Assignment accepted successfully.";
        public const string AssignmentDenied = "Assignment denied successfully.";
        public const string AssignmentReassigned = "Complaint reassigned successfully.";
        public const string AlreadyAssigned = "Complaint already has an active assignment.";
        public const string AlreadyActioned = "Assignment has already been accepted or denied.";
        public const string StaffNotFound = "Staff not found.";
        public const string InvalidAssignment =
            "This assignment does not belong to this complaint.";
        public const string AssignmentStatusNotConfigured =
            "Assignment status ref term is not configured.";
        public const string StaffRequired = "Staff is required.";
        public const string DueDateRequired = "Due date is required.";
        public const string DueDateMustBeFuture = "Due date must be in the future.";
        public const string DenialReasonRequired = "Denial reason is required.";
        public const string DenialReasonMaxLength = "Denial reason must not exceed 500 characters.";
        public const string ComplaintIdRequired = "Complaint is required.";
        public const string AssignmentIdRequired = "Assignment is required.";
        public const string AssignedByRequired = "AssignedBy is required.";
        public const string StaffUserIdRequired = "Staff user is required.";
    }

    public static class EscalationMessages
    {
        public const string EscalationNotFound = "Escalation not found.";
        public const string EscalationFetched = "Escalation fetched successfully.";
        public const string EscalationUpdated = "Escalation updated successfully.";
        public const string UnauthorizedEscalation = "You can only escalate your own complaints.";
        public const string EscalationCreated = "Complaint escalated successfully.";
        public const string EscalationReasonRequired = "Escalation reason is required.";
        public const string EscalationReasonMaxLength =
            "Escalation reason must not exceed 1000 characters.";
        public const string ResolutionDateRequiredWhenResolved =
            "Resolution date is required when marking as resolved.";
        public const string ResolutionDateCannotBeFuture =
            "Resolution date cannot be in the future.";
        public const string ComplaintIdRequired = "Complaint is required.";
        public const string UpdatedByRequired = "UpdatedBy is required.";
    }

    public static class CommentMessages
    {
        public const string CommentAdded = "Comment added successfully.";
        public const string CommentsFetched = "Comments fetched successfully.";
        public const string CommentNotFound = "Comment not found.";
        public const string CommentTextRequired = "Comment text is required.";
        public const string CommentTextMaxLength = "Comment text must not exceed 1000 characters.";
        public const string StaffRatingRange = "Staff rating must be between 1 and 5.";
        public const string ComplaintIdRequired = "Complaint is required.";
        public const string StaffIdRequired = "Staff is required.";
        public const string CommentedByRequired = "Commented by is required.";
    }

    public static class RatingMessages
    {
        public const string RatingSubmitted = "Rating submitted successfully.";
        public const string RatingNotAllowed =
            "Rating can only be submitted after complaint is resolved.";
        public const string AlreadyRated = "You have already rated this complaint.";
    }

    public static class ProgressLogMessages
    {
        public const string ProgressLogFetched = "Progress log fetched successfully.";
        public const string ComplaintIdRequired = "Complaint is required.";
    }

    public static class DateFormats
    {
        public const string OutputDate = "yyyy-MM-dd";
        public const string OutputTime = @"hh\:mm";

        public static readonly string[] Accepted = new[]
        {
            "dd-MM-yyyy",
            "dd/MM/yyyy",
            "yyyy-MM-dd",
            "yyyy/MM/dd",
            "dd-MM-yyyy HH:mm:ss",
            "dd/MM/yyyy HH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:ss.ffffffZ",
        };
    }

    public static class ReportMessages
    {
        public const string ReportFetched = "Report fetched successfully.";
        public const string InvalidDateRange = "To date must be on or after from date.";
        public const string FromDateCannotBeFuture = "From date cannot be in the future.";
        public const string ToDateCannotBeFuture = "To date cannot be in the future.";
    }

    public static class ValidationLimits
    {
        public const int DenialReasonMaxLength = 500;
        public const int EscalationReasonMaxLength = 1000;
        public const int CommentTextMaxLength = 1000;
        public const int StaffRatingMin = 1;
        public const int StaffRatingMax = 5;
        public const int DescriptionMaxLength = 1000;
        public const int CancellationReasonMaxLength = 500;
        public const int DetailsMaxLength = 1000;
    }

    public static class BackgroundJobMessages
    {
        public const string TriggeredByRequired = "TriggeredBy is required.";
    }

    public static class ReportStatusCodes
    {
        public const string Open = "OPEN";
        public const string Assigned = "ASSIGNED";
        public const string InProgress = "IN_PROGRESS";
        public const string Resolved = "RESOLVED";
        public const string Cancelled = "CANCELLED";
        public const string Escalated = "ESCALATED";
    }
}
