import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { RegisterService } from './register.service';
import { RegisterUserDto } from '../../../dtos/register-user.dto';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  private static readonly MIN_LENGTH = 5;
  private static readonly MAX_LENGTH = 16;
  private static readonly USERNAME_PATTERN = /^[a-zA-Z0-9._-]{5,16}$/;
  registerForm: FormGroup;
  // Modal state shown after a registration attempt.
  registrationSucceeded = false;
  showResultModal = false;
  modalTitle = '';
  modalMessage = '';

  constructor(
    private fb: FormBuilder,
    private registerService: RegisterService,
    private router: Router,
  ) {
    this.registerForm = this.fb.group({
      username: [
        '',
        [
          Validators.required,
          Validators.minLength(RegisterComponent.MIN_LENGTH),
          Validators.maxLength(RegisterComponent.MAX_LENGTH),
          Validators.pattern(RegisterComponent.USERNAME_PATTERN),
        ],
      ],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(RegisterComponent.MIN_LENGTH),
          Validators.maxLength(RegisterComponent.MAX_LENGTH),
        ],
      ],
      team: ['0', Validators.required],
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      console.log('invalid form');
      this.registerForm.markAllAsTouched();
      return;
    }

    const formValue = this.registerForm.value;

    const registerUserDto: RegisterUserDto = {
      username: formValue.username,
      password: formValue.password,
      team: Number(formValue.team),
    };

    this.registerService.registerUser(registerUserDto).subscribe({
      next: (response) => {
        console.log('Registration successful:', response);
        this.openResultModal(
          true,
          'Registration successful',
          'Your account has been created successfully.',
        );
      },
      error: (err) => {
        console.error('Registration failed:', err);
        // The API returns HTTP 409 (Conflict) when the username already exists.
        if (err?.status === 409) {
          this.openResultModal(
            false,
            'Registration failed',
            'User is already registered.',
          );
        }
      },
    });
  }

  // Opens the result modal with text matching the current registration outcome.
  private openResultModal(
    succeeded: boolean,
    title: string,
    message: string,
  ): void {
    this.registrationSucceeded = succeeded;
    this.modalTitle = title;
    this.modalMessage = message;
    this.showResultModal = true;
  }

  // Routes the user after acknowledging the modal result.
  onModalOk(): void {
    this.showResultModal = false;
    if (this.registrationSucceeded) {
      this.router.navigate(['/login']);
      return;
    }
    this.router.navigate(['/register']);
  }
}
