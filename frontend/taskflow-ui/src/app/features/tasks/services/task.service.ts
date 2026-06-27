import { inject, Injectable } from "@angular/core";
import { ApiService } from "../../../core/services/api.service";
import { CreateTaskRequest } from "../models/create-task-request";
import { Observable } from "rxjs";
import { TaskResponse } from "../models/task-response";
import { TaskDetails } from "../models/task-details";
import { UpdateTaskRequest } from "../models/update-task-request";
import { CommentResponse } from "../models/comment-response";
import { CreateCommentRequest } from "../models/create-comment-request";
import { UpdateCommentRequest } from "../models/update-comment-request";
import { TaskDetailPage } from "../models/task-detail-page.model";
import { AttachmentResponse } from "../models/attachment-response.model";

@Injectable({
    providedIn: `root`
})
export class TaskService{
    private readonly api = inject(ApiService);

    CreateTask(
        request: CreateTaskRequest
        ): Observable<TaskResponse> {
            return this.api.post<TaskResponse>(
                'Tasks',
                request
            );
        }

    GetTaskById(taskId: string){
        return this.api.get<TaskDetails>(
            `Tasks/taskId=${taskId}`,
        )
    }

    GetTaskDetailPage(taskId: string): Observable<TaskDetailPage> {
        return this.api.get<TaskDetailPage>(
            `Tasks/taskId=${taskId}/detail`
        );
    }

    UpdateTask(
        taskId: string,
        request: UpdateTaskRequest){
            return this.api.put(
                `Tasks/taskId=${taskId}`,
                request
            );
    }

    DeleteTask(taskId: string){
        return this.api.delete(
            `Tasks/taskId=${taskId}`
        )
    }

    AssignTask(taskId: string, assigneeIds: string[]) {
    return this.api.put(
        `Tasks/taskId=${taskId}/assign`,
        { assigneeIds }
    );
    }

    GetMyTasks(){
        return this.api.get<TaskDetails[]>(
            `Tasks/my`
        );
    }

    CreateComment(taskId: string, request: CreateCommentRequest) {
        return this.api.post<CommentResponse>(
            `Tasks/taskId=${taskId}/comments`,
            request
        );
    }

    GetComments(taskId: string) {
        return this.api.get<CommentResponse[]>(
            `Tasks/taskId=${taskId}/comments`
        );
    }

    UpdateComment(commentId: string, request: UpdateCommentRequest) {
        return this.api.put(
            `Tasks/comments/commentId=${commentId}`,
            request
        );
    }

    DeleteComment(commentId: string) {
        return this.api.delete(
            `Tasks/comments/commentId=${commentId}`
        );
    }

    UploadAttachment(taskId: string, file: File): Observable<string> {
        const formData = new FormData();
        formData.append('file', file);

        return this.api.post<string>(
            `Tasks/${taskId}/attachments`,
            formData
        );
    }

    GetAttachments(taskId: string): Observable<AttachmentResponse[]> {
        return this.api.get<AttachmentResponse[]>(
            `Tasks/${taskId}/attachments`
        );
    }

    DownloadAttachment(attachmentId: string): Observable<Blob> {
        return this.api.getBlob(`Tasks/attachments/${attachmentId}/download`);
    }
}