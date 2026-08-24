export interface CommentDto {
  commentId: string;
  complaintId: string;
  commentedBy: string;
  commentText: string;
  staffRating: number | null;
  createdAt: string;
}

export interface CreateCommentRequestDto {
  commentText: string;
  staffRating?: number | null;
}

export interface ProgressLogEntryDto {
  logId: string;
  complaintId: string;
  changedBy: string;
  status: string;
  remarks: string | null;
  changedDate: string;
}
