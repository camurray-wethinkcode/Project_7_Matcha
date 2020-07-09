import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  isShow = false;

  constructor(private http: HttpClient, private alertify: AlertifyService, private authService: AuthService) {}

  ngOnInit() {
    if (localStorage.getItem('connection') === '1') {
      this.alertify.error('Network connectivity issue detected, reloading page, consider disconnecting and reconnecting your wifi');
      localStorage.setItem('connection', '0');
    }
    if (this.authService.loggedIn() === true)
      this.isShow = !this.isShow;
}

  registerToggle() {
    this.registerMode = true;
  }

  forgotPassword() {
  }

  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }
}
