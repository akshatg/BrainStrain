using UnityEngine;
using System.Collections;

public class PlainBlock : Block
{
	public override char Char
	{
		get
		{
			return '0';
		}
	}
	
	protected override void Start()
	{
		base.Start();
		material.mainTexture = Textures[0];
	}
	
	protected override void OnInteract()
	{
		base.OnInteract();
		Musician.PlaySound(Musician.Sounds.BlockHit, transform.position);
	}
	
	protected override void StateChanged(State state)
	{
		material.color = Palete[(int)state];
		colider.enabled = (state == State.Unsolved);
	}
	
	protected override void InspectedChanged(bool inspected)
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
	
	protected override void MarkedChanged(bool marked)
	{
		material.mainTexture = Marked ? Textures[Textures.Length - 1] : Textures[0];
	}
}
