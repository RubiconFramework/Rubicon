#region Imports
    global using Godot;
    global using System;
    global using Godot.Sharp.Extras;
    using System.Collections.Generic;
#endregion

namespace Rubicon;
public partial class Main : Node
{
	/*   The average global class that has utilities and sometimes holds useful variables.	*/

	public static Vector2 WindowSize {get; set;} = new Vector2((float)ProjectSettings.GetSetting("display/window/size/viewport_width"),
		(float)ProjectSettings.GetSetting("display/window/size/viewport_height"));
	public static string[] AudioFileTypes = {".ogg",".mp3",".wav",".flac"};

    public override void _Ready()
    {
		//godot should have an editor-only background override or something this shits annoying
        RenderingServer.SetDefaultClearColor(new Color(0,0,0,1));
    }

	// i know this is in FlashImporter too
	// its done this way so the FlashImporter plugin can be used in other projects
	// (as an actual plugin and stuff)
    public static List<string> FilesInDirectory(string path)
    {
	    List<string> files = new();
		DirAccess directory = DirAccess.Open(path);
		if (directory != null)
		{
			try
			{
				directory.ListDirBegin();
				while (true)
				{
					string file = directory.GetNext();
					if (file == "") break;
					if (!file.StartsWith(".")) files.Add(file);
				}
			}
			finally
			{
				directory.ListDirEnd();
			}
		}
		return files;
	}

	public static AudioStream SearchAllAudioFormats(string filePath, bool throwError = true)
	{
		string finalPath = "";

		foreach(string type in AudioFileTypes)
		{
			if(ResourceLoader.Exists(filePath+type)){
				if(finalPath == "") {
					finalPath = filePath+type;
				} else GD.Print("Another audio file was found, but will not be used");
			}
		}
		if(ResourceLoader.Exists(finalPath)) return GD.Load<AudioStream>(finalPath);
		else {
			if(throwError) GD.PrintErr($"No audio file was found with the provided path: {filePath}");
			return null;
		}
	}
}
