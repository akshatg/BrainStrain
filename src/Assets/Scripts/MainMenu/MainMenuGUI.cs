using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

enum MenuState
{
	Main,
	Settings,
	Credits,
	Worlds,
	Levels,
}

public class MainMenuGUI : BaseGUI
{
	private Global _global;
	private Vector2 scrollWorlds;
	private Vector2 scrollLevels;
	private World currentWorld;
	private Stack<MenuState> states;
	private MenuState State
	{ 
		get
		{
			return states.Peek();
		}
	}

	// Use this for initialization
	protected override void Start()
	{
		base.Start();
		
		GetComponent<CameraControl>().State = CameraControl.CameraState.MainMenu;
		_global = GameObject.Find("Global").GetComponent<Global>();
		
		states = new Stack<MenuState>();
		
		if(_global.CurrentLevel.Number == 1 && _global.CurrentWorld.Number == 1)
		{
			states.Push(MenuState.Main);
		}
		else
		{
			states.Push(MenuState.Main);
			states.Push(MenuState.Worlds);
			states.Push(MenuState.Levels);
			currentWorld = _global.CurrentWorld;
		}
	}
	
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
		#if UNITY_ANDROID || UNITY_IOS
		//this is for the scrollers
		if(Input.touchCount > 0)
		{	
			scrollWorlds.x -= Input.GetTouch(0).deltaPosition.x;
			scrollWorlds.y += Input.GetTouch(0).deltaPosition.y;
			
			scrollLevels.x -= Input.GetTouch(0).deltaPosition.x;
			scrollLevels.y += Input.GetTouch(0).deltaPosition.y;
		}
		#endif
	}
	
	protected override void OnGUI()
	{
		base.OnGUI();
		
		switch (State)
		{
			case MenuState.Main:
				if(Input.GetKeyDown(KeyCode.Escape))
				{
					Application.Quit();
				}
				MainMenu();
				break;
			case MenuState.Settings:
				SettingsMenu();
				break;
			case MenuState.Credits:
				CreditsMenu();
				break;
			case MenuState.Worlds:
				WorldMenu();
				break;
			case MenuState.Levels:
				LevelMenu();
				break;
		}
	}
	
	private void MainMenu()
	{
		GUILayout.BeginArea(screen);
			GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
					if(GUILayout.Button(Textures.Settings, button_size_w, button_size_h))
					{
						states.Push(MenuState.Settings);
					}
					GUILayout.FlexibleSpace();
					if(GUILayout.Button(Localization.Get("play"), GUILayout.MaxWidth(w_2 / 3f), button_size_h))
					{
						states.Push(MenuState.Worlds);
					}
					GUILayout.FlexibleSpace();
					if(GUILayout.Button(Textures.Credits, button_size_w, button_size_h))
					{
						states.Push(MenuState.Credits);
					}
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	private void SettingsMenu()
	{
		var bg = Resources.Load("Localization/BG", typeof(Texture)) as Texture;
		var en = Resources.Load("Localization/EN", typeof(Texture)) as Texture;
		
		BackButton();
		GUILayout.BeginArea(screen);
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.Box(Localization.Get("lang"));
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(en, button_height))
			{
				Localization.Language = SystemLanguage.English;
			}
			if(GUILayout.Button(bg, button_height))
			{
				Localization.Language = SystemLanguage.Bulgarian;
			}
			GUILayout.EndHorizontal();
			GUILayout.Box(Localization.Get("snd"));
			GUILayout.BeginHorizontal();
			if(_global.MusicOn)
				_global.MusicOn = !GUILayout.Button(Textures.MusicOn, button_height);
			else
				_global.MusicOn = GUILayout.Button(Textures.MusicOff, button_height);
			if(_global.SoundsOn)
				_global.SoundsOn = !GUILayout.Button(Textures.SoundsOn, button_height);
			else
				_global.SoundsOn = GUILayout.Button(Textures.SoundsOff, button_height);
			GUILayout.EndHorizontal();
			if(GUILayout.Button(Localization.Get("cls"), button_height))
			{
				foreach(Level level in _global.Worlds.SelectMany(world => world.Levels))
				{
					level.Stars = 0;
					level.Completed = false;
				}
				if(File.Exists(Global.LevelsFile))
				{
					File.Delete(Global.LevelsFile);
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void CreditsMenu()
	{
		BackButton();
		GUILayout.BeginArea(screen);
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
					GUILayout.FlexibleSpace();
					GUILayout.Box(Localization.Get("credits"));
					GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void WorldMenu()
	{
		GUILayout.BeginArea(screen);
			GUILayout.BeginVertical();
				BackButton();
				GUILayout.BeginHorizontal();
					GUILayout.Space(big_offset);
					scrollWorlds = GUILayout.BeginScrollView(scrollWorlds, GUILayout.ExpandHeight(true));
						GUILayout.BeginHorizontal();
							foreach(World world in _global.Worlds)
							{
								if(WorldButton(world))
								{
									currentWorld = world;
									states.Push(MenuState.Levels);
								}
							}
						GUILayout.EndHorizontal();
					GUILayout.EndScrollView();
					GUILayout.Space(big_offset);
				GUILayout.EndHorizontal();
				GUILayout.Space(big_offset);
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	private void LevelMenu()
	{
		GUILayout.BeginVertical();
			BackButton();
			scrollLevels = GUILayout.BeginScrollView(scrollLevels, GUILayout.Width(w));
				foreach(Level level in currentWorld.Levels)
				{
					if(LevelButton(level))
					{
						_global.CurrentLevel = level;
						Application.LoadLevel("Game");
					}
				}
			GUILayout.EndScrollView();
			GUILayout.Space(big_offset);
		GUILayout.EndVertical();
	}
	
	private void BackButton()
	{
		if(GUILayout.Button(Textures.Back, button_width, button_height))
		{
			if(states.Count > 0)
			{
				states.Pop();
			}
		}
	}
	
	private bool LevelButton(Level level)
	{
		bool pressed;
		var w = GUILayout.Width(h_2 / 3f);
		var h = GUILayout.Height(h_2 / 3f);
		
		GUILayout.BeginHorizontal();
		var col = GUI.color;
		if(level.Completed)
			GUI.color = Color.green;
		else
			GUI.color = Color.gray;
		GUILayout.Box(level.Number.ToString(), w, h);
		GUI.color = col;
		
		GUILayout.Box(Localization.Get(level.Name), GUILayout.ExpandWidth(true), h);
		
		for(int i = 1; i <= 3; i++)
		{
			if(level.Stars >= i)
				GUILayout.Box(Textures.StarFull, w, h);
			else
				GUILayout.Box(Textures.StarEmpty, w, h);
		}
		pressed = GUILayout.Button(Textures.Play, w, h);
		GUILayout.EndHorizontal();
		return pressed;
	}
	
	private bool WorldButton(World world)
	{
		bool pressed;
		var w = GUILayout.Width(base.w / 3);
		var h = GUILayout.Height(button_h);
		var dificultyImage = Resources.Load("Dificulties/Difficulty" + world.Number, typeof(Texture)) as Texture;
		
		GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.Box(dificultyImage, w, GUILayout.Height(button_h * 3.5f));
			GUILayout.Box(new GUIContent(world.Levels.Sum(level => level.Stars) + "/" + world.Levels.Count * 3, Textures.StarFull), w, h);
			pressed = GUILayout.Button(Localization.Get("world" + world.Number), w, h);
		GUILayout.EndVertical();
		return pressed;
	}
}