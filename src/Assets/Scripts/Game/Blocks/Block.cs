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
	
	public virtual char Char
	{
		get
		{
			return ' ';
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
	
	protected virtual void StateChanged(State state){ }
	protected virtual void MarkedChanged(bool marked){ }
	protected virtual void InspectedChanged(bool inspected){ }
	protected virtual void InitFromToken(char token){ }
	
	public char ToChar()
	{
		return this.Char;
	}
	
	public Block AddBlockFromToken(char token)
	{		
		Block newBlock = gameObject.AddComponent(BlockTypeFromToken(token)) as Block;
		newBlock.Id = this.Id;
		newBlock.Textures = this.Textures;
		newBlock.Palete = this.Palete;
		newBlock.InitFromToken(token);
		UnityEngine.Object.Destroy(this); //get rid of parent object
		return newBlock;
	}
	
	public static Type BlockTypeFromToken(char token)
	{
		if(token == '0')
		{
			return typeof(PlainBlock);
		}
		else if(char.IsDigit(token))
		{
			return typeof(DigitBlock);
		}
		else
		{
			throw new ArgumentException("Unsuported token: '" + token + "'");	
		}
	}
}
