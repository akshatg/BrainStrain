using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class BrainStrain : MonoBehaviour {
	
	public GameObject BlockPrefab;
	
	private Vector3 RotationCenter
	{
		get
		{
			var vect = new Vector3();
			var scale = BlockPrefab.transform.localScale;
			vect.x = (blocks.GetLength(2) * scale.z) / 2f - scale.z / 2f;
			vect.y = (blocks.GetLength(1) * scale.y) / 2f - scale.y / 2f;
			vect.z = (blocks.GetLength(0) * scale.x) / 2f - scale.x / 2f;
			return vect;
		}
	}
	
	private float MinDistance
	{
		get
		{
		    var vect = new Vector3
		                   {
		                       x = blocks.GetLength(0), 
                               y = blocks.GetLength(1), 
                               z = blocks.GetLength(2)
		                   };
		    return vect.magnitude / 2f;
		}
	}
	
	private Block[,,] blocks;
	
	// Use this for initialization
	void Start()
	{
		var brainstrain = LoadBrainStrain();
		LoadLevel(brainstrain);
		
		var cameraControl = GameObject.Find("Main Camera").GetComponent<CameraControl>();
		if(cameraControl != null)
		{
			cameraControl.RotationCenter = RotationCenter;
			cameraControl.MinDistance = MinDistance + 1.5f;
			cameraControl.MaxDistance = cameraControl.MinDistance * 2.5f;
			cameraControl.RestartView();
		}
	}
	
	// Update is called once per frame
	void Update()
	{
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
	
	private void LoadLevel(char[,,] level)
	{
		blocks = new Block[level.GetLength(0),level.GetLength(1),level.GetLength(2)];
		for(var z = 0; z < level.GetLength(2); z++)
		for(var y = 0; y < level.GetLength(1); y++)
		for(var x = 0; x < level.GetLength(0); x++)
		{
			var token = level[x,y,z];
			if(token != '_')
			{
				CreateBlock(x, y, z, token);
			}
		}	
	}
	
	public GameObject CreateBlock(int x, int y, int z, char token)
	{
		var scale = BlockPrefab.transform.localScale;
		var posX = x * scale.x;
		var posY = y * scale.y;
		var posZ = z * scale.z;
		var posVect = Vector3.Scale(new Vector3(posZ, posY, posX), scale);
		
		var newObject = (GameObject)Instantiate(BlockPrefab, posVect, BlockPrefab.transform.rotation);
		newObject.name = "Block " + token;
		newObject.transform.parent = GameObject.Find("Brain Strain").transform;
		
		Block blockComponent = newObject.GetComponent<Block>();
		blockComponent.Id = Spawner.IndexToId(new Vector3(x,y,z));
		
		var component = newObject.AddComponent(Block.FromToken(token).GetType()) as Block;
		
		component.Textures = blockComponent.Textures;
		component.Palete = blockComponent.Palete;
		
		UnityEngine.Object.Destroy(blockComponent);
		
		if(component is PlainBlock)
		{
			var block = component as PlainBlock;
			blocks[x,y,z] = block;
		}
		else if(component is DigitBlock)
		{
			var block = component as DigitBlock;
			block.Number = (int)char.GetNumericValue(token);
			blocks[x,y,z] = block;
		}
		
		return newObject;
	}
}
