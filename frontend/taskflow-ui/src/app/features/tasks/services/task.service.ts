import { inject, Injectable } from "@angular/core";
import { ApiService } from "../../../core/services/api.service";
import { CreateTaskRequest } from "../models/create-task-request";
import { Observable } from "rxjs";
import { TaskResponse } from "../models/task-response";

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
}