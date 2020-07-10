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

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  isShow = false;
  resetForm: FormGroup;

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
  }

  registerToggle() {
    this.registerMode = true;
  }

  forgotPassword() {
    if(this.resetForm.value != null && this.resetForm.value != undefined)
      this.alertify.success('If you have entered a valid email address, an email will be sent with a link to reset your password');
    //window.location.reload();
  }

  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }
}
