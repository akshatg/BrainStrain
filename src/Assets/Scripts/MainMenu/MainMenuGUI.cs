using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MainMenuGUI : BaseGUI
{
	private Global _global;
	private Vector2 scrollSettings;
	private Vector2 scrollWorlds;
	private Vector2 scrollLevels;
	private World currentWorld;
	
	private int dificulty;
	private string[] dificulties;

	// Use this for initialization
	protected override void Start()
	{
		base.Start();
		
		GetComponent<CameraControl>().State = CameraControl.CameraState.MainMenu;
		_global = GameObject.Find("Global").GetComponent<Global>();
		
		if(_global.MenuState == MenuState.Levels)
		{
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
			var scroll = new Vector2(-Input.GetTouch(0).deltaPosition.x, Input.GetTouch(0).deltaPosition.y);
			scrollSettings += scroll;
			scrollWorlds += scroll;
			scrollLevels += scroll;
		}
		#endif
	}
	
	protected override void OnGUI()
	{
		base.OnGUI();
		
		switch(_global.MenuState)
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
			case MenuState.Modes:
				ModesMenu();
				break;
			case MenuState.GeneratedLevel:
				GeneratedLevelMenu();
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
						_global.MenuState = MenuState.Settings;
					}
					GUILayout.FlexibleSpace();
					if(GUILayout.Button(Localization.Get("play"), GUILayout.MaxWidth(w_2 / 3f), button_size_h))
					{
						_global.MenuState = MenuState.Modes;
					}
					GUILayout.FlexibleSpace();
					if(GUILayout.Button(Textures.Credits, button_size_w, button_size_h))
					{
						_global.MenuState = MenuState.Credits;
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
			scrollLevels = GUILayout.BeginScrollView(scrollLevels);
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
			if(Musician.MusicOn)
				Musician.MusicOn = !GUILayout.Button(Textures.MusicOn, button_height);
			else
				Musician.MusicOn = GUILayout.Button(Textures.MusicOff, button_height);
			if(Musician.SoundsOn)
				Musician.SoundsOn = !GUILayout.Button(Textures.SoundsOn, button_height);
			else
				Musician.SoundsOn = GUILayout.Button(Textures.SoundsOff, button_height);
			GUILayout.EndHorizontal();
			Musician.MusicVolume = VolumeSlider(Musician.MusicVolume, Localization.Get("music"));
			Musician.SoundsVolume = VolumeSlider(Musician.SoundsVolume, Localization.Get("sounds"));
			Musician.MasterVolume = VolumeSlider(Musician.MasterVolume, Localization.Get("master"));
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
			GUILayout.EndScrollView();
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
	
	private void ModesMenu()
	{
		BackButton();
		GUILayout.BeginArea(screen);
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
					GUILayout.FlexibleSpace();
					if(GUILayout.Button(Localization.Get("worlds"), GUILayout.MaxWidth(w_2 / 3f), button_height))
					{
						_global.MenuState = MenuState.Worlds;
					}
					if(GUILayout.Button(Localization.Get("generatedlevel"), GUILayout.MaxWidth(w_2 / 3f), button_height))
					{
						_global.MenuState = MenuState.GeneratedLevel;
					}
					GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void GeneratedLevelMenu()
	{
		BackButton();
		GUILayout.BeginArea(screen);
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
					GUILayout.FlexibleSpace();
					dificulties = new[]{
											Localization.Get("easy"),
											Localization.Get("medium"),
											Localization.Get("hard"),
											Localization.Get("insane")
									   };
					dificulty = GUILayout.SelectionGrid(dificulty, dificulties, 2, button_size_h);
					if(GUILayout.Button(Textures.Play, button_size_h))
					{
						var chance = LevelGenerator.Chanses.Easy;
						switch(dificulty)
						{
							case 0:
								chance = LevelGenerator.Chanses.Easy;
								break;
							case 1:
								chance = LevelGenerator.Chanses.Medium;
								break;
							case 2:
								chance = LevelGenerator.Chanses.Hard;
								break;
							case 3:
								chance = LevelGenerator.Chanses.Insane;
								break;
						}
						
						LevelGenerator.InitFromCube(5); //change to be ajustable
						LevelGenerator.Chance = chance;
				
						_global.Data = LevelGenerator.Generate();
						Application.LoadLevel("Game");
					}
					GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void WorldMenu()
	{
		BackButton();
		GUILayout.BeginArea(screen);
			GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
					GUILayout.Space(big_offset);
					scrollWorlds = GUILayout.BeginScrollView(scrollWorlds, GUILayout.ExpandHeight(true));
						GUILayout.BeginHorizontal();
							foreach(World world in _global.Worlds)
							{
								if(WorldButton(world))
								{
									currentWorld = world;
									_global.MenuState = MenuState.Levels;
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
		BackButton();
		GUILayout.BeginVertical();
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
			_global.MenuBack();
	}
	
	private float VolumeSlider(float volume, string name)
	{
		GUILayout.BeginHorizontal();
			GUILayout.Box(name, button_width);
			volume = GUILayout.HorizontalSlider(volume, 0f, 1f);
		GUILayout.EndHorizontal();
		return volume;
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