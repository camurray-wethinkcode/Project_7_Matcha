import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination, PaginatedResult } from '../_models/pagination';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';
  i: number = 0;
  a: string;
  b: string;
  c: string;
  d: string;
  e: string;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    if (this.i === 0) {
      window.location.reload();
    }
    this.route.data.subscribe(data => {
      this.messages = data['messages'].result;
      if (this.messages[0] === undefined) {
        this.i = 0;
      }
      else if (this.messages[1] === undefined) {
        this.a = this.messages[0].isRead.toString();
        if (this.a === 'false') {
          this.i = 1;
        }
      }
      else if (this.messages[2] === undefined) {
        this.b = this.messages[1].isRead.toString();
        if (this.a === 'false' && this.b === 'false') {
          this.i = 2;
        }
      }
      else if (this.messages[3] === undefined) {
        this.c = this.messages[2].isRead.toString();
        if (this.a === 'false' && this.b === 'false' && this.c === 'false') {
          this.i = 3;
        }
      }
      else if (this.messages[4] === undefined) {
        this.d = this.messages[3].isRead.toString();
        if (this.a === 'false' && this.b === 'false' && this.c === 'false' && this.d === 'false') {
          this.i = 4;
        }
      }
      else {
        this.e = this.messages[4].isRead.toString();
        if (this.a === 'false' && this.b === 'false' && this.c === 'false' && this.d === 'false' && this.e === 'false') {
          this.i = 5;
        }
      }
      localStorage.setItem('count', this.i.toString());
      this.pagination = data['messages'].pagination;
    });
  }

  loadMessages() {
    this.userService
      .getMessages(
        this.authService.decodedToken.nameid,
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.messageContainer
      )
      .subscribe(
        (res: PaginatedResult<Message[]>) => {
          this.messages = res.result;
          this.pagination = res.pagination;
        },
        error => {
          this.alertify.error(error);
        }
      );
  }

  deleteMessage(id: number) {
    this.alertify.confirm(
      'Are you sure you want to delete this message?',
      () => {
        this.userService
          .deleteMessage(id, this.authService.decodedToken.nameid)
          .subscribe(
            () => {
              this.messages.splice(
                this.messages.findIndex(m => m.id === id),
                1
              );
              this.alertify.success('Message has been deleted');
            },
            error => {
              this.alertify.error('Failed to delete the message');
            }
          );
      }
    );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }
}
