namespace Markdown.Owin
{
	public class MarkdownOwinOptions
	{
		public string RootRequestPath { get; set; }
		public string RootDirectory { get; set; }
		public Http404Behavior Http404Behavior { get; set; }
	}
}