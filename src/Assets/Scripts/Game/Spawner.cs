using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MiniJSON;

public enum Tool
{
	Block,
	Marker,
	Inspector
}

public class Spawner : MonoBehaviour {
	
	public GameObject BlockPrefab;
	
	public bool LevelSolved{ get; private set; }
	public int StartingUndos{ get; private set; }
	public int UndosLeft{ get; set; }
	public int NumberOfStars
	{
		get
		{
			int stars;
			if(UndosLeft >= 0)
				stars = Mathf.CeilToInt(Global.Utills.ScaleValue(UndosLeft, 0, StartingUndos, 1, 3));
			else
				stars = 0;
			return stars;
		}
	}
	public Tool CurrentTool
	{ 
		get
		{
			return currentTool;
		}
		set
		{
			currentTool = value;
			
			foreach(var block in blocks)
			{
				if(block != null)
				{
					block.Inspected = (currentTool == Tool.Inspector);
				}
			}
		}
	}
	
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
	
	private Global _global;
	private Tool currentTool;
	protected Block[,,] blocks;
	private Stack<int> undoStack;
	
	// Use this for initialization
	void Start()
	{
		_global = GameObject.Find("Global").GetComponent<Global>();
		LoadLevel(_global.Data);
	}
	
	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.I))
		{
			CurrentTool = Tool.Inspector;
		}
		if(Input.GetKeyDown(KeyCode.M))
		{
			CurrentTool = Tool.Marker;
		}
		if(Input.GetKeyDown(KeyCode.B))
		{
			CurrentTool = Tool.Block;
		}
		if(Input.GetKeyDown(KeyCode.U))
		{
			UndoBlock();
		}
		if(Input.GetKeyDown(KeyCode.R))
		{
			RestartLevel();
		}
	}
	
	public void LoadLevel(char[,,] level)
	{
		DestroyLevel();
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
		LevelSolved = false;
		ResetUndos();
		ResetCamera();
	}
	
	public void DestroyLevel()
	{
		if(blocks != null)
			foreach(var block in blocks)
				if(block != null)
					Destroy(block.gameObject);
	}
	
	public void DestroyBlock(int id)
	{
		var index = IdToIndex(id);
		var block = blocks[(int)index.x,(int)index.y,(int)index.z];
		
		Musician.PlaySound(Musician.Sounds.BlockHit, block.transform.position);
		
		undoStack.Push(block.Id);
		Destroy(block.gameObject);
		blocks[(int)index.x,(int)index.y,(int)index.z] = null;
	}
	
	public void RestartLevel()
	{
		while(UndoBlock()){ }
	    foreach(var block in blocks)
		{
			if(block != null)
			{
				block.Marked = false;
				block.State = State.Unsolved;
			}
		}
		ResetUndos();
	}
	
	public void BlockPressed(int id)
	{
		var index = IdToIndex(id);
		var block = blocks[(int)index.x, (int)index.y, (int)index.z];
		
		if(CurrentTool == Tool.Marker)
		{
			block.Marked = !block.Marked;
		}
		else if(CurrentTool == Tool.Block)
		{
			if(block.Marked == false && block is PlainBlock)
			{
				DestroyBlock(id);
			}
		}
		ValidateLevel();
	}
	
	public bool UndoBlock()
	{
		if(undoStack.Count > 0)
		{
			CreateBlock(undoStack.Pop(), '0');
			UndosLeft--;
			ValidateLevel();
			return true;
		}
		return false;
	}
	
	public GameObject CreateBlock(int id, char token)
	{
		var index = IdToIndex(id);
		return CreateBlock((int)index.x, (int)index.y, (int)index.z, token);
	}
	
	public virtual GameObject CreateBlock(int x, int y, int z, char token)
	{
		var scale = BlockPrefab.transform.localScale;
		var posX = x * scale.x;
		var posY = (blocks.GetLength(1) - 1 - y) * scale.y;
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
	
	public int BlocksCount()
	{
		return blocks.Length;
	}
	public int BlocksCount(Func<Block,bool> sieve)
	{
		int count = 0;
		foreach(var block in blocks)
			if(sieve(block))
				count++;
		return count;
	}
	
	private void ValidateLevel()
	{
		CountSeparatedGroups();
		LevelSolved = IsLevelSolved();
	}
	
	private void ResetUndos()
	{
		const int minUndos = 1;
		const int maxUndos = 15;
		
		var count = BlocksCount(b => b != null && b is PlainBlock);
		StartingUndos =  Mathf.Clamp(count / 5, minUndos, maxUndos);
		
		UndosLeft = StartingUndos;
		undoStack = new Stack<int>();
	}
	
	private bool IsLevelSolved()
	{
		foreach(var block in blocks)
		{
			if(block != null && block.State != State.Solved)
			{
				return false;
			}
		}
		return true;
	}
	
	private void CountSeparatedGroups()
	{
		foreach (var block in blocks)
		{
			if(block != null && block is DigitBlock)
			{
				var digitBlock = block as DigitBlock;
				var sequence = new List<Block>();
				var sequenceCount = CountSeparatedFrom(digitBlock, sequence);
				
				State state = State.Unsolved;
				
				//if and only if the sequence contains exactly one diggit the sequence is valid
				if(sequence.Count(b => b is DigitBlock) == 1)
				{
					if(sequenceCount == digitBlock.Number)
						state = State.Solved;
					else if(sequenceCount < digitBlock.Number)
						state = State.Wrong;
					else
						state = State.Unsolved;
				}
				
				foreach (Block item in sequence)
				{
					item.State = state;
				}
			}
		}
	}
	
	//use this instead
	private int CountSeparatedFrom(DigitBlock block, List<Block> sequence)
	{
		return CountSeparatedFrom(block.Id, sequence, block.Number);
	}
	
	//I am proud of this :D RECURSION!!!
	private int CountSeparatedFrom(int blockId, List<Block> sequence, int targetCount)
	{
		//this is for calculation optimization so that it doesn't count the whole array for every digit block
		if(sequence.Count > targetCount)
		{
			return 0;
		}
		
		var index = IdToIndex(blockId);
		//check if block is in array bounds
		for (int i = 0; i < 3; i++)
			if(index[i] < 0 || index[i] >= blocks.GetLength(i))
				return 0;
		
		var block = blocks[(int)index.x, (int)index.y, (int)index.z];
		
		//if there is no block or the block is already counted return 0
		if(block == null || sequence.IndexOf(block) >= 0)
		{
			return 0;
		}
		
		sequence.Add(block);
		
		return 1 +
			CountSeparatedFrom(IndexToId(index + Vector3.forward), sequence, targetCount) +
			CountSeparatedFrom(IndexToId(index + Vector3.right), sequence, targetCount) +
			CountSeparatedFrom(IndexToId(index + Vector3.back), sequence, targetCount) +
			CountSeparatedFrom(IndexToId(index + Vector3.left), sequence, targetCount) +
			CountSeparatedFrom(IndexToId(index + Vector3.up), sequence, targetCount) +
			CountSeparatedFrom(IndexToId(index + Vector3.down), sequence, targetCount);
	}
	
	private void ResetCamera()
	{
		//set the rotation center of camera controller
		var cameraControl = GetComponent<CameraControl>();
		if(cameraControl != null)
		{
			cameraControl.RotationCenter = RotationCenter;
			cameraControl.MinDistance = MinDistance + 1.5f;
			cameraControl.MaxDistance = cameraControl.MinDistance * 2.5f;
			cameraControl.RestartView();
		}
	}
	
	//pure magick
	public static int IndexToId(Vector3 index)
	{
		int id = 0;
		for (int i = 0; i < 3; i++)
			id |= (int)index[i] << 8 * i;
		return id;
	}
	//pure anti-magick
	public static Vector3 IdToIndex(int id)
	{
		Vector3 index = Vector3.zero;
		for (int i = 0; i < 3; i++)
			index[i] = (id & (255 << 8 * i)) >> 8 * i;
		return index;
	}
}