import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import {
  FormGroup,
  FormControl,
  FormBuilder
} from '@angular/forms';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { User } from '../_models/user';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  isShow = false;
  resetForm: FormGroup;
  passwordForm: FormGroup;
  user: User;
  password: string;
  isReset = false;

  constructor(private fb: FormBuilder, private http: HttpClient, private alertify: AlertifyService, private authService: AuthService) {}

  ngOnInit() {
    this.createForm();
    if (localStorage.getItem('connection') === '1') {
      this.alertify.error('Network connectivity issue detected, reloading page, consider disconnecting and reconnecting your wifi');
      localStorage.setItem('connection', '0');
    }
    if (this.authService.loggedIn() === true)
      this.isShow = !this.isShow;
}

  createForm() {
    this.resetForm = this.fb.group({ email: [''] });
    this.passwordForm = this.fb.group({ password: [''] });
  }

  registerToggle() {
    this.registerMode = true;
  }

  newPassword() {
    this.user = Object.assign({}, this.passwordForm.value);
    this.alertify.success('Your password has been updated');
    this.authService.password(this.user);
  }

  forgotPassword() {
    this.user = Object.assign({}, this.resetForm.value);
    if(this.resetForm.value != null && this.resetForm.value != undefined)
      this.alertify.success('If you have entered a valid email address, an email will be sent with a link to reset your password');
    this.authService.reset(this.user);
  }

  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }
}
