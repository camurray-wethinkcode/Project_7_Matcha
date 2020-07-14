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
import { Email } from '../_models/email';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';

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
  email: Email;
  password: string;
  isReset = true;
  token: string;

  constructor(private activatedRoute: ActivatedRoute, private fb: FormBuilder, private http: HttpClient, private alertify: AlertifyService, private authService: AuthService, private router: Router) {
  this.activatedRoute.queryParams.subscribe(params => {
        this.token = params['token'];
        if (this.token != null || this.token != undefined)
          this.isReset = !this.isReset;
    });
  }

  ngOnInit() {
    this.createForm();
    if (localStorage.getItem('flag') === '1') {
      this.isShow = false;
      this.isReset = true;
    }
    else if (localStorage.getItem('flag') === null) {
    }
    if (localStorage.getItem('connection') === '1') {
      this.alertify.error('Network connectivity issue detected, reloading page, consider disconnecting and reconnecting your wifi');
      localStorage.setItem('connection', '0');
    }
    if (this.authService.loggedIn() === true) {
      this.isShow = !this.isShow;
    }
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
    this.user.token = this.token;
    this.authService.password(this.user).subscribe(
      () => {
        this.alertify.error('Something went wrong here...');
        this.isShow = false;
        this.isReset = !this.isReset;
        localStorage.setItem('flag', '1');
      },
      error => {
        this.alertify.success('Your password has been updated');
        this.isShow = false;
        this.isReset = !this.isReset;
        localStorage.setItem('flag', '1');
      }
    );
  }

  forgotPassword() {
    this.email = Object.assign({}, this.resetForm.value);
    this.authService.reset(this.email).subscribe(
      () => {
        this.alertify.error('Something went wrong here...');
      },
      error => {
        this.alertify.success('If you have entered a valid email address, an email will be sent with a link to reset your password');
      }
    );
  }

  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }
}
