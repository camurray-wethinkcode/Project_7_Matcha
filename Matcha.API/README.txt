We are not allowed to use LINQ or Entity Framework as these are ORM's and we need to replace these with manually written code or with a self written ORM.

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

Places we use LINQ:
Controllers
	PhotosController
	WeatherForecastController (need to delete this)
	ValuesController
Data
	Seed
	DatingRepository
	DataContext
Helpers
	PagedList
	AutoMapperProfiles
Root
	Startup
