import { inject, Injectable } from "@angular/core";
import { ApiService } from "../../../core/services/api.service";
import { CreateTaskRequest } from "../models/create-task-request";
import { Observable } from "rxjs";
import { TaskResponse } from "../models/task-response";
import { TaskDetails } from "../models/task-details";
import { UpdateTaskRequest } from "../models/update-task-request";

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
            `Tasks/${taskId}`,
        )
    }

    UpdateTask(
        taskId: string,
        request: UpdateTaskRequest){
            return this.api.put(
                `Tasks/${taskId}`,
                request
            );
    }

    DeleteTask(taskId: string){
        return this.api.delete(
            `Tasks/${taskId}`
        )
    }

    AssignTask(taskId: string, assigneeId: string) {
    return this.api.put(
        `Tasks/${taskId}/assign`,
        { assigneeId }
    );
    }

    GetMyTasks(){
        return this.api.get<TaskDetails[]>(
            `Tasks/my`
        );
    }
}