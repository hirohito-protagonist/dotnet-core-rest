# dotnet-core-rest 

This project is for educational purpose. It covers build REST API in .Net Core 2 and Entity Framework Core.

___

# Overview

This REST API is example an book library manager to showcasing common patterns and techniques.

Included
- [CRUD](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete) operation for authors and books
- [DTO](https://en.wikipedia.org/wiki/Data_transfer_object) pattern via [AutoMapper](http://automapper.org/)
- [The Repository pattern](https://msdn.microsoft.com/en-us/library/ff649690.aspx)
- [JSON PATCH](http://jsonpatch.com/) resource for partially update
- Paging feature
- Data validation
- Logging
- [HATEOAS](https://en.wikipedia.org/wiki/HATEOAS) via Accept header `vnd.hateoas+json`
- [Swagger UI](https://swagger.io/swagger-ui/) for discover API
- HTTP Caching
- Control rate of allowed requests by [AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit)
- Integration test

# Prerequisites

* OS X, Windows or Linux
* [.NET Core](https://www.microsoft.com/net/core) and [.NET Core SDK](https://www.microsoft.com/net/core)
* [Visual Studio Code](https://code.visualstudio.com/) with [C# extension](https://github.com/OmniSharp/omnisharp-vscode) (or Visual Studio 2015 or newer)

# Getting Started

**Step 1**. Clone the latest version of **dotnet-core-rest** on your local machine by running:

```shell
$ git clone -o dotnet-core-rest -b master --single-branch \
      https://github.com/hirohito-protagonist/dotnet-core-rest.git BookLibraryREST
$ cd BookLibraryREST
```

**Step 2**. Run the web server:

```shell
$ cd src/BooksLibrary
$ dotnet run
```

# License
 [UNLICENSE](/LICENSE)