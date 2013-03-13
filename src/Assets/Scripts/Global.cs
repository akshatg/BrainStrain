using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class World
{
	public int Number;
	public List<Level> Levels;
}

public class Level
{
	public int World;
	public int Number;
	public string Name;
	
	public bool Completed;
	public int Stars;
	
	public string FileName
	{
		get
		{
			return World + "_" + Number + "_" + Name;
		}
	}
	
	public char[,,] GetData()
	{
		var textAsset = Resources.Load("Levels/World " + World + "/" + FileName, typeof(TextAsset)) as TextAsset;
		return Parser.ParseFromString(textAsset.text);
	}
	
	public static Level FromFileName(string name)
	{
		var data = name.Split('_');
	    Level level = new Level
	                      {
	                          World = int.Parse(data[0]), 
                              Number = int.Parse(data[1]), 
                              Name = data[2]
	                      };
	    return level;
	}
}

/// <summary>
/// Holds worlds and levels information and manages saving/loading game data
/// </summary>
public class Global : MonoBehaviour
{
	public static string LevelsFile;
	public static string SettingsFile;
	
	public World CurrentWorld
	{
		get	
		{
			return Worlds[currentWorld];
		}
	}
	public Level CurrentLevel
	{
		get
		{
			return CurrentWorld.Levels[currentLevel];
		}
		set
		{
			currentLevel = value.Number - 1;
			currentWorld = value.World - 1;
		}
	}
	
	public List<World> Worlds{ get; private set; }
	
	private int currentLevel;
	private int currentWorld;
	
	void Awake()
	{
        DontDestroyOnLoad(transform.gameObject);
    }

	// Use this for initialization
	void Start()
	{
		LevelsFile = Application.persistentDataPath + "/levels_data.txt";
		SettingsFile = Application.persistentDataPath + "/settings_data.txt";
		
		try
		{
			if(File.Exists(SettingsFile))
			{
				LoadSettingsData(SettingsFile);
			}
			
			CreateWorlds();
			if(File.Exists(LevelsFile))
			{
				LoadLevelData(LevelsFile);
			}
		}
		catch(IOException ex)
		{
			throw new Exception("Problem with save files!", ex);
		}
		Application.LoadLevel("MainMenu");
	}
	
	public Level NextLevel()
	{
		if(currentLevel >= CurrentWorld.Levels.Count - 1)
		{
			if(currentWorld < Worlds.Count - 1)
			{
				currentWorld++;
				currentLevel = 0;
			}
		}
		else
		{
			currentLevel++;
		}
		return Worlds[currentWorld].Levels[currentLevel];
	}
	
	private void LoadSettingsData(string filename)
	{
		using(StreamReader sr = new StreamReader(filename))
		{
            var dictionary = Json.Deserialize(sr.ReadToEnd()) as Dictionary<string, object>;
			
			Localization.Language = (SystemLanguage)(int)(long)dictionary["Language"];
			Musician.MusicOn = (bool)dictionary["Music"];
			Musician.SoundsOn = (bool)dictionary["Sounds"];
			Musician.MusicVolume = (float)(double)dictionary["MusicVolume"];
			Musician.SoundsVolume = (float)(double)dictionary["SoundsVolume"];
			Musician.MasterVolume = (float)(double)dictionary["MasterVolume"];
		}
	}
	
	private void LoadLevelData(string filename)
	{
		using(StreamReader sr = new StreamReader(filename))
		{			
			var list = (List<object>)Json.Deserialize(sr.ReadToEnd());
			
			foreach(var obj in list)
			{
				var dictionary = obj as Dictionary<string, object>;
				
				var world = (int)(long)dictionary["World"];
				var number = (int)(long)dictionary["Number"];
				var name = (string)dictionary["Name"];
				var completed = (bool)dictionary["Completed"];
				var stars = (int)(long)dictionary["Stars"];
				
				var level = Worlds[world - 1].Levels[number - 1];
				level.World = world;
				level.Number = number;
				level.Name = name;
				level.Completed = completed;
				level.Stars = stars;
			}
		}
	}
	
	private void SaveSettingsData(string filename)
	{
		using(StreamWriter sw = new StreamWriter(filename))
		{
			var dictionary = new Dictionary<string, object>();
			
			dictionary["Language"] = (int)Localization.Language;
			dictionary["Music"] = Musician.MusicOn;
			dictionary["Sounds"] = Musician.SoundsOn;
			dictionary["MusicVolume"] = Musician.MusicVolume;
			dictionary["SoundsVolume"] = Musician.SoundsVolume;
			dictionary["MasterVolume"] = Musician.MasterVolume;
			
			sw.WriteLine(Json.Serialize(dictionary));
		}
	}
	
	private void SaveLevelData(string filename)
	{
		using(StreamWriter sw = new StreamWriter(filename))
		{			
			var result = Worlds.SelectMany(world => world.Levels)
							   .Select(level => new Dictionary<string,object>{
																				{"World",level.World},
																				{"Number",level.Number},
																				{"Name",level.Name},
																				{"Completed",level.Completed},
																				{"Stars",level.Stars}
																				})
							   .ToList();
			
			sw.WriteLine(Json.Serialize(result));
		}
	}
	
	public void CreateWorlds()
	{
		Worlds = new List<World>();
		
		var files = Resources.LoadAll("Levels", typeof(TextAsset)).Select(e => e.name);

        foreach (var item in files.Select(fileName => Level.FromFileName(fileName))
								 .GroupBy(level => level.World)
		        				 .OrderBy(item => item.Key)
		       )
		{
		    var world = new World
		                    {
		                        Number = item.Key, 
                                Levels = new List<Level>()
		                    };

		    foreach (var level in item.OrderBy(i => i.Number))
			{
				world.Levels.Add(level);
			}
			Worlds.Add(world);
		}
	}
	
	void OnApplicationQuit()
	{
		SaveData();
	}
	
	void OnApplicationPause()
	{
		SaveData();	
	}
	
	private void SaveData()
	{
		#if !UNITY_EDITOR
		SaveSettingsData(SettingsFile);
		SaveLevelData(LevelsFile);
		#endif
	}
	
	public static class Utills
	{
		public static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360)
				angle += 360;
			if (angle > 360)
				angle -= 360;
			return Mathf.Clamp(angle, min, max);
		}
		
		public static float ScaleValue(float value, float oldMin, float oldMax, float newMin, float newMax)
		{
		    return newMin + (value - oldMin) / (oldMax - oldMin) * (newMax - newMin);
		}
	}
}
