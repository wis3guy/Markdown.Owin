using System;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;

namespace Markdown.Owin
{
	internal class RenderingEngine
	{
		public string Render(Resource resource, ServerContext context)
		{
			if (resource == null) throw new ArgumentNullException(nameof(resource));
			if (context == null) throw new ArgumentNullException(nameof(context));

			var mdBody = resource.Markdown; // todo: read and convert the md file to html

			string templateKey; // todo: actually deal with caching the templates and using the cached versions
			string templateBody;

			if (string.IsNullOrEmpty(resource.Template))
			{
				templateKey = "default";
				templateBody = @"<!DOCTYPE html>
<html>
<head>
	<title>@Model.Title</title>
</head>
<body>
	<h1>@Model.Title</h1>
	@Model.Body
</body>
</html>";
			}
			else
			{
				templateKey = resource.Template;
				templateBody = File.ReadAllText(resource.Template);
			}

			var model = new RenderingContext
			{
				Body = resource.Markdown,
				Title = resource.Markdown
			};

			return Engine.Razor.RunCompile(templateBody, templateKey, typeof(RenderingContext), model);
		}
	}
}