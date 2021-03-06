import { Component, ChangeDetectorRef, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef, NgZone, HostListener } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import {
  FormGroup,
  FormControl,
  FormBuilder
} from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { AgmCoreModule } from '@agm/core';
import { MapsAPILoader, MouseEvent } from '@agm/core';
import { } from 'google-maps';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  user: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;
  latitude: number;
  longitude: number;
  zoom: number;
  address: string;
  city: string;
  country: string;
  private geoCoder;
  flag: number = 1;

  @ViewChild('search', {static:true})
  public searchElementRef: ElementRef;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red'
    };
    this.createRegisterForm();
    this.setCurrentLocation();
    //load Places Autocomplete
    this.mapsAPILoader.load().then(() => {
      this.setCurrentLocation();
      this.geoCoder = new google.maps.Geocoder();
      let autocomplete = new google.maps.places.Autocomplete(this.searchElementRef.nativeElement);
      autocomplete.addListener("place_changed", () => {
        this.ngZone.run(() => {
          //get the place result
          let place: google.maps.places.PlaceResult = autocomplete.getPlace();

          //verify result
          if (place.geometry === undefined || place.geometry === null) {
            return;
          }

          //set latitude, longitude and zoom
          this.latitude = place.geometry.location.lat();
          this.longitude = place.geometry.location.lng();
          this.zoom = 12;
        });
      });
    });
    this.cdr.detectChanges();
  }

  private setCurrentLocation() {
    if ('geolocation' in navigator) {
      navigator.geolocation.getCurrentPosition((position) => {
        this.latitude = position.coords.latitude;
        this.longitude = position.coords.longitude;
        this.zoom = 8;
        this.getAddress(this.latitude, this.longitude);
      });
    }
  }

  markerDragEnd($event: MouseEvent) {
    console.log($event);
    this.latitude = $event.coords.lat;
    this.longitude = $event.coords.lng;
    this.getAddress(this.latitude, this.longitude);
  }

  getAddress(latitude, longitude) {
    if (this.geoCoder === undefined || this.geoCoder === null) {
      localStorage.setItem('connection', '1');
      window.location.reload(); 
    }
    this.geoCoder.geocode({ 'location': { lat: latitude, lng: longitude } }, (results, status) => {
      if (status === 'OK') {
        if (results[0]) {
          this.zoom = 12;
          this.flag = 0;
          this.address = results[0];
          var countrystr = results[5].formatted_address;
          this.country = countrystr.split(',', 3)[2];
          var countrytrim = this.country;
          this.country = countrytrim.trim();
          var citystr = results[5].formatted_address;
          this.city = citystr.split(',', 1)[0];
        } else {
          window.alert('No results found');
        }
      } else {
        window.alert('Geocoder failed due to: ' + status);
      }
    });
  }

  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        gender: ['male'],
        sexuality: ['bisexual'],
        username: [''],
        name: [''],
        surname: [''],
        dateOfBirth: [null],
        city: [''],
        country: [''],
        email: [''],
        password: [''],
        confirmPassword: [''],
        introduction: [''],
        lookingFor: [''],
        interests: ['']
      },
      { validator: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value
      ? null
      : { mismatch: true };
  }

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.user.activated = 0;
      if (this.flag === 0) {
        this.user.city = this.city;
        this.user.country = this.country;
      }
      this.authService.register(this.user).subscribe(
        () => {
          localStorage.setItem('nophoto', '1');
          this.alertify.success('Registration successful');
        },
        error => {
          this.alertify.error(error);
        },
        () => {
          this.alertify.error('Please login');
          window.location.reload();
        }
      );
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
