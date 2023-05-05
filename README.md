# Azure AD Authentication & Authorization
## On Behalf Of User Flow (calling an api from another api)

In the microservices world, it is common to see apis call different apis either in the same
environment or in different environment. Just like in the topic of this tutorial, i will outline a tiny
sample of working of that case in the area of authentication and authorization using Azure AD MSAL for Web.


![Technical-Architecture](/Assets/2023-04-29_02h39_44.png)

We will have a simple web app that calls an api named api #1 (for example) and this api #1 
will call api #2 as well. If we talk about access_token, if those apis are in same app registration we
won't face a difficulty as we can utilize the same scope(s) inside the access_token generated by 
the calling web app. But, what if api #1 and api #2 are in the different app registrations with
different client ids etc? 
we have 2 solutions for that. 
 - The simplest but less secure, is we can use client credentials inside the 
api #1 to call api #2. This technique will result in non-user based access_token or we can say -> machine to machine
communication. 
- Our solution in this tutorial is to use on behalf of user flow technique where we can use access_token from user itself. 

How does it work ? Just like the diagram above. User will be prompted to login to the web app-Azure Ad. 
After that, in the background the web app will get the access_token for that user with scopes to access api #1. This token
will be maintaned by MSAL for Web so that we don't need to care about refreshing the token if it expires. 
After api #1 validated the previous access_token, to access api #2 (inside api #1), it can't use the same access_token generated in web app.
That's why, api #1 needs to request new access_token for that user using user-assertion with the new scopes to access 
api #2 (this process uses MSAL Library). After that process is finished, we can save that token in any cache storage.

All right, let's build a sample application called **Data Security App**.

This app, will have 2 controllers, Hash controller, and Cryptography (Encrpt/Decrypt) Controller by invoking Api #1 (act as a gateway). The Hash Controller resides purely in 
Api #1 (no need to call Api #2). But, Cryptography Controller in Api #1 just a controller calling Api #2 using Http Client. 
So, when the user needs to call Cryptography Controller, there will be On Behalf Of Flow Process there to generate new access_token for accessing 
the Api #2.

![Data-Security-App-Architecture](/Assets/2023-04-29_02h48_00.png)


### Azure AD Portal Configuration

Ok, let's configure the Azure AD Portal first by creating 3 app registrations (Web App, Web Api #1 and Web Api #2).

![Azure-AD-Portal](/Assets/2023-04-29_02h50_59.png)

In Azure AD Portal App Registration, i have created 3 app registrations with those names in the picture above.
Let's configure the **Obo.Api.Two (Api #2)** first. You can use whatever names you like in your own Azure AD.

![Obo-Api-Two](/Assets/2023-04-29_02h51_24.png)

