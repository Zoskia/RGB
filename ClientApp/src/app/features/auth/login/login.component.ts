import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginService } from './login.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LoginUserDto } from '../../../dtos/login-user.dto';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(private fb: FormBuilder, private loginService: LoginService) {
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
      next: (response) => {
        console.log('Login successful:', response);
      },
      error: (err) => {
        console.error('Login failed:', err);
      },
    });
  }
}
