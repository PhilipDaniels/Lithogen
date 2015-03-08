TODO - Next
===========
[ ] Rebasing of CSS URLs now works, it is done relative to the output directory.
    But is this a real solution? In GCS I had to hack _Layout.hbs to contain
	absolute links: <link rel="stylesheet" href="./content/theme.solarized_dark.css" id="themeSheet">
	But that only works if the final generated page is in the root directory. If GCS had
	a "views\subfolder" directory then files in that directory would not load their stylesheets.

    What we could really do with is a "~" operator like in .Net.
	How do they solve this in the node world?

	http://weblog.west-wind.com/posts/2009/Dec/21/Making-Sense-of-ASPNET-Paths

	Since everything is a string that we are processing, we could look for a special marker
	such as ~APPROOT~ and replace that with appropriate ./ or ../ or ../../
	RESOLVE(~)

	Alternative: http://www.w3schools.com/TAGS/tag_base.asp
	http://stackoverflow.com/questions/893144/equivalent-in-javascript
	<head>
      <base href="http://<%= Request.Url.Authority + Request.ApplicationPath%>/" />
	</head>

	Defn: A relative Url is one that does not begin with a slash or http://
	      images/pic.jpg
		  ./blah/foo.html
		  ../../scripts/mine.js

     Affects: potentially all files being written, including those being processed
	 by bundler. We seem to have fixed bundler...

	 content: Affected - bunder
	 scripts: Affected - bundler
	 views  : Affected - ViewPipeline. Replace in the FileLoadBlock?
	    layouts: 

Bugs
====
[ ] Skip remin in bundler does not work?
[ ] You cannot recompile a C# the model when Lithogen is working in server mode. Shadow copy?
[ ] Printing GCS omits Misc 1 set.
[ ] Server mode does not redisplay the > prompt after building a file.


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
[ ] How to serve the LiveReload and static sites during development.
    https://github.com/livereload/livereload-js
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


Deferred, Possibly Forever
==========================
[ ] Allow pipeline processors to create multiple output files from one input
    file. This might be useful for code generators. IPipelineFile and its use
	if the ViewPipeline would have to change.
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
