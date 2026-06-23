import { inject, Injectable } from "@angular/core";
import { ApiService } from "../../../core/services/api.service";
import { Observable } from "rxjs";
import { ProjectMember } from "../models/project-member.model";

@Injectable({
  providedIn: 'root'
})
export class ProjectMemberService {
  private api = inject(ApiService);

  getMembers(projectId: string): Observable<ProjectMember[]> {
    return this.api.get<ProjectMember[]>(
      `projects/projectId=${projectId}/members`
    );
  }

  removeMember(projectId: string, userId: string) {
    return this.api.delete(
      `projects/projectId=${projectId}/members/userId=${userId}`
    );
  }
}