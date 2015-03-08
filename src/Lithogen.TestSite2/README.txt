TestSite2 is a maximal test site, created using the Visual Studio MVC4
Wizard and then left "as is."

The intention is that Lithogen should be able to compile such a "maximal"
project, as this may be how many people create their projects.

However, it *IS* necessary to remove artifacts such as "@Html" from
the views. This is because these are an aspect of MVC, which Razor
itself (as a parsing engine) knows nothing about.

From https://antaris.github.io/RazorEngine/AboutRazor.html, which is the engine
that Lithogen uses to parse Razor:

Razor vs. MVC vs. WebPages vs. RazorEngine
There is often a confusion about where Razor sits in this set of technoligies. Essentially
Razor is the parsing framework that does the work to take your text template and convert it
into a compileable class. In terms of MVC and WebPages, they both utilise this parsing
engine to convert text templates (view/page files) into executable classes (views/pages).
Often we are asked questions such as "Where is @Html, @Url", etc. These are not features
provided by Razor itself, but implementation details of the MVC and WebPages frameworks.

RazorEngine is another consumer framework of the Razor parser. We wrap up the
instantiation of the Razor parser and provide a common framework for using runtime
template processing.
