We used this tutorial to build the skeleton of this app:

https://www.udemy.com/course/build-an-app-with-aspnet-core-and-angular-from-scratch/learn/lecture/8694276#overview

And this tutorial to edit CRUD operations and replace Entity Framework with self written code:

https://www.udemy.com/course/crud-application-using-c-sharp-and-sqlite/learn/lecture/13554544?start=150#overview

To run this code:
Ensure angular and the dotnet framework is installed (follow the tutorial).
Ensure DB Browser (SQLlite) is installed (follow the tutorial).
Open two terminal windows.
In the first window:
Navigate into Matcha.API and run "dotnet run".
In the second window:
Navigate into Matcha-SPA and run "npm install" (only needed once).
Then run "ng build" (only needed once).
Then run "ng serve".

For email:
docker run -d -p 1025:1025 -p 1080:1080 --name fake-smtp-server reachfive/fake-smtp-server