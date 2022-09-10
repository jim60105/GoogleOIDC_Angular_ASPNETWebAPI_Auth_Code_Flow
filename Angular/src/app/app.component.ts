import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { AuthenticationService } from './authentication.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow';
  user = { userID: '', userName: '', id: '', picture: '', email: '' };
  idToken = '';

  constructor(
    public authenticationService: AuthenticationService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params: ParamMap) => {
      this.idToken = params.get('idToken') || '';
      if (this.idToken) {
        this.authenticationService.getUser(this.idToken).subscribe((res) => {
          this.user = res;
        });
      }
    });
  }

  signInWithGoogle() {
    this.authenticationService.oidcLogin();
  }
}
