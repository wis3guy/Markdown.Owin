# Markdown.Owin
A simple piece of OWIN middleware to serve up markdown files.

## Disclaimer
Note that this is still very much a work in progress. It is a first attempt at writing a markdown server that supports Razor templates. 

If you are interested in the idea please chime in.

## Motivation
My intended use-case is to make it easy to incorporate documentation for CURIE links to point to inside a hypermedia driven API.

## Initial requirements

* Serve markdown files as html (use: https://github.com/Knagis/CommonMark.NET)
* Allow for templating based on Razor views (use: https://github.com/Antaris/RazorEngine)
* Have 404 handling logic, so requests can fall through
* Use conventions to determine:
   * Request path
   * Template to use
* Allow for a way to generate basic navigation inside the Razor template
	* siblings
	* home
	* 1st level 
* Work in both IIS and console app ... basically any OWIN host


## Intended usage
~~~c#
using Owin;

namespace Markdown.Owin.Demo
{
	public class Startup
	{
		public void Configuration(IAppBuilder builder)
		{
			builder.Use(typeof(MarkdownOwinMiddleware), new MarkdownOwinOptions
			{
				RootRequestPath = "/docs", // request path to serve within
				RootDirectory = "Markdown", // folder where the resources (md and cshtml files) reside
				Http404Behavior = Http404Behavior.FallThrough // must be play nice with WebApi or f.ex. Nancy
			});
		}
	}
}
~~~

