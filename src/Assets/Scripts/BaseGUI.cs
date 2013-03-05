using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class GUITextures
{
	public Texture2D Restart;
	public Texture2D Menu;
	public Texture2D Undo;
	public Texture2D Block;
	public Texture2D Mark;
	public Texture2D Inspect;
	public Texture2D Back;
	public Texture2D Next;
	public Texture2D StarFull;
	public Texture2D StarEmpty;
	public Texture2D Settings;
	public Texture2D Play;
	public Texture2D Credits;
}

public class BaseGUI : MonoBehaviour
{
	public GUISkin Skin;
	public GUITextures Textures;
	
	protected float w;
	protected float w_2;
	protected float h;
	protected float h_2;
	protected Vector2 center;
	protected float offset;
	protected float big_offset;
	protected float button_w;
	protected float button_h;
	protected GUILayoutOption button_width;
	protected GUILayoutOption button_height;
	protected float button_size;
	protected GUILayoutOption button_size_w;
	protected GUILayoutOption button_size_h;
	protected Rect screen;
	
	// Use this for initialization
	protected virtual void Start()
	{
		InitGUI();
	}
	
	protected void InitGUI()
	{
		w = Screen.width;
		h = Screen.height;
		screen = new Rect(0, 0, w, h);
		w_2 = w / 2f;
		h_2 = h / 2f;
		center = new Vector2(w_2, h_2);
		button_w = w / 8f;
		button_h = h / 8f;
		button_width = GUILayout.Width(button_w);
		button_height = GUILayout.Height(button_h);
		button_size = (button_w + button_h) / 2f;
		button_size_w = GUILayout.Width(button_size);
		button_size_h = GUILayout.Height(button_size);
		offset = Mathf.Min(0.0078125f * w, 0.0078125f * h);
		big_offset = button_size / 3f;
	}
	
	// Update is called once per frame
	protected virtual void Update()
	{
		//should I remove this or should I!?
		InitGUI();
	}
	
	protected virtual void OnGUI()
	{
		GUI.skin = Skin;
	}
}
