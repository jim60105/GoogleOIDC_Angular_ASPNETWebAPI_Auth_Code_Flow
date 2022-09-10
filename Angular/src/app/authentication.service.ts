import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

import jwtDecode from 'jwt-decode';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  constructor(private router: Router) {}

  oidcLogin(): void {
    const client: google.accounts.oauth2.CodeClient = google.accounts.oauth2.initCodeClient({
      client_id: environment.clientId,
      scope: 'openid profile email',
      ux_mode: 'redirect',
      redirect_uri: environment.apiUrl + '/Auth/oidc/signin',
      state: '12345GG',
    });
    client.requestCode();
  }

  getUser(idToken: string): Observable<{
    userID: string;
    userName: string;
    id: string;
    picture: string;
    email: string;
  }> {
    const { sub: userID, name: userName, id: id, picture, email } = jwtDecode<any>(idToken);
    console.log(idToken);
    return of({
      userID,
      userName,
      id,
      picture,
      email,
    });
  }
}
