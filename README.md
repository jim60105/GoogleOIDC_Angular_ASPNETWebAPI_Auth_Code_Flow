# Google OAuth 2.0 OpenID Connect with Authorization Code Flow<br>(Angular + ASP<area>.NET Web API)

![google-oauth-angular-netcorewebapi](https://user-images.githubusercontent.com/16995691/189505017-c73c0d92-32ed-4899-8983-03948643f78f.png)

## Demo site

<https://googleoidcdemo.maki0419.com/>

## 介紹

使用 Angular 實現 OpenID Connect 登入，網路上的教學大都是使用隱含流程(Implicit flow)。\
隱含流程不需要後端，適合 Angular 這種純前端應用。\
它的設定比較簡單，設備維護成本低，但相較於授權碼流程(Authorization code flow)來說 ***隱含流程的安全性較低***。

在較為大型的 Angular 應用程式專案中，常會搭配後端API以處理資料庫或是複雜的應用邏輯。\
或許你不會為了 OIDC 而多開一台後端伺服器，但倘若只是把授權碼流程的一部份搬到現有的後端API，這就成為了一個划算的決定。

本專案使用 Angular + ASP<area>.NET Web API，示範如何在前後端分離的專案中實作 Google OAuth2 OIDC 登入。

- C# ASP<area>.NET Core 6 Web API 專案
  - OIDC僅使用 Google 官方的 `Google.Apis.Auth` 套件
- Angular 14 前端專案
  - OIDC僅使用 Google 官方的 `Google Sign-In` 客戶端套件

> 本文重點主要是在於實作，而非OAuth 2.0的流程講解\
> 如果想要深入學習，請參考保哥的課程 [《精通 OAuth 2.0 授權框架》](https://www.facebook.com/profile/100064322940906/search/?q=%E7%B2%BE%E9%80%9A%20OAuth%202.0%20%E6%8E%88%E6%AC%8A%E6%A1%86%E6%9E%B6)

## Try it out

- 在 [Google API Console](https://console.developers.google.com/) 註冊新的專案，「憑證→OAuth 2.0 用戶端 ID→網頁應用程式」取得「用戶端編號」、「用戶端密碼」
- 在 「已授權的重新導向 URI」填入 `https://localhost:7091/api/Auth/oidc/signin`
- Git clone

  ```bash
  git clone https://github.com/jim60105/GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow.git
  ```

- ASP<area>.NET Web API
  - Visual Studio 啟動 `ASPNET_WebAPI/GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow.sln`
  - 修改 `ASPNET_WebAPI\appsettings.json` 檔案
    - 在 `YOUR CLIENT ID` 填入「用戶端編號」
    - 在 `YOUR CLIENT SECRET` 填入「用戶端密碼」
  - 以 Debug 模式啟動但不偵錯 (Ctrl+F5)，Swagger 將啟動在 <https://localhost:7091>
- Angular
  - Visual Studio Code 啟動 `Angular` 目錄
  - 修改 `Angular\src\environments\environment.ts` 檔案
    - 在 `YOUR CLIENT ID` 填入「用戶端編號」
  - npm install 並啟動伺服器

    ```bash
    npm install
    npm run-script start
    ```

- 訪問 <https://localhost:4200/>

## 套件安裝

- ASP<area>.NET Core Web API
  - 安裝 nuget 套件: [Google.Apis.Auth](https://www.nuget.org/packages/Google.Apis.Auth/)
- Angular
  - 引用 [Google Sign-In](https://developers.google.com/identity/gsi/web/guides/client-library) 客戶端套件庫
  <https://github.com/jim60105/GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow/blob/559fdc47724bb6a3a9848c6639399e2572ae8f84/Angular/src/index.html#L11>
  - 安裝 npm 套件 [@types/google.accounts](https://www.npmjs.com/package/@types/google.accounts), [jwt-decode](https://www.npmjs.com/package/jwt-decode)

    ```bash
    npm install --save-dev @types/google.accounts
    npm install --save jwt-decode
    ```

  - 在 `Angular\tsconfig.app.json` 中的 `compilerOptions:types` 中加入 `"google.accounts"`
    <https://github.com/jim60105/GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow/blob/559fdc47724bb6a3a9848c6639399e2572ae8f84/Angular/tsconfig.app.json#L2-L10>

## 授權流程

1. 用戶在前端按下 Sign in with Google 按鈕
2. 以 gsi 客戶端套件啟動授權碼流程
   <https://github.com/jim60105/GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow/blob/559fdc47724bb6a3a9848c6639399e2572ae8f84/Angular/src/app/authentication.service.ts#L14-L23>
3. 導向至 Google OAuth 授權頁面
4. (使用者同意後)，導向至 `後端API/Auth/oidc/signin`，Model Binding 取得授權碼\
若是使用者拒絕，或是發生了任何失敗，`error` 參數就會接到內容
   <https://github.com/jim60105/GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow/blob/559fdc47724bb6a3a9848c6639399e2572ae8f84/ASPNET_WebAPI/Controllers/AuthController.cs#L24-L40>
5. 以授權碼去要回 idToken
   <https://github.com/jim60105/GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow/blob/559fdc47724bb6a3a9848c6639399e2572ae8f84/ASPNET_WebAPI/Services/OIDCService.cs#L33-L51>
6. 導向回前端，將 idToken 以網址參數傳給 Angular
   <https://github.com/jim60105/GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow/blob/559fdc47724bb6a3a9848c6639399e2572ae8f84/ASPNET_WebAPI/Controllers/AuthController.cs#L39>
7. Angular 前端應用程式接到 idToken
   <https://github.com/jim60105/GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow/blob/559fdc47724bb6a3a9848c6639399e2572ae8f84/Angular/src/app/app.component.ts#L22-L29>
8. 將 idToken 做 JWT decode，取得內容物
   <https://github.com/jim60105/GoogleOIDC_Angular_ASPNETWebAPI_Auth_Code_Flow/blob/559fdc47724bb6a3a9848c6639399e2572ae8f84/Angular/src/app/authentication.service.ts#L32>

## 參考資料

- [Google API Console](https://console.developers.google.com/)
- [Verify the Google ID token on your server side  |  Sign In With Google  |  Google Developers](https://developers.google.com/identity/gsi/web/guides/verify-google-id-token)
- [Using OAuth 2.0 for Web Server Applications  |  Google Identity  |  Google Developers](https://developers.google.com/identity/protocols/oauth2/web-server)
- [OpenID Connect  |  Google Identity  |  Google Developers](https://developers.google.com/identity/protocols/oauth2/openid-connect)
- [Namespace Google.Apis.Auth | Google API support libraries](https://googleapis.dev/dotnet/Google.Apis.Auth/latest/api/Google.Apis.Auth.html)

## 延伸閱讀

- [LINE Login OpenID Connect Demo Project (ASP.NET Core 6 MVC)](https://github.com/jim60105/LINELoginOIDCDemo)
- [LINE Login OpenID Connect Implementation Demo Project (ASP.NET Core 6 MVC)](https://github.com/jim60105/LINELoginOIDCImplementationDemo)
- [LINE Login OpenID Connect Demo Project (ASP.NET MVC5, .NET Framework 4.8)](https://github.com/jim60105/LINELoginOIDCDemo_MVC5)
