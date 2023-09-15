# User Api Ux

This Repo uses .Net and MongoDB for backend applications.

The application has User and Document Services. User service manipulates users' information and Document service lets users upload, update, and delete their documents.

To run the application, firstly you need to create a MongoDB database. 
The database name and collection names are set below. 
If you create those with different names and different ports, you need to change those values. 
They are dynamically set in startup.cs under the region "Settings Defined".

![Appsettings Db Setting](https://github.com/MuratS4hin/user-api-ux/assets/73753725/0c0aa2bd-9da5-40d5-a64a-60bd8087a520)

After running the code, the swagger should open. Some of the request does not need authorization but some does. To authorize and get a JWT token, you need to register. 

![post](https://github.com/MuratS4hin/user-api-ux/assets/73753725/9adbe66b-6ff6-454e-94a4-8a3cfda5f037)

Here userRole can be "Admin" or "User". It's not the best usage, but it's used this way for simplicity. After you register, the Login request should be used with the registered information to get the JWT token.

![login](https://github.com/MuratS4hin/user-api-ux/assets/73753725/74b29b5d-b907-44c8-8e6f-d86fabde8494)

To authorize, you need to press the authorize button at the top and paste the JWT token into the area by writing Bearer in front, such as "Bearer jwt-token-here"

![Authorize](https://github.com/MuratS4hin/user-api-ux/assets/73753725/b50eddbb-890e-4561-bf08-5eb6eecb4d6a)


Thanks for your interest
