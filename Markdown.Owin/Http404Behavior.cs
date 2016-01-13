namespace Markdown.Owin
{
	public enum Http404Behavior
	{
		FallThrough,
		ServeIfBelowRootPath,
		Serve
	}
}