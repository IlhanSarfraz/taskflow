export interface AppNotification {
  id: string;
  type: 'ProjectInvite' | 'InviteAccepted' | 'InviteDeclined';
  title: string;
  message: string;
  relatedEntityId: string | null;
  isRead: boolean;
  createdAtUtc: string;
}