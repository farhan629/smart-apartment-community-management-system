export interface AssignComplaintRequestDto {
  staffId: string;
  dueDate: string;
}

export interface DenyAssignmentRequestDto {
  denialReason: string;
}

export interface AssignmentResponseDto {
  assignmentId: string;
  complaintId: string;
  staffId: string;
  staffName: string;
  status: string;
  assignedDate: string;
  dueDate: string;
  acceptedDate: string | null;
  deniedDate: string | null;
  denialReason: string | null;
  assignedBy: string;
}

export interface ResidentFlatResponseDto {
  flatId: string;
  residentName: string;
  residentEmail: string;
  block: string;
  flatNumber: string;
}
