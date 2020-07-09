import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import {
  NgxGalleryOptions,
  NgxGalleryImage,
  NgxGalleryAnimation
} from 'ngx-gallery';
import { TabsetComponent } from 'ngx-bootstrap';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  user: User;
  nophoto: number = 0;
  userParams: any = {};
  isClicked = false;
  likeName = 'Like';
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
      if (localStorage.getItem('nophoto') === '1') {
        this.nophoto = 1;
        this.alertify.error('You must upload atleast one photo before you can contact other users');
      }
    });

    this.route.queryParams.subscribe(params => {
      const selectedTab = params['tab'];
      this.memberTabs.tabs[selectedTab > 0 ? selectedTab : 0].active = true;
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];
    this.galleryImages = this.getImages();
  }

  getImages() {
    const imageUrls = [];
    for (const photo of this.user.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
        description: photo.description
      });
    }
    return imageUrls;
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  toggleLike(id: number) {
    this.isClicked = !this.isClicked;
    if (this.isClicked === true) {
      this.sendLike(this.user.id);
      this.likeName = 'Unlike';
    }
    else {
      this.sendUnlike(this.user.id);
      this.likeName = 'Like';
    }
  }

  sendLike(id: number) {
    this.userService.getUser(this.authService.decodedToken.nameid).subscribe(data => {
      data.fameRating = data.fameRating + 1;
      this.userService.updateUser(this.authService.decodedToken.nameid, data).subscribe(data => {
        this.alertify.success('You have increased your fame!');
      }, error => {
        this.alertify.error('Something went wrong');
      })
    }, error => {
      console.log('Nope');
    });
    this.userService.sendLike(this.authService.decodedToken.nameid, id).subscribe(data => {
      this.alertify.success('You have liked: ' + this.user.username);
    }, error => {
      this.alertify.error(error);
    });
  }

  sendUnlike(id: number) {
    this.userService.sendUnlike(this.authService.decodedToken.nameid, id).subscribe(data => {
      this.alertify.success('You have unliked: ' + this.user.username);
    }, error => {
      this.alertify.error(error);
    });
  }

  blockUser(id: number) {
    localStorage.setItem('blockedlist', id.toString());
    this.alertify.success('User added to blocked list and will no longer appear in search');
    this.router.navigate(['/home']);
  }

  reportUser(id: number) {
    localStorage.setItem('reportedlist', id.toString());
    this.alertify.success('User reported to admin, user will be banned from platform and unable to login');
    this.router.navigate(['/home']);
  }
}