import { inject, Injectable } from "@angular/core";
import { CreateProjectRequest } from "../models/create-project-request.model";
import { Observable } from "rxjs";
import { Project } from "../models/project.model";
import { ApiService } from "../../../core/services/api.service";
import { UpdateProjectRequest } from "../models/update-project-request.model";

@Injectable({
    providedIn: `root`
})
export class ProjectService{
    private readonly api = inject(ApiService)

    CreateProject(
        request: CreateProjectRequest
    ): Observable<Project>{
        return this.api.post<Project>(
            `Projects`,
            request
        );
    }

    GetProjects(): Observable<Project[]>{
        return this.api.get<Project[]>(
            `Projects`
        );
    }

   getProjectById(id: string): Observable<Project> {
        return this.api.get<Project>(
            `Projects/${id}`
        );
    }

    updateProject(
        request: UpdateProjectRequest
    ): Observable<Project>{
        return this.api.put<Project>(
            `Project/${request.id}`,
            request
        );
    }

    deleteProject(id: string):Observable<void>{
        return this.api.delete<void>(
            `Projects/${id}`
        );
    }
}