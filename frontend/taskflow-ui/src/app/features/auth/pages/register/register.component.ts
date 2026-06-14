import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { RegisterRequest } from '../../models/register-request';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule],
  standalone: true,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService)
  private router = inject(Router)

  form = this.fb.group({
    firstName:[``,[Validators.required]],
    lastName:[``,[Validators.required]],
    email:[``,[Validators.required]],
    password:[``,[Validators.required]],
  });

  register(){
    if(this.form.invalid){
      this.form.markAsTouched();
      return;
    }

    const request = this.form.value as RegisterRequest;

    this.authService.register(request).subscribe({
      next: () => {
        this.router.navigate([`/login`]);
    },
    error: (err) => {
        console.error(`Register Failed`, err);
    }
  });
  }
}
