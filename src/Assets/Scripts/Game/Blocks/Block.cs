using UnityEngine;
using System;
using System.Collections;

public enum State
{
	Unsolved,
	Solved,
	Wrong
}

public class Block : MonoBehaviour
{	
	public Texture2D[] Textures;
	public Color[] Palete;
	
	public int Id{ get; set; }
	public State State
	{ 
		get
		{
			return state;
		}
		set
		{
			if(value != state)
			{
				state = value;
				StateChanged(value);
			}
		}
	}
	
	public bool Marked
	{
		get
		{
			return marked;
		}
		set
		{
			if(value != marked)
			{
				marked = value;
				MarkedChanged(value);
			}
		}
	}
	
	public bool Inspected
	{
		get
		{
			return inspected;
		}
		set
		{
			if(value != inspected)
			{
				inspected = value;
				InspectedChanged(value);
			}
		}
	}
	
	protected Material material;
	protected Collider colider;
	
	private State state;
	private bool marked;
	private bool inspected;
	
	protected virtual void Awake()
	{
		material = GetComponent<Renderer>().material;
		colider = GetComponent<Collider>();
	}
	
	protected virtual void Start()
	{
		State = State.Unsolved;
		Marked = false;
		Inspected = false;
	}
	
	void OnMouseUpAsButton()
	{
		OnInteract();
	}
	
	protected virtual void OnInteract()
	{
		var obj = GameObject.Find("Main Camera").GetComponent<Spawner>();
		if(obj != null)
		{
			obj.SendMessage("BlockPressed", Id);
		}
	}
	
	protected virtual void StateChanged(State state)
	{
	}
	protected virtual void MarkedChanged(bool marked)
	{
	}
	protected virtual void InspectedChanged(bool inspected)
	{
	}
	
	public static Block FromToken(char token)
	{
		if(token == '0')
		{
			return new PlainBlock();
		}
		else if(char.IsDigit(token))
		{
			return new DigitBlock();
		}
		else
		{
			throw new ArgumentException("Unsuported token: '" + token + "'");	
		}
	}
}
