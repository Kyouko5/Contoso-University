
# Contoso University Sample Application

This is a sample ASP.NET Core 9.0 MVC web application that demonstrates how to use Entity Framework Core in an ASP.NET Core MVC web application.

## Features

- Student enrollment and management
- Course creation and management
- Instructor assignment to courses
- Department organization
- Many-to-many and one-to-many relationship examples
- Entity Framework Core with SQL Server

## Database Schema

The application manages the following entities:

- Students
- Courses
- Instructors
- Departments
- Enrollments (join entity between Students and Courses)
- CourseAssignments (join entity between Instructors and Courses)
- OfficeAssignments (one-to-one with Instructors)

## Requirements

- .NET 9.0 SDK
- SQL Server (or LocalDB)

## Getting Started

1. Clone the repository
2. Ensure you have .NET 9.0 SDK installed
3. Update the connection string in `appsettings.json` if necessary
4. Run the application using `dotnet run`

## License

This project is open source and available under the [MIT License](LICENSE). 