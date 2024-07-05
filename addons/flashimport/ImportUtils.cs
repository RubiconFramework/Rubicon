using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlashImport;
public partial class ImportUtils
{
    public static List<string> ListDirectory(string path)
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

	public static float FramesToTime(int frames, int fps)
	{
		float frameTime = fps/1000;
		return frameTime*frames;
	}
}
