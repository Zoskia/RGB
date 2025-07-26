import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RegisterService } from './register.service';
import { RegisterUserDto } from '../../../dtos/register-user.dto';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
})
export class RegisterComponent {
  registerForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private registerService: RegisterService
  ) {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      team: ['0', Validators.required],
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      console.log('invalid form');
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
      },
      error: (err) => {
        console.error('Registration failed:', err);
      },
    });
  }
}
