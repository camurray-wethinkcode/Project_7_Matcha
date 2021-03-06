import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
    { value: 'other', display: 'Other' }
  ];
  userParams: any = {};
  pagination: Pagination;

  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      var i = 6;
      var j = 6;
      if (localStorage.getItem('blockedlist')) {
          if (localStorage.getItem('blockedlist') === data['users'].result[0].id.toString()) {
            i = 0;
            j = 4;
          }
          else if (localStorage.getItem('blockedlist') === data['users'].result[1].id.toString()) {
            i = 1;
            j = 3;
          }
          else if (localStorage.getItem('blockedlist') === data['users'].result[2].id.toString()) {
            i = 2;
            j = 4;
          }
          else if (localStorage.getItem('blockedlist') === data['users'].result[3].id.toString()) {
            i = 3;
            j = 1;
          }
          else if (localStorage.getItem('blockedlist') === data['users'].result[4].id.toString()) {
            i = 4;
            j = 0;
          }
        }
      this.users = data['users'].result;
      if (i != 6) {
        this.users[i] = this.users[j];
      }
      this.pagination = data['users'].pagination;
    });

    if (this.user.gender === 'female')
      this.userParams.gender = 'female';
    else if (this.user.gender === 'male')
      this.userParams.gender = 'male';
    else if (this.user.gender === 'other')
      this.userParams.gender = 'other';
    //this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.orderBy = 'lastActive';
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  resetFilters() {
    if (this.user.gender === 'female')
      this.userParams.gender = 'female';
    else if (this.user.gender === 'male')
      this.userParams.gender = 'male';
    else if (this.user.gender === 'other')
      this.userParams.gender = 'other';
    //this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.loadUsers();
  }

  loadUsers() {
    this.userService
      .getUsers(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.userParams
      )
      .subscribe(
        (res: PaginatedResult<User[]>) => {
            this.users = res.result;
            this.pagination = res.pagination;
        },
        error => {
          this.alertify.error(error);
        }
      );
  }
}
