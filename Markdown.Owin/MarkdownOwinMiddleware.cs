using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Markdown.Owin
{
	public class MarkdownOwinMiddleware
	{
		private readonly AppFunc _next;
		private readonly Server _server;

		public MarkdownOwinMiddleware(AppFunc next, MarkdownOwinOptions options)
		{
			if (next == null)
				throw new ArgumentNullException(nameof(next));

			if (options == null)
				throw new ArgumentNullException(nameof(options));

			_next = next;

			var sitemap = ServerContext.Create(options);
			var engine = new RenderingEngine();

			_server = new Server(sitemap, engine);
		}

		public async Task Invoke(IDictionary<string, object> environment)
		{
			if (!_server.Serve(environment))
				await _next.Invoke(environment);
		}
	}
}
