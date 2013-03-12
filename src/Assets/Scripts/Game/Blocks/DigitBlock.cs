using UnityEngine;
using System.Collections;

public class DigitBlock : Block
{
	public int Number{ get; set; }
	
	protected override void Start()
	{
		base.Start();
		material.mainTexture = Textures[Number];
	}
	
	protected override void OnInteract()
	{
	}
	
	protected override void StateChanged(State state)
	{
		material.color = Palete[(int)state];
		colider.enabled = (state == State.Unsolved);
	}
}
