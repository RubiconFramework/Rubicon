namespace Rubicon.Backend.Autoload;
public partial class LoadingHandler : Node
{
	public static string LastScenePath = "";
	public static string NewScenePath = "";
	public static bool IsLoading = false;
	public static Godot.Collections.Array LoadingProgress = new();
	private static LoadingHandler StaticInstance;
	private static bool transitioning = false;
	
	public LoadingHandler()
	{
		StaticInstance = this;
	}

	public static void ChangeScene(string NewScene){
		NewScenePath = NewScene;
		StaticInstance.LoadScene();
	}

	private void LoadScene()
	{
		var LastScene = GetTree().CurrentScene;
		LastScenePath = LastScene.SceneFilePath;

		GetTree().CallDeferred("change_scene_to_file","res://source/backend/LoadingScreen.tscn");

		if(ResourceLoader.Exists(NewScenePath)) {
			ResourceLoader.LoadThreadedRequest(NewScenePath, "", true, ResourceLoader.CacheMode.Ignore);

			IsLoading = true;
			LoadingProgress.Clear();

			GD.Print("Loading new scene...");
		}
		else GD.PushError("New scene file was not found.");
	}

	public override void _Process(double delta) {
		if(IsLoading) {
			ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(NewScenePath, LoadingProgress);
			GD.Print($"Loading Progress: {LoadingProgress[0]}");
			if(status == ResourceLoader.ThreadLoadStatus.Loaded && !transitioning) {
				GD.Print("New scene loaded.");
				transitioning = true;
				IsLoading = false;

				Resource LoadedScene = ResourceLoader.LoadThreadedGet(NewScenePath);
				SceneTreeTimer timer = GetTree().CreateTimer(1);
				timer.Connect("timeout",Callable.From(() => {
					transitioning = false;
					GetTree().ChangeSceneToPacked((PackedScene)LoadedScene);
				}));
			}
			else if(status == ResourceLoader.ThreadLoadStatus.InvalidResource)
				GD.Print("fuck");
				
		}
	}
}