In the **Obo.Api.Two (Api #2)**, please take a note on *Client Id* and *Tenant Id* to be used in appsettings for Obo.Api.Two.

Now, go to **Expose an Api** menu. In there, you can create a scope like mine. 

![Obo-Api-Two-Scope1](/Assets/2023-04-29_02h52_13.png)

![Obo-Api-Two-Scope2](/Assets/2023-04-29_02h53_18.png)

I created a scope with name Obo.Api.Two for it. You can create any name you like.

After that, we can add authorized client appplications (optional). This will be your Api #1 as it will call the Api #2.
Just click Add a Client Application button and add Client Id of Api #1 (Obo.Api.One in mine) and select the scope you have created.

![Obo-Api-Two-Scope2](/Assets/2023-04-29_02h54_14.png)



All right, we have configured the **Obo.Api.Two**, now let's configure the **Obo.Api.One**.

![Obo-Api-One](/Assets/2023-04-29_02h55_19.png)

Take a note both *Client Id* and *Tenant Id* for later use. And now, we can navigate to **Certificate and Secrets** menu.
In this menu, you should generate a secret to be used to call Api #2 (**Obo.Api.Two**). Don't forget to copy the value after generating it.

![Obo-Api-One-Secret](/Assets/2023-04-29_02h56_05.png)

![Obo-Api-One-Secret](/Assets/2023-04-29_02h56_29.png)

Now, let's jump to **Api Permissions** menu to add the permission for **Obo.Api.Two** scope that we have created earlier.

![Obo-Api-One-Permission](/Assets/2023-04-29_02h56_51.png)

You can hit the Add Permission button > My Apis Tab > Select Obo.Api.Two scope and add it. Don't forget to hit **Grant admin consent for default directory** button.

![Obo-Api-One-Permission](/Assets/2023-04-29_02h57_25.png)

After adding scope permission, now go to **Expose an Api** menu to create a scope for **Obo.Api.One**. This will be used by 
the calling web app.

![Obo-Api-One-Expose-Scope](/Assets/2023-04-29_02h59_33.png)

Hit the **Add a scope** button > Add the scope with name Obo.Api.One (follow the screenshot) and hit **Save**.

![Obo-Api-One-Expose-Scope](/Assets/2023-04-29_03h00_08.png)

Follow the step like in the **Obo.Api.Two** to add authorized client app. This client id here must use client id of calling web app.


![Obo-Api-One-Expose-Scope](/Assets/2023-04-29_03h01_16.png)

The last step, we will configure the calling web app which is **Obo.Web.App**.
Take a note on both *Client Id* and *Tenant Id* for later use as well.

![Obo-Web-App](/Assets/2023-04-29_03h02_27.png)

Go to **Certificates & Secrets** menu to create the client secret to call **Obo.Api.One**.

![Obo-Web-App-Secret](/Assets/2023-04-29_03h03_08.png)

![Obo-Web-App-Secret](/Assets/2023-04-29_03h03_24.png)

Go to **API Permisions** menu and add both **Obo.Api.One** and **Obo.Api.Two** scopes and don't forget to hit 
**Grant admin consent for Default Directory** button.

![Obo-Web-App-Permission](/Assets/2023-04-29_03h03_38.png)

![Obo-Web-App-Permission](/Assets/2023-04-29_03h04_07.png)

Now, we are jumping to the most important part. Configuring the Authentication by adding web url of our web web app (**Obo.Web.App**).
Go to **Authentication** menu and add the localhost urls in the web application section.

![Obo-Web-App-Authentication](/Assets/2023-04-29_03h05_05.png)



### Visual Studio Configuration and Code Explanation


![VS](/Assets/2023-04-29_03h06_02.png)

Azure AD Portal configuration has finished. Let's jump to the solution in VS. There, you can see we have 3 projects
that are 
 - **Obo.Web.App => ASP.NET Core MVC Web App** 
 - **Obo.Api.One => Api #1 (ASP.NET Core API)** 
 - **Obo.Api.Two => Api #2 (ASP.NET Core API)**



 ### Obo.Api.Two Project

Let's go and take a look on Obo.Api.Two for details.
First, make sure install the packages listed in the screenshot:

![Obo-Api-Two](/Assets/2023-04-29_03h14_27.png)

After that, don't forget to replace the following information in the appsettings.json.

![Obo-Api-Two](/Assets/2023-04-29_03h11_46.png) 

Now, we can see our own Program.cs configuration to see how we register the Web Api Authentication/Authorization process.

![Obo-Api-Two](/Assets/2023-04-29_03h13_40.png)

`services.AddMicrosoftIdentityWebApiAuthentication(configuration)` is line of code to register the MSAL for web api that handles
token validation (JWT), refreshing the token and etc. The add scoped for *CryptoHelper* below it is used to register the helper class.

For the controller, make sure you add RequiredScope attribute to make sure the calling app has the **Obo.Api.Two** scope.

![Obo-Api-Two](/Assets/2023-04-29_03h15_28.png)


 ### Obo.Api.One Project

 For this project, the configuration is mostly the same with **Obo.Api.Two**. Let's take a look on this.
 First, make sure install the packages listed in the screenshot:

![Obo-Api-One](/Assets/2023-04-29_03h16_34.png)

After that, don't forget to replace the following information in the appsettings.json.

![Obo-Api-One](/Assets/2023-04-29_03h16_51.png) 

Now, we can see our own Program.cs configuration to see how we register the Web Api Authentication/Authorization process.

![Obo-Api-One](/Assets/2023-04-29_03h20_50.png)

`services.AddMicrosoftIdentityWebApiAuthentication(configuration)` is line of code to register the MSAL for web api that handles
token validation (JWT), refreshing the token and etc. 

`JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();` will clear default claim types. This will be used to
map incoming claims from Azure Ad.

`services.AddDistributedMemoryCache();` is used to register distributed caching to save our token after doing user-assertion (on behalf of user flow) process.

`services.AddScoped<ITokenHelper, TokenHelper>();` - registered the TokenHelper class used to user assertion process (getting access_token, save it to the cache and use it for calling **Obo.Api.Two**).

`services.AddHttpClient<CryptoHttpService>(options =>
            {
                options.BaseAddress = new Uri(configuration["OboApiTwo:BaseUrl"]);
            });` This code used to register CryptoHttpService. A custom http service to invoke request to **Obo.Api.Two** endpoint.

` services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, config =>
            {
                config.MapInboundClaims = true;
            });` This code used to configure JwtBearerOptions to map inbound claims.

The most important part in **Obo.Api.One** project is TokehHelper.cs class that used to perform user assertion using current access token and request
new access token using scope for **Obo.Api.Two** endpoint.

![Obo-Api-One](/Assets/2023-04-29_03h18_30.png)

![Obo-Api-One](/Assets/2023-04-29_03h19_14.png)


Basically, it will check in the cache storage whether or not it has existing access token for current username
that's still valid/not expired. If it exists and not expired, it will return the existing access token and used it
when making request to **Obo.Api.Two** endpoint.

![Obo-Api-One](/Assets/2023-04-29_03h19_25.png)

If not, then it will perform user assertion and by means of MSAL Confidential Client Application, it will request
a new token with scope for **Obo.Api.Two** on behalf of current user then it will be saved in cache.


 ### Obo.Web.App Project

















