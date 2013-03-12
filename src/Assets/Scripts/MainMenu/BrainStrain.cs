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
		newObject.transform.parent = GameObject.Find("Level").transform;
		
		Block blockComponent = newObject.GetComponent<Block>();
		blockComponent.Id = IndexToId(new Vector3(x,y,z));
		blocks[x,y,z] = blockComponent.AddBlockFromToken(token);
		
		return newObject;
	}
	
	private char[,,] LoadBrainStrain()
	{
		var result = new char[25,9,31];
		for(var zz = 0; zz < result.GetLength(2); zz++)
		for(var yy = 0; yy < result.GetLength(1); yy++)
		for(var xx = 0; xx < result.GetLength(0); xx++)
		{
			result[xx,yy,zz] = '_';
		}
		
		var jsonString = ((TextAsset) Resources.Load("BrainStrain")).text;
		
		var list = (List<object>)Json.Deserialize(jsonString);
		foreach(var item in list)
		{
            var dict = (Dictionary<string, object>)item;
			var x = (int)(long)dict["x"];
			var y = (int)(long)dict["y"];
			var z = (int)(long)dict["z"];
			result[x,y,z] = '0';
		}
		return result;
	}
}
