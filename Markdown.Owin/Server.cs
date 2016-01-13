using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Markdown.Owin
{
	internal class Server
	{
		private readonly ServerContext _context;
		private readonly RenderingEngine _renderingEngine;

		public Server(ServerContext context, RenderingEngine renderingEngine)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (renderingEngine == null) throw new ArgumentNullException(nameof(renderingEngine));

			_context = context;
			_renderingEngine = renderingEngine;
		}

		private void Serve(HttpStatusCode statusCode, IDictionary<string, object> environment, Route node)
		{
			var content = _renderingEngine.Render(node.Resource, _context);

			Serve(statusCode, "text/html", content, environment);
		}

		private void Serve(HttpStatusCode statusCode, Resource context, IDictionary<string, object> environment)
		{
			string content;
			string contentType;

			if (context == null)
			{
				content = statusCode.ToString();
				contentType = "text";
			}
			else
			{
				content = _renderingEngine.Render(context, _context);
				contentType = "text/html";
			}

			Serve(statusCode, contentType, content, environment);
		}

		private static void Serve(HttpStatusCode statusCode, string contentType, string content, IDictionary<string, object> environment)
		{
			environment["owin.ResponseStatusCode"] = (int) statusCode;
			environment["owin.ResponseHeaders"] = new Dictionary<string, string[]> { { "Content-Type", new[] { contentType } } };

			using (var sw = new StreamWriter((Stream) environment["owin.ResponseBody"]))
			{
				sw.Write(content);
			}
		}

		public bool Serve(IDictionary<string, object> environment)
		{
			Route node;

			var path = (string) environment["owin.RequestPath"];

			path = (path.Length > 1)
				? path.TrimEnd('/')
				: path;

			if (_context.Routes.TryGetValue(path, out node))
			{
				Serve(HttpStatusCode.OK, environment, node);
			}
			else
			{
				if (_context.Http404Behavior == Http404Behavior.FallThrough)
					return false;

				if ((_context.Http404Behavior == Http404Behavior.ServeIfBelowRootPath) &&
					!path.Equals(_context.RootRequestPath, StringComparison.OrdinalIgnoreCase) &&
					!path.StartsWith(_context.RootRequestPath + '/', StringComparison.OrdinalIgnoreCase))
					return false;

				Serve(HttpStatusCode.NotFound, _context.Custom404Page, environment);
			}

			return true;
		}
	}
}