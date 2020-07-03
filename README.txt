We used this tutorial to build the skeleton of this app:

https://www.udemy.com/course/build-an-app-with-aspnet-core-and-angular-from-scratch/learn/lecture/8694276#overview

And this tutorial to edit CRUD operations and replace Entity Framework with self written code:

https://www.udemy.com/course/crud-application-using-c-sharp-and-sqlite/learn/lecture/13554544?start=150#overview

Login details (paid course):
murraylydie@gmail.com
Mwawada666

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

We are currently using Entity Framework and need to either write our own ORM to replace it or write code to replace the functions it performs.
We are missing some features to ensure a good grade on the Matcha project, including map location, multiple gender options, etc.