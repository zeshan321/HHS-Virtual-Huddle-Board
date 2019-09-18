# HHS-Virtual-Huddle-Board
Virtual Huddle Board for Hamilton Health Sciences

Live demo: http://vhb.zeshanaslam.com

Video demo: https://zeshanaslam.com/hhsdemo

## Operating Instructions/Build Instructions
### Development Tools
- C# 7.1
- .NET CORE 2.0 ASP.NET MVC
- Server dependencies (Included but may need setup depending on computer):
  - Include="Microsoft.AspNetCore.All" Version="2.0.9"
  - Include="Microsoft.EntityFrameworkCore.Tools" Version="2.0.3"
  - Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="2.0.4"
  - Include="Newtonsoft.Json" Version="12.0.1"
  - Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.3"
- Web dependencies:
  - Already included and setup in project www root. No setup required.

### Database
Configuration location: web/HHSBoard/HHSBoard/appsettings.json

On first time setup do the following:
- Update database configuration to your SQL server.
- Comment out 'CreateRolesandUsersAsync' (line 102) and 'CreateDefaults' (line 103) method calls in Startup.cs.
- Open Package Manager Console or other tools to run migrations.
- Run update database command: "Update-Database"
- Once complete un-comment methods previously commented out and run.
- Done!

## Outstanding Issues
- Unable to edit create request for new record. User has to delete and make again. Perfectly works for Admins.
- HHS strategic goals only allows one option.
- No other known bugs.

## Future Plans
### What features would you like to add to the next release of the project if there is one?
Complete rewrite and more seamless editing experience.

### Are you willing to continue to work on this project (please provide preferred email addresses if so)? 
Yes:
- zeshan12aslam@gmail.com

### Are you willing to hand off this project (source code and all other materials) to other students at Mohawk to continue this project?
Yes.

### Are you willing to hand off this project (source code and all other materials) to your customer to continue the project? 
Yes.
