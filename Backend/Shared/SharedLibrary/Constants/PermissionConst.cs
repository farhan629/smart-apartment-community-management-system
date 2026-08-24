namespace Shared.SharedLibrary.Constants;

/// <summary>
/// Contains permission constants used for authorization.
/// </summary>
public static class PermissionConst
{
    /// <summary>Permission to view amenities.</summary>
    public const string AMENITY_VIEW = "amenity:view";

    /// <summary>Permission to manage amenities (create, update, delete).</summary>
    public const string AMENITY_MANAGE = "amenity:manage";

    /// <summary>Permission to view slots.</summary>
    public const string SLOT_VIEW = "slot:view";

    /// <summary>Permission to manage slots (create, update, delete).</summary>
    public const string SLOT_MANAGE = "slot:manage";

    /// <summary>Permission to apply/book slots.</summary>
    public const string SLOT_APPLY = "slot:apply";

    /// <summary>Permission to view users.</summary>
    public const string USER_VIEW = "user:view";

    /// <summary>Permission to manage users.</summary>
    public const string USER_MANAGE = "user:manage";

    /// <summary>Permission to view approvals.</summary>
    public const string APPROVAL_VIEW = "approval:view";

    /// <summary>Permission to manage approvals.</summary>
    public const string APPROVAL_MANAGE = "approval:manage";

    /// <summary>Admin can view all flats.</summary>
    public const string VIEW_ALL_FLATS = "flat:view";

    /// <summary>Resident can view their own flat details.</summary>
    public const string SPECIFIC_FLATS = "flat:self";

    /// <summary>Permission to view visitor records.</summary>
    public const string VISITOR_VIEW = "visitor:view";

    /// <summary>Permission to create and update visitor records.</summary>
    public const string VISITOR_MANAGE = "visitor:manage";

    /// <summary>Permission to hard-delete or soft-delete visitor records.</summary>
    public const string VISITOR_DELETE = "visitor:delete";

    /// <summary>Permission to view all visits (Resident sees own, Security/Admin see all).</summary>
    public const string VISIT_VIEW = "visit:view";

    /// <summary>Permission to register a planned visit (Resident) or walk-in visit (Security).</summary>
    public const string VISIT_REGISTER = "visit:register";

    /// <summary>Permission to update or cancel own visits (Resident, Admin).</summary>
    public const string VISIT_MANAGE = "visit:manage";

    /// <summary>Permission to approve or reject a visit (Resident — for unplanned guests sent by Security).</summary>
    public const string VISIT_APPROVE = "visit:approve";

    /// <summary>Permission to check-in or check-out a visitor at the gate (Security only).</summary>
    public const string VISIT_CHECKIN = "visit:checkin";

    /// <summary>Resident can submit a new complaint.</summary>
    public const string COMPLAINT_SUBMIT = "complaint:submit";

    /// <summary>Admin and Staff can view complaints and related data
    /// (list, detail, assignments, comments, escalation, progress log).</summary>
    public const string COMPLAINT_VIEW = "complaint:view";

    /// <summary>Staff can accept/deny assignments and update complaint status.
    /// Admin can update escalation details.</summary>
    public const string COMPLAINT_MANAGE = "complaint:manage";

    /// <summary>Resident can cancel their own complaint.</summary>
    public const string COMPLAINT_CANCEL = "complaint:cancel";

    /// <summary>Admin can assign or reassign a complaint to staff.</summary>
    public const string COMPLAINT_ASSIGN = "complaint:assign";

    /// <summary>Resident can add follow-up comments on their own complaint.</summary>
    public const string COMPLAINT_COMMENT = "complaint:comment";

    /// <summary>Resident can re-escalate an unresolved complaint.</summary>
    public const string COMPLAINT_ESCALATE = "complaint:escalate";

    /// <summary>Admin can view all staff profiles.</summary>
    public const string STAFF_VIEW = "staff:view";

    /// <summary>Admin can create or update any staff profile.</summary>
    public const string STAFF_MANAGE = "staff:manage";

    /// <summary>Staff can update their own profile only.</summary>
    public const string STAFF_SELF_UPDATE = "staff:self";

    /// <summary>Admin and the staff member themselves can view availability slots.</summary>
    public const string STAFF_AVAILABILITY_VIEW = "staff:availability:view";

    /// <summary>Admin can create or delete availability slots.</summary>
    public const string STAFF_AVAILABILITY_MANAGE = "staff:availability:manage";

    /// <summary>Admin can access all reports (complaints, staff, residents,
    /// categories, escalations).</summary>
    public const string REPORT_VIEW = "report:view";

    /// <summary>Internal system permission to trigger background jobs
    /// (e.g. escalation check). Not assigned to any user role.</summary>
    public const string JOB_TRIGGER = "job:trigger";
}
