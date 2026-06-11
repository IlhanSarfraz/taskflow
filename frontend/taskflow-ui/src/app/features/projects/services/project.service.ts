import { Injectable } from "@angular/core";
import { ApiService } from "../../../core/services/api.service";
import { Observable } from "rxjs";
import { Project } from "../models/project.model";

@Injectable({
    providedIn: 'root'
})
export class ProjectService{
    constructor(private api: ApiService){}

    getProjects(): Observable<Project[]>{
        return this.api.get<Project[]>('projects');
    }
}