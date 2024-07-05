#region Includes
	using System.Collections.Generic;
#endregion

namespace FNFGodot.Backend.Autoload;
public partial class Preferences : Node
{
    public static Dictionary<string, dynamic> placeholderSettings = new Dictionary<string, dynamic>();

	public override void _Ready() 
	{
		placeholderSettings.Add("downscroll",true);
	}
}
