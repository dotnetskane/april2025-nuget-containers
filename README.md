# .NET Skåne April 2025 meetup - Code alone
This time, we’re diving into two essential skills every .NET developer should have in their toolkit. Creating NuGet packages and containerizing .NET apps without dockerfiles. This hands-on code-along session is perfect for anyone who enjoys learning through live coding and step-by-step demonstrations. Whether these topics are new to you or you’re just looking for a refresher, this session will give you practical tips and help you level up your .NET skills. To get the most out of this session, be sure to bring your laptop—let’s code together!

# 1. Creating a NuGet package
First start by setting up a new folder for your project. You can do this in Windows Explorer or using PowerShell.

cd D:\Playground\dotnetskane\dotnetSkane-April2025

Then navigate to the folder and create a new solution file.
```powershell	
dotnet new sln -n dotnetSkane-April2025 -f slnx
```

Create the API project:

```powershell
dotnet new webapi -n MyApi -o .\src\MyApi
```

Add the project to the solution: 
```powershell
dotnet sln add .\src\MyApi\MyApi.csproj 
```

Run the project: 
```powershell
dotnet run --project .\src\MyApi\MyApi.csproj
```

Make sure the Api is up and running by sending a request to the API using Powershell or curl.
```powershell
  Invoke-RestMethod -Uri http://localhost:5041/weatherforecast | Format-Table
  Invoke-RestMethod -Uri http://localhost:5041/weatherforecast | ConvertTo-Json
  curl http://localhost:5041/weatherforecast
```

## Create NuGet package
Create a folder called packages in the root of the solution. This is where we will store our local NuGet packages.

```powershell
mkdir D:\packages
```

Update the NuGet.config file in the root of the solution to include the local package source. This will allow us to create and consume packages from our local feed.
```powershell
dotnet nuget add source packages -n local
```

Create new project called MyPackage and add it to the solution. This will be our NuGet package.

```powershell
dotnet new classlib -n MyPackage -o .\src\MyPackage
dotnet sln add .\src\MyPackage\MyPackage.csproj
```

Add the following the MyPackage.csproj file:
```xml
<ItemGroup>
  <FrameworkReference Include="Microsoft.AspNetCore.App" />
</ItemGroup>
```

Create a new file in MyPackage called WeatherEndpoints.cs and add 
```csharp
using Microsoft.AspNetCore.Builder;

namespace MyPackage;

public static class WeatherEndpoints
{
    public static void MapWeatherEndpoints(this WebApplication app)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", () =>
        {
            var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast");
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```

## Licence
It's good practice to include a LICENCE file.
Create a file called LICENCE in the MyPackage folder and then add the following to the file

```text
MIT License

Copyright (c) 2025 Peter Nylander

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

Then add the following to MyPackage.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <VersionPrefix>1.0.0</VersionPrefix>
    <PackageLicenseFile>LICENSE</PackageLicenseFile>
  </PropertyGroup>

  <ItemGroup>
    <None Include="LICENSE" Pack="true" PackagePath="" />
  </ItemGroup>

  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>
  
</Project>
```

It is also good practice to include a README.md file so let's add one. The content doesn't matter right now.
Modify the .csproj file and add:
```xml
  <PropertyGroup>
    <PackageReadmeFile>README.md</PackageReadmeFile>
  </PropertyGroup>    

  <ItemGroup>
    <None Include="README.md" Pack="true" PackagePath="" />
  </ItemGroup>
```

### Use our package from our local package feed
Remove the reference to MyPackage class library from the MyApi.csproj
```xml
  <ItemGroup>
    <ProjectReference Include="..\MyPackage\MyPackage.csproj" />
  </ItemGroup>
```

In Visual Studio, right-click on the MyApi project and select "Manage NuGet packages". Or use the command line:
```powershell
dotnet package add MyPackage -v 1.0.0 --project .\src\MyApi.csproj -s local
```


Make sure you can see your README file and the license in the UI.
Then build the app and test as before. You should see the weather forecast.