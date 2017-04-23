Tomorrow
========

A no-frills job scheduling library for .Net Core.

| Package               | Nuget Status                                                                                                                                 |
|-----------------------|----------------------------------------------------------------------------------------------------------------------------------------------|
| Tomorrow.Abstractions | [![NuGet](https://img.shields.io/nuget/v/Tomorrow.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/Tomorrow.Abstractions) |
| Tomorrow.InProcess    | [![NuGet](https://img.shields.io/nuget/v/Tomorrow.InProcess.svg?style=flat-square)](https://www.nuget.org/packages/Tomorrow.InProcess)       |

Goal
----

_Tomorrow_ is designed to be a simple interface to handle deferring jobs. This
repository also contains a simple implementation of this interface that runs
in-process.

*Please note that the in-process implementation **loses all remaining jobs** when
the process exits - this may not suit your purposes in production!*

Basic Usage
-----------

* Add a derived library (e.g. `Tomorrow.InProcess`)

        dotnet add package Tomorrow.InProcess

* Add the services to your dependency injection pipeline

        public void ConfigureServices(IServiceCollection services)
		{
			...
			services.AddTomorrowInProcess();
			...
		}

* Inject the `IJobScheduler` where required in code

        public class HomeController : Controller
		{
			private readonly IJobScheduler _jobScheduler;

			public HomeController(IJobScheduler jobScheduler)
			{
				_jobScheduler = jobScheduler;
			}

			public IActionResult Index()
			{
				_jobScheduler.Schedule(_ => Console.WriteLine("Hello World!"));
				return View();
			}
		}

Roadmap
-------

* [ ] Support multiple job scheduler backends in the same application (e.g.
    in-process to finish file uploads, persistent to perform encoding tasks)
* [ ] Write a persistent job scheduler backend
* [ ] Support more advanced tasks (than an expression)
