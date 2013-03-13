using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class BrainStrain : Spawner {
	
	// Use this for initialization
	void Start()
	{
		var level = Parser.ParseFromString(((TextAsset)Resources.Load("BrainStrain")).text);
		LoadLevel(level);
	}
	
	public override GameObject CreateBlock (int x, int y, int z, char token)
	{
		var scale = BlockPrefab.transform.localScale;
		var posX = x * scale.x;
		var posY = y * scale.y;
		var posZ = z * scale.z;
		var posVect = Vector3.Scale(new Vector3(posZ, posY, posX), scale);
		
		var newObject = (GameObject)Instantiate(BlockPrefab, posVect, BlockPrefab.transform.rotation);
		newObject.name = "Block " + token;
		newObject.transform.parent = GameObject.Find("BrainStrain").transform;
		
		Block blockComponent = newObject.GetComponent<Block>();
		blockComponent.Id = IndexToId(new Vector3(x,y,z));
		blocks[x,y,z] = blockComponent.AddBlockFromToken(token);
		
		return newObject;
	}
}
