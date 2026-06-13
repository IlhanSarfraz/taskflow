import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectMember } from '../../models/project-member.model';
import { ProjectMemberService } from '../../services/project-member.service';

@Component({
  selector: 'app-project-members',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './project-members.component.html',
  styleUrl: './project-members.component.scss',
})
export class ProjectMembersComponent {
  private route = inject(ActivatedRoute);
  private memberService = inject(ProjectMemberService);

  projectId!: string;
  members: ProjectMember[] = [];

  ngOnInit(){
    this.projectId = this.route.snapshot.paramMap.get(`projectId`)!;
    this.loadMembers();
  }

  loadMembers(){
    this.memberService.getMembers(this.projectId)
      .subscribe(res => this.members = res)
  }

    remove(userId: string) {
    this.memberService.removeMember(this.projectId, userId)
      .subscribe(() => this.loadMembers());
  }
}
