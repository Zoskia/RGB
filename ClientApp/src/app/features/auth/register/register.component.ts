import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
})
export class RegisterComponent {
  registerForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      team: ['red', Validators.required],
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      //implement frontent error stuff
      console.log('invalid form');
      return;
    }

    const userData = this.registerForm.value;
    //send to backend
    console.log('valid form');
  }
}
