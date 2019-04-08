# HHS-Virtual-Huddle-Board
Virtual Huddle Board for Hamilton Health Sciences

Live demo: Coming soon

# Development Tools
- .NET CORE 2.0 ASP.NET MVC
- Server dependencies:
  - Include="Microsoft.AspNetCore.All" Version="2.0.9"
  - Include="Microsoft.EntityFrameworkCore.Tools" Version="2.0.3"
  - Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="2.0.4"
  - Include="Newtonsoft.Json" Version="12.0.1"
  - Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.3"
- Web dependencies:
  - Already included and setup in project www root. No setup required.

# Database
Configuration location: web/HHSBoard/HHSBoard/appsettings.json

On first time setup do the following:
- Update database configuration to your SQL server.
- Comment out 'CreateRolesandUsersAsync' (line 102) and 'CreateDefaults' (line 103) method calls in Startup.cs.
- Open Package Manager Console or other tools to run migrations.
- Run update database command: "Update-Database"
- Once complete un-comment methods previously commented out and run.
- Done!
