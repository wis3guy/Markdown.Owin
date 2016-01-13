# Markdown.Owin
A simple piece of OWIN middleware to serve up markdown files.

## Work in progress
Note that this is still very much a work in progress. It is a first attempt at writing a markdown server that supports Razor templates. My intended use-case is to make it easy to incorporate documentation for CURIE links to point to inside a hypermedia driven API.

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

## Open for ideas
If you are interested in the idea please chime in.
