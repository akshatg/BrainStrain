using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public static class Localization
{
	public static SystemLanguage Language
	{
		get
		{
			return language;
		}
		set
		{
			ChangeLanguage(value);
		}
	}
	
	private static SystemLanguage language;
	private static Dictionary<string, string> dictionary;
	
	private static readonly List<SystemLanguage> suportedLanguages = new List<SystemLanguage>{SystemLanguage.Bulgarian, SystemLanguage.English};
    private const SystemLanguage standardLanguage = SystemLanguage.English;

    static Localization()
	{
		Language = Application.systemLanguage;
	}
	
	public static string Get(string key)
	{
		if(dictionary != null)
			return dictionary[key];
		throw new NullReferenceException("_dictionary was not initialized correctly(it is null)");
	}
	
	private static void ChangeLanguage(SystemLanguage language)
	{
		if(suportedLanguages.Contains(language))
		{
			Localization.language = language;
		}
		else
		{
			Debug.LogWarning("Unsupported language: " + language.ToString() + "!");
			Localization.language = standardLanguage;
		}
		
		var jsonString = ((TextAsset)Resources.Load("Localization/" + Localization.language)).text;
		dictionary = new Dictionary<string, string>();
		foreach(var item in (Dictionary<string, object>)Json.Deserialize(jsonString))
		{
			dictionary.Add(item.Key, (string)item.Value);
		}
	}
}