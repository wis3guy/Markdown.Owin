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