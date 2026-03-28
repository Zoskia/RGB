import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginService } from './login.service';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { LoginUserDto } from '../../../dtos/login-user.dto';
import { LoginResponseDto } from '../../../dtos/login-response.dto';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  private static readonly MIN_LENGTH = 5;
  private static readonly MAX_LENGTH = 16;
  private static readonly USERNAME_PATTERN = /^[a-zA-Z0-9._-]{5,16}$/;
  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private loginService: LoginService,
    private router: Router,
  ) {
    this.loginForm = this.fb.group({
      username: [
        '',
        [
          Validators.required,
          Validators.minLength(LoginComponent.MIN_LENGTH),
          Validators.maxLength(LoginComponent.MAX_LENGTH),
          Validators.pattern(LoginComponent.USERNAME_PATTERN),
        ],
      ],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(LoginComponent.MIN_LENGTH),
          Validators.maxLength(LoginComponent.MAX_LENGTH),
        ],
      ],
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      console.log('Form data invalid!');
      this.loginForm.markAllAsTouched();
      return;
    }

    const formData = this.loginForm.value;
    const loginUser: LoginUserDto = {
      username: formData.username,
      password: formData.password,
    };

    this.loginService.loginUser(loginUser).subscribe({
      next: (response: LoginResponseDto) => {
        console.log('Login successful:', response);
        this.loginService.storeSession(response);

        this.router.navigate(['/hex-grid', response.team]);
      },
      error: (err) => {
        console.error('Login failed:', err);
      },
    });
  }
}
