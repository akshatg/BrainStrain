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
	
	protected override void StateChanged(State state)
	{
		material.color = Palete[(int)state];
		colider.enabled = (state == State.Unsolved);
	}
	
	protected override void InspectedChanged(bool inspected)
	{
		if(State == State.Unsolved)
		{
			var col = material.color;
			col.a = inspected ? 0.5f : 1f;
			material.color = col;
			colider.enabled = !inspected;
		}
	}
	
	protected override void MarkedChanged(bool marked)
	{
		material.mainTexture = Marked ? Textures[Textures.Length - 1] : Textures[0];
	}
}
