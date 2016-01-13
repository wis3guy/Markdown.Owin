using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Markdown.Owin
{
	public class ServerContext
	{
		private ServerContext()
		{
			Routes = new Dictionary<string, Route>();
		}
		
		public IDictionary<string, Route> Routes { get; private set; }
		public Resource Custom404Page { get; private set; }
		public Resource Custom500Page { get; private set; }
		public Http404Behavior Http404Behavior { get; private set; }
		public string RootRequestPath { get; private set; }

		public static ServerContext Create(MarkdownOwinOptions options)
		{
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			var rootDirectoryPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, options.RootDirectory);

			if (!Directory.Exists(rootDirectoryPath))
				throw new ArgumentException("The specified root directory does not exist in the expected location: " + rootDirectoryPath);

			var rootRequestPath = NormalizeConfiguredRootRequestPath(options.RootRequestPath);
			var scanResult = ScanRootDirectoryForRoutes(rootDirectoryPath, rootRequestPath);
			var context = CreateContext(scanResult, options.Http404Behavior, options.RootRequestPath);

			return context;
		}

		private static string NormalizeConfiguredRootRequestPath(string org)
		{
			if (string.IsNullOrEmpty(org))
				return "/";

			var parts = org.Split('/');
			var first = parts.First();

			if ((first == "~") || (first == string.Empty))
				parts = parts.Skip(1).ToArray();

			return string.Concat("/", string.Join("/", parts));
		}

		private static ServerContext CreateContext(Dictionary<string, Route> scanResult, Http404Behavior http404Behavior, string rootRequestPath)
		{
			var context = new ServerContext
			{
				Http404Behavior = http404Behavior,
				RootRequestPath = rootRequestPath
			};

			var http404 = scanResult.Keys.FirstOrDefault(x => x.EndsWith("/_error/404"));

			if (http404 != null)
			{
				context.Custom404Page = scanResult[http404].Resource;
				scanResult.Remove(http404);
			}

			var http500 = scanResult.Keys.FirstOrDefault(x => x.EndsWith("/_error/500"));

			if (http500 != null)
			{
				context.Custom500Page = scanResult[http500].Resource;
				scanResult.Remove(http500);
			}

			context.Routes = scanResult;

			return context;
		}
		
		private static Dictionary<string, Route> ScanRootDirectoryForRoutes(string rootDirectoryPath, string rootRequestPath)
		{
			var routes = new Dictionary<string, Route>();
			var mdFiles = new List<string>();
			var cshtmlFiles = new List<string>();

			ScanFolderAndAppendFilesRecursive(rootDirectoryPath, mdFiles, "*.md");
			ScanFolderAndAppendFilesRecursive(rootDirectoryPath, cshtmlFiles, "*.cshtml");

			var mdByPath = mdFiles.ToDictionary(x => GetRelativeRequestPath(x, ".md", rootDirectoryPath, rootRequestPath));
			var cshtmlByPath = cshtmlFiles.ToDictionary(x => GetRelativeRequestPath(x, ".cshtml", rootDirectoryPath, rootRequestPath));

			foreach (var mdPath in mdByPath.Keys)
			{
				var cshtmlPath = cshtmlByPath.Where(x =>
					x.Key.Equals(mdPath, StringComparison.OrdinalIgnoreCase) ||
					mdPath.StartsWith(x.Key + "/", StringComparison.OrdinalIgnoreCase))
					.OrderBy(x => x.Key.Length)
					.Last()
					.Key;

				var node = new Route
				{
					RequestPath = mdPath,
					Resource = new Resource
					{
						Markdown = mdByPath[mdPath],
						Template = cshtmlByPath[cshtmlPath]
					}
				};

				routes.Add(node.RequestPath, node);
			}

			return routes;
		}

		private static void ScanFolderAndAppendFilesRecursive(string dir, List<string> files, string searchPattern)
		{
			files.AddRange(Directory.GetFiles(dir, searchPattern));

			foreach (var subdir in Directory.GetDirectories(dir))
				ScanFolderAndAppendFilesRecursive(subdir, files, searchPattern);
		}
		
		private static string GetRelativeRequestPath(string file, string extension, string rootDirectoryPath, string rootRequestPath)
		{
			const string defaultPage = "index";

			var length = rootDirectoryPath.Length;
			var path = file.Substring(length, file.Length - length - extension.Length).Replace('\\', '/');

			path = string.Concat(rootRequestPath, path);

			if (path.EndsWith(defaultPage))
				path = path.Substring(0, path.Length - defaultPage.Length);

			path = (path.Length > 1)
				? path.TrimEnd('/')
				: path;

			if (string.IsNullOrEmpty(path))
				path = "/";

			return path;
		}
	}
}