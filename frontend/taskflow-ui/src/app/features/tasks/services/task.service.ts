import { inject, Injectable } from "@angular/core";
import { ApiService } from "../../../core/services/api.service";
import { CreateTaskRequest } from "../models/create-task-request";
import { Observable } from "rxjs";
import { TaskResponse } from "../models/task-response";
import { TaskDetails } from "../models/task-details";

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
}