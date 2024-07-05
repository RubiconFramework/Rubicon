using System.Collections.Generic;

namespace Rubicon.Backend.Autoload;
public partial class Preferences : Node
{
    public static Dictionary<string, dynamic> placeholderSettings = new Dictionary<string, dynamic>();

	public override void _Ready() 
	{
		placeholderSettings.Add("downscroll",true);
	}
}
