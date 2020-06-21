We are not allowed to use Entity Framework as it is an ORM's and we need to replace this with manually written code or with a self written ORM.

Places we use Entity Framework:
Controllers
	ValuesController
Data
	DatingRepository
	AuthRepository
Helpers
	PagesList
Migrations
	All files in this folder
Root
	Program
	Startup

I found two tutorials that can help with replacing this code:
https://www.youtube.com/watch?v=1IFS33sPDhE#:~:text=first%20of%20all%20we%20have,you%20want%20to%20save%20this.
https://www.youtube.com/watch?v=EyrKUSwi4uI

More reading on CRUD operations wiithout entity:
https://www.completecsharptutorial.com/mvc-articles/insert-update-delete-in-asp-net-mvc-5-without-entity-framework.php
https://www.aspsnippets.com/Articles/Implement-CRUD-operations-without-using-Entity-Framework-in-ASPNet-MVC.aspx
http://www.codesolution.org/tag/crud-operations-in-asp-net-mvc-4-without-entity-framework/

The first thing we need to do is replace entity framework with self written CRUD operations as we will fail if we cheat and use an ORM.
We should add a footer to our page, perhaps with our usernames.
We need to add an email on signup and do email verification with a link.
We should change the known as box to first name and add a last name box on signup as well.
We should add a photo upload on signup or notify a user to upload a photo on first login.
We should add a non binary gender option to select from on signup and in search.
We should add a map to location in profile and need geolocation to auto detect where the person is even without their permission.
We should add a location option to the search.
We should add an interest tag option to the search.
We should add an online now and last seen to profiles.
And pray to God we can get all this done in time...
This is a brand new stack to me, and Marchall, I hope you know this stuff Matthew.
