export interface EscalationDto {
  escalationId: string;
  complaintId: string;
  escalatedBy: string;
  escalatedTo: string;
  escalationReason: string;
  escalationDate: string;
  resolvedAfterEscalation: boolean;
  resolutionDate: string | null;
}

export interface ReEscalateRequestDto {
  escalationReason: string;
}

export interface ReEscalateResponseDto {
  escalationId: string;
  complaintId: string;
  escalationReason: string;
  escalationDate: string;
}

export interface UpdateEscalationRequestDto {
  resolvedAfterEscalation: boolean;
  resolutionDate: string | null;
}
