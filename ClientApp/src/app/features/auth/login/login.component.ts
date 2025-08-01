import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginService } from './login.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LoginUserDto } from '../../../dtos/login-user.dto';
import { ReactiveFormsModule } from '@angular/forms';
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
  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private loginService: LoginService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  onSubmit(): void {
    if (!this.loginForm.valid) {
      console.log('Form data invalid!');
    }

    const formData = this.loginForm.value;
    const loginUser: LoginUserDto = {
      username: formData.username,
      password: formData.password,
    };

    this.loginService.loginUser(loginUser).subscribe({
      next: (response: LoginResponseDto) => {
        console.log('Login successful:', response);

        localStorage.setItem('token', response.token);
        localStorage.setItem('username', response.username);
        localStorage.setItem('team', response.team.toString());
        localStorage.setItem('isAdmin', response.isAdmin.toString());

        this.router.navigate(['/hex-grid']);
      },
      error: (err) => {
        console.error('Login failed:', err);
      },
    });
  }
}
