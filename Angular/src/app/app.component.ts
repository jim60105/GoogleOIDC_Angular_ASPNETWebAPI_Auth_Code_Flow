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
        // 直接解開JWT Token
        this.authenticationService.extractToken(this.idToken).subscribe((res) => {
          this.user = res;
        });

        // 如果需要在前端驗證Token，可以使用這個Google API
        this.authenticationService.verifyToken(this.idToken).subscribe({
          next: (data) => {
            console.log('This is verified by Google', data);
          },
          error: (error) => {
            console.log(error);
          },
        });

        // TODO: 接下來可以將idToken存到localStorage，並且在每次發送API請求時，將idToken放到Authorization Header。後端只要用一樣的方式驗證idToken即可確認身份。
      }
    });
  }

  // 按鈕
  signInWithGoogle() {
    this.authenticationService.oidcLogin();
  }
}
