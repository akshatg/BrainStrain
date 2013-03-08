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
	
	public int Number{ get; set; }
	public int Id{ get; set; }
	public bool IsDiggit
	{
		get
		{
			return Number != 0;
		}
	}
	public State State
	{ 
		get
		{
			return state;
		}
		set
		{
			state = value;
			material.mainTexture = Textures[Number];
			material.color = Palete[(int)state];
			colider.enabled = (state == State.Unsolved);
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
			marked = value;
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
			inspected = value;
			
			if(!IsDiggit)
			{
				if(State == State.Unsolved)
				{
					if(inspected)
					{
						var col = material.color;
						col.a = 0.5f;
						material.color = col;
						colider.enabled = false;
					}
					else
					{
						var col = material.color;
						col.a = 1f;
						material.color = col;
						colider.enabled = true;
					}
				}
			}
		}
	}
	
	private Material material;
	private Collider colider;
	private State state;
	private bool marked;
	private bool inspected;
	
	private bool pressed;
	private float time;
	
	void Awake()
	{
		material = GetComponent<Renderer>().material;
		material.mainTexture = Textures[Number];
		colider = GetComponent<Collider>();
	}
	
	void Start()
	{
		State = State.Unsolved;
	}
	
	// Update is called once per frame
	void Update()
	{
		if(Marked && material.mainTexture != Textures[Textures.Length - 1])
		{
			material.mainTexture = Textures[Textures.Length - 1];
		}
		else if(!Marked && material.mainTexture != Textures[Number])
		{
			material.mainTexture = Textures[Number];
		}
	}
	
	void OnMouseUpAsButton()
	{	
		if(State == State.Unsolved)
		{
			if(!IsDiggit)
			{
				Musician.PlaySound(Musician.Sounds.BlockHit, transform.position);
				
				var obj = GameObject.Find("Main Camera").GetComponent<Spawner>();
				if(obj != null)
				{
					obj.SendMessage("BlockPressed", Id);
				}
			}
		}
	}
}
