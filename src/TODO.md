TODO - Next
===========
[ ] How to redirect stdout from Edge.
[ ] We now need to consider the whole Node/Edge/NuGet design before we start redoing things. Thoughts:
    [ ] Move Node folder into Engine
    [ ] Pull handlebars in via the npm restore rather than explicitly.
    [ ] We need "our npm" and "user npm" to both work. So we will need to put the node folder
        into their website via NuGet?


[ ] Implement Edge.js.
    [ ] Consider replacing the EmbedIO web server too.
        https://github.com/indexzero/http-server
        https://github.com/cloudhead/node-static
        http://stackoverflow.com/questions/6084360/using-node-js-as-a-simple-web-server?lq=1
    [ ] And the live-reload server.
    [ ] And running bundler
    [ ] And the npm and node build steps...
[ ] Implement handlebars helpers. We now have a way forward. Probably want to port some existing helpers
    but do it from a .Net perspective, for example making a {{fmt}} helper to call String.Format(), in
    other words, we want some of the node functions to call back to .Net.
    Port the handlebars helpers from Assemble.IO.
[ ] Factor the ViewPipeline into an object and a Runner?
[ ] How to serve the LiveReload and static sites during development.
    https://github.com/livereload/livereload-js

Bugs
====
[ ] You cannot recompile a C# model when Lithogen is working in server mode. Shadow copy?
[ ] Printing GCS omits Misc 1 set.
[ ] Old versions of the GCS are cached by Chrome.
[ ] If there are errors processing in the view pipeline the website still gets served.
    We should report the error count.

TODO
====
[ ] The build command we used in the first invocation must be remembered and re-used in the server mode?
    How do you know if something requires an "npm run build" command?
    A "b" should reapply all your build commands.
    The default build command is "npm run build,csiv".
[ ] When Lithogen is used as a NuGet package, how can a website take a dependency on Lithogen.Core
    and/or Lithogen.Engine? Need to reconsider how we are deploying the NuGet package, it might
    be better to add everything in as a project in the assembly so that references can be taken
    like I am doing in the main Lithogen solution.
[ ] Rename Lithogen.Interfaces to Lithogen.Services?
[ ] Create black and white theme for GCS.
[ ] Create Powershell Cheatsheet.
[ ] YAML editor: https://visualstudiogallery.msdn.microsoft.com/34423c06-f756-4721-8394-bc3d23b91ca7
    Recommend in docs.
[ ] Remaining template work
    [ ] Support Jade. Jade can take a model directly.
	[ ] Handlebars: Allow any file to be passed through HandleBarsProcessor without
	    changing the extension.
	[ ] Markdown: No concept of a template or model. We need to provide both of these.
	    http://stackoverflow.com/questions/9931531/jade-template-with-variables-nodejs-server-side
        http://assemble.io/docs/Markdown.html : includes
		  - Write entire documents in markdown, and choose where and when to compile them to HTML
          - Write document fragments in markdown, so they can be "included" or used as partials within other larger documents
          - Write sections of markdown directly inside HTML documents (referred to as "inline markdown")
          - {{md  .. /path /to /content.md }}
[ ] Rebuild philipdaniels.com/configzilla.
[ ] Create nuget packages.
    [ ] Lithogen.Boilerplate - HTML5 boilerplate project
    [ ] Lithogen.Blog - blog example
    [ ] Lithogen.Samples - examples
[ ] Consider whether there is a use-case for a "*" extension mapping.
[ ] JavaScript linting.
[ ] Investigate HTML minifcation.
[ ] Highlight.js for syntax highlighting of code.
[ ] jQuery Colorbox for image popups, carousels and dialogs.
[ ] Friendly URL generation.
[ ] Add a one-off command to do image compression.
    This should be a one-off task. We can bundle some tools to help out.
      PNG quantization to 256 colors.
      MetaData stripping.
      JPEQ compression.    
[ ] Publication to web server: FTP, rsync?

Marketing
=========
[ ] Get Lithogen mentioned on https://www.npmjs.com/package/handlebars
[ ] Get Lithogen mentioned on https://github.com/tjanczuk/edge?
[ ] Get Lithogen mentioned on https://github.com/pinceladasdaweb/Static-Site-Generators and
    https://staticsitegenerators.net/ and http://www.staticgen.com/

[ ] Get GCS mentioned at 

