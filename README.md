# app-center

This is a console application that will help you run a build and get data about the builds of your branches  

**How to use the project:**
1. Clone this repository on your local machine
2. To use the project you should have a Visual Studio 2019
3. Go to App.config file and enter the following fields:
```
<configuration>
  <appSettings>
    <add key="OwnerName" value="YourOwnerName" />
    <add key="AppName" value="YourAppName" />
    <add key="ApiToken" value="YourApiToken" />
  </appSettings>
</configuration>
```
To get your Api Token you should following steps:
1. Go to App Center
2. Go to Accaunt Setting
3. Go to User Api Tokens
4. Create new Api Token and copies it
