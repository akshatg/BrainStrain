using UnityEngine;
using System.Collections;

public class DigitBlock : Block
{
	public int Number{ get; set; }
	
	public override char Char
	{
		get
		{
			return char.Parse(Number.ToString());
		}
	}
	
	protected override void Start()
	{
		base.Start();
		material.mainTexture = Textures[Number];
	}
	
	protected override void OnInteract()
	{
		//do nothing for now(ignore informing spawner)
	}
	
	protected override void StateChanged(State state)
	{
		material.color = Palete[(int)state];
		colider.enabled = (state == State.Unsolved);
	}
}
