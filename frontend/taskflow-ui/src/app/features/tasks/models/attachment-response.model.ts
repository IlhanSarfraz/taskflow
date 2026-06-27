export interface AttachmentResponse {
  id: string;
  fileName: string;
  contentType: string;
  fileSize: number;
  url: string;
  uploadedByUserId: string;
  uploadedByName: string;
  uploadedByEmail: string;
  createdAtUtc: string;
}