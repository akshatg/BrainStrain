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
			return _language;
		}
		set
		{
			ChangeLanguage(value);
		}
	}
	
	private static SystemLanguage _language;
	private static Dictionary<string, string> _dictionary;
	
	private static readonly List<SystemLanguage> suportedLanguages = new List<SystemLanguage>(){SystemLanguage.Bulgarian, SystemLanguage.English};
	private static readonly SystemLanguage standardLanguage = SystemLanguage.English;
	
	static Localization()
	{
		Language = Application.systemLanguage;
		Debug.Log(Application.systemLanguage.ToString());
	}
	
	public static string Get(string key)
	{
		if(_dictionary != null)
			return _dictionary[key];
		throw new NullReferenceException("_dictionary was not initialized correctly(it is null)");
	}
	
	private static void ChangeLanguage(SystemLanguage language)
	{
		if(suportedLanguages.Contains(language))
		{
			_language = language;
		}
		else
		{
			Debug.LogWarning("Unsupported language: " + language.ToString() + "!");
			_language = standardLanguage;
		}
		
		var jsonString = (Resources.Load("Localization/" + _language.ToString()) as TextAsset).text;
		_dictionary = new Dictionary<string, string>();
		foreach(var item in Json.Deserialize(jsonString) as Dictionary<string,object>)
		{
			_dictionary.Add(item.Key, (string)item.Value);
		}
	}
}