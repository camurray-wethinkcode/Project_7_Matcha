import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;
  likeName = 'Like';
  isClicked = false;

  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
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

}