Deferred, Possibly Forever
==========================
[ ] Allow pipeline processors to create multiple output files from one input
    file. This might be useful for code generators. IPipelineFile and its use
	if the ViewPipeline would have to change. It can be worked around by
    handling the PreBuild command to produce multiple files that will get fed
    into the pipeline.
[ ] Create a "buffering logger" for speed/parallelism?
[ ] Use Edge instead of node? Node has the advantage of allowing our users to
    create extendable pipelines.
[ ] Get rid of Newtonsoft - 500k to write some Json! But we might need it
    for loading Json models.
[ ] ConfigZilla integration
      A basic configuration that is used to create a class that is injected
	  into the ViewBag and can be used to control the views. @if
	  ViewBag.Config.EnableGoogleAnalytics
[ ] We are going to have several components that scan the Views dir.
    - The initial step in the ViewPipeline
	- The SideBySide processor
	- The ConfigurationResolver.
	It would be more efficient to load the ViewsDir into a virtual viewsdir
	object that contains a list of all the files in it. Construct this once
	during startup. It would then implement ISideBySide and IConfigurationResolver
	and IViewFileProvider. Would be a singleton.
	But in server mode this list will be expensive if building just an individual file.
	But we can monitor file deletes and creates and add them to the cache!
    - PerfTest: We can do 100,000 File.Exists() checks in 1.6 seconds. Given the length
	            of time it would take to compile 100,000 templates, this is an
				inconsequential overhead.


Interesting Links
=================
http://ajorkowski.github.io/NodeAssets/


Rebasing of Filenames (Relative filenames in views)
===================================================
Aim: setup 1 "website" in nginx which can be used to serve multiple independent sites.

	server {
		server_name philipdaniels.com www.philipdaniels.com;
		root /usr/share/nginx;

		location /site1 {
			try_files $uri $uri/ /index.html
		}
	}

- This will work with relative URLs such as "images/image1.png"
  since the browser requests: http://www.philipdaniels.com/site1/images/image1.png
- But for absolute URLs such as "/images/image1.png""
  the browser requests: http://www.philipdaniels.com/images/image1.png
  which will not work.

Basically the browser does not know that this is a separate "site", so "/images"
is always going to be relative to the real root of the website (http://www.philipdaniels.com/images/...).
This is not something that can be fixed by tweaking nginx config.

See also: http://weblog.west-wind.com/posts/2009/Dec/21/Making-Sense-of-ASPNET-Paths
http://www.w3schools.com/TAGS/tag_base.asp
http://stackoverflow.com/questions/893144/equivalent-in-javascript

    CONCLUSION
    ----------
    * Do not use absolute rooted "/" URLs.
    * Always use relative urls: "images" or "./images" or "../images"
    * If you must use absolute URLs, they must have the full domain
      request in them: http://www.philipdaniels.com/site1/images/image1.png
    * If you do this you can install the resultant website anywhere on disk
      just by copying it to a new location, without any fear that Urls will break.


Edge Hooking
============
// see https://gist.github.com/pguillory/729616
// and https://github.com/tjanczuk/edge#how-to-handle-nodejs-events-in-net

// Variant 1: collecting messages.

var func = Edge.Func(@"
return function(payload, callback) {
    var hooker = require('./../hooker');
    hooker.hookStreams(payload);

    // Do stuff...

    // Return to caller.
    var result = {};
    result['otherResults'] = { some: object };
    result['STANDARDSTREAMMESSAGES'] = hooker.getMessages();
    callback(null, result);
}");

// Variant 2: hooking the streams.

var func = Edge.Func(@"
return function(payload, callback) {
    var hooker = require('./../hooker');
    hooker.hookStreams(payload);

    // Do stuff

    // Return to caller.
    var result = {};
    result['otherResults'] = { some: object };
    callback(null, result);
}"


// Now wrapping other stuff.
var func = Edge.Func(@"
    return function(data, callback) {
        var hooker = require('./../hooker');
        hooker.hookStreams(data);

        // Do stuff
        var someTask = require('./../sometask');
        var result = someTask(data);

        callback(null, result);
    }");

and in sometask.js:

    "use strict";

    // See http://bites.goodeggs.com/posts/export-this/   (a very good explanation)
    // and https://quickleft.com/blog/creating-and-publishing-a-node-js-module/

    // You should name your functions so that stack traces are more understandable.
    module.exports = function mytask(data) {
        console.log("User task is running");
        console.log("User task is done");
    };
