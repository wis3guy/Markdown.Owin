using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.Owin
{
	public class Route
	{
		public string Title { get; set; }
		public string RequestPath { get; set; }

		internal Resource Resource { get; set; }
	}
}
