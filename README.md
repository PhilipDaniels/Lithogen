Lithogen
========
Lithogen is a static site generator for .Net. A "static site" is a website that
contains only HTML, CSS, images and JavaScript. There is no back-end processor such
as ASP or CGI, and as a consequence the resulting website can be served by
any webserver - including Linux servers - and is typically small and fast compared
to a website that needs a backend since there is no runtime overhead.

Lithogen is a program that can be run from the command line or it can be hooked
into the standard Visual Studio build process.

Lithogen uses Razor as its view engine, but also ships with a copy of node.js
which is used to compile assets (CSS, JavaScript) and npm which is used to run
several aspects of the build.

Lithogen is usable out of the box, but is also very extensible: the build
pipeline can be extended using either C# or node.

Try "Lithogen.exe --fullhelp" to see the full help, or read it
here https://github.com/PhilipDaniels/Lithogen/blob/master/src/Lithogen.Core/CommandLine/Lithogen.usage.txt

Features
========
* Websites are based on the ASP.Net MVC folder structure, but using the HTML5
  boilerplate project, which gives you a great starting point for building modern,
  mobile-friendly websites. HTML5 boilerplate also gives you jQuery, Modernizr and
  Normalize.css and Google Analytics.
  
* Uses ServiceStack/Bundler https://github.com/ServiceStack/Bundler to
  compile, minify and combine your less, sass, stylus, css, coffeescript,
  livescript and js files.

* Ships with a working copy of node.exe and npm, making it easy to extend
  the build process with node modules.

