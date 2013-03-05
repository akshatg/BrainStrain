using UnityEngine;
using System.Collections;

public class LevelGUI : BaseGUI {
	
	public bool Blur
	{
		get
		{
			return GetComponent<BlurEffect>().enabled;
		}
		set
		{
			GetComponent<BlurEffect>().enabled = value;
		}
	}
	
	private Global _global;
	private Spawner _spawner;
	private CameraControl _cameraControl;
	
	private int tool;
	private Texture2D[] tools;
	private Rect windowRect;
	
	// Use this for initialization
	protected override void Start()
	{
		base.Start();
		
		_global = GameObject.Find("Global").GetComponent<Global>();
		_spawner = GetComponent<Spawner>();
		_cameraControl = GetComponent<CameraControl>();
		
		tool = 0;
		tools = new[]{Textures.Block, Textures.Mark, Textures.Inspect};
		windowRect = new Rect(w_2 - (w_2 / 2f), h_2 - (h_2 / 2f), w_2, h_2);
	}
	
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
	}
	
	protected override void OnGUI()
	{
		base.OnGUI();
		
		if(_spawner.LevelSolved)
		{
			#if !(UNITY_ANDROID || UNITY_IOS)
			Blur = true;
			#endif
			GetComponent<CameraControl>().State = CameraControl.CameraState.LevelComplete;
			windowRect = GUILayout.Window(0, windowRect, LevelCompleteGUI, "");
		}
		else
		{
			Blur = false;
			GetComponent<CameraControl>().State = CameraControl.CameraState.Game;
			
			GUILayout.BeginArea(screen);
				GUILayout.BeginVertical();
					if(GUILayout.Button(Textures.Menu, button_size_w, button_size_h))
					{
						Application.LoadLevel("MainMenu");
					}
					if(GUILayout.Button(Textures.Restart, button_size_w, button_size_h))
					{
						_spawner.RestartLevel();
						_cameraControl.RestartView();
					}
					GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.BeginHorizontal();
					if(GUILayout.Button(Textures.Undo, button_size_w, button_size_h))
					{
						_spawner.UndoBlock();
					}
					GUILayout.Box(new GUIContent(NumberOfStars().ToString(), Textures.StarFull), button_size_w, button_size_h);
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
			
			GUILayout.BeginArea(screen);
				GUILayout.BeginVertical();
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						tool = GUILayout.Toolbar(tool, tools, GUILayout.Width(button_size * tools.Length), button_size_h);
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			GUILayout.EndArea();
			
			if(GUI.changed)
			{
				_spawner.CurrentTool = (Tool)tool;
			}
		}
	}
	
	private int NumberOfStars()
	{
		int stars;
		if(_spawner.UndosLeft >= 0)
			stars = Mathf.CeilToInt(Global.Utills.ScaleValue(_spawner.UndosLeft, 0, _spawner.StartingUndos, 1, 3));
		else
			stars = 0;
		return stars;
	}
	
	private void LevelCompleteGUI(int windowId)
	{
		//changes the level stars and completed
		_global.CurrentLevel.Completed = true;
		var stars = NumberOfStars();
		if(stars > _global.CurrentLevel.Stars)
			_global.	CurrentLevel.Stars = stars;
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(Localization.Get("congratulations"));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				for(int i = 1; i <= 3; i++)
				{
					if(stars >= i)
						GUILayout.Label(Textures.StarFull, button_size_w, button_size_h);
					else
						GUILayout.Label(Textures.StarEmpty, button_size_w, button_size_h);
				}
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if(GUILayout.Button(Textures.Menu, button_size_w, button_size_h))
				{
					Application.LoadLevel("MainMenu");
				}
				GUILayout.FlexibleSpace();
				if(GUILayout.Button(Textures.Restart, button_size_w, button_size_h))
				{
					_spawner.RestartLevel();
					_cameraControl.RestartView();
				}
				GUILayout.FlexibleSpace();
				if(GUILayout.Button(Textures.Next, button_size_w, button_size_h))
				{
					_spawner.LoadLevel(_global.NextLevel().GetData());
				}
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
}
