The Assemblies
==============
Lithogen : the EXE.
Lithogen.Boilerplate : the boilerplate website available on NuGet.
Lithogen.Core : the guts of Lithogen.
Lithogen.ExamplePlugin : example of writing a very simple plugin
Lithogen.TaskShim : Integrated into MSBuild, calls the EXE.

The Lithogen Shim
=================

The Lithogen EXE
================

Dependency Injection
====================
Immutable, thread safe.

The Default Build Pipeline
==========================
- CSS and JS Compilation ServiceStack/Bundler
- Image copy
- "npm run build"
- View pipeline

Creating Plugins
================

Creating New View Processors
============================
One read, one write.

Commands and Handlers
=====================
The command handler can require any services it needs, but the command should
contain all the information necessary to execute it (in other words, it should
not be necessary to inject the Settings class).

Model Injection
===============
In Yaml:
    ---
    layout: _layout.hbs                  // built in property of IPipelineFile
    model: Lithogen.Models.SolarSystem   // built in property of IPipelineFile
	extout: html                         // built in property of IPipelineFile
	publish: false                       // built in property of IPipelineFile
    title: My title string
	planets: [Mercury, Venus, Mars, Earth]
    ---

In Razor:
    ---
	planets: [Mercury, Venus, Mars, Earth]
	---
    @model Lithogen.Models.SolarSystem
	@layout "_Layout.cshtml"

	Result: ViewBag has @ViewBag.planets set.
	        ViewBag has @ViewBag.FileInfo
			ViewBag has @ViewBag.UserData


Razor Engines Considered
========================
Numbers are NuGet downloads:
RazorEngine - 227,240 - lots of things are based on this, looks a bit simplistic. https://github.com/Antaris/RazorEngine USED THIS.
RazorMachine - 9615 - looks good, see the CodeProject article. LOOKS BEST ALTERNATIVE of the standalone ones. https://github.com/jlamfers/RazorMachine
RazorGenerator - seems to be a VS extension, avoid
RazorTempaltes - 3899 - small, maybe interesting. https://github.com/volkovku/RazorTemplates
WestWind.RazorHosting - 2116 - can inject a model! Does not support layouts or partials?
SharpRazor - 110 - looks interesting, can inject a model into its Parse() method

Web Servers Considered
======================
Chosen:	https://github.com/unosquare/embedio no Nuget. looks good - support for static files http://stackoverflow.com/questions/4268814/embedded-c-sharp-web-server?rq=1
  big list of mime types log4net NewtonSoft.

My own: Take code from the first article, plus code from EmbedIO's StaticFilesModule.
https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server.aspx
http://stackoverflow.com/questions/9034721/handling-multiple-requests-with-c-sharp-httplistener?rq=1
https://github.com/JamesDunne/Aardwolf

https://github.com/Bobris/Nowin 19000 no dependencies Based on OWin.  Does not support static file?
https://github.com/int6/uhttpsharp 1300 , log4net Newtonsoft promising Oct 2014
https://github.com/pvginkel/NHttp 1200 (old)
http://stackoverflow.com/questions/11746298/embedding-a-lightweight-web-server-into-a-net-application-node-js
http://www.asp.net/web-api/overview/hosting-aspnet-web-api/use-owin-to-self-host-web-api
