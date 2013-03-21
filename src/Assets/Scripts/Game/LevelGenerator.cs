using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public static class LevelGenerator
{
	
	public class Chanses
	{
		public static Chanses Easy{ get; private set; }
		public static Chanses Medium{ get; private set; }
		public static Chanses Hard{ get; private set; }
		public static Chanses Insane{ get; private set; }
		public static Chanses[] Mostly{ get; private set; }
		
		static Chanses()
		{
			Easy = new Chanses(60, 25, 15);
			Medium = new Chanses(40, 30, 20, 10);
			Hard = new Chanses(10, 15, 20, 40, 10, 5);
			Insane = new Chanses(5, 10, 15, 30, 15, 15, 10);
			
			Mostly = new Chanses[10];
			Mostly[0] = new Chanses();
			for(var i = 0; i < Mostly.Length - 1; i++)
			{
				var chances = new int[9];
				chances[i] = 100;
				Mostly[i + 1] = new Chanses(chances);
			}
		}
		
		private int[] chanses;
		
		public Chanses(params int[] chanses)
		{
			this.chanses = new int[9];
			var length = chanses.Length <= 9 ? chanses.Length : 9;
			for(var i = 0; i < length; i++)
			{
				this.chanses[i] = chanses[i];
			}
		}
		
		public int RandomNumber
		{
			get
			{
	            var roll = RandomChance;
	            for(var i = 0; i < chanses.Length; roll -= chanses[i], i++)
	            {
	                if(roll < chanses[i])
	                {
	                    return ++i;
	                }
	            }
	            return chanses.Length + 1;
			}
        }
	}
	
	private static readonly Vector3[] directions = new[]{
															Vector3.up,
															Vector3.down,
															Vector3.left,
															Vector3.right,
															Vector3.forward,
															Vector3.back
														};
	
	private static List<Vector3> RandomDirections
	{
		get
		{
			var result = new List<Vector3>();
			do
			{
				Vector3 dir;
				do
				{
					dir = directions[Random.Range(0,directions.Length)];
				}while(result.Contains(dir));
				result.Add(dir);
			}while(result.Count < directions.Length);
			return result;
		}
	}
	
	private static int RandomChance
	{
		get
		{
			return Random.Range(0, 100);	
		}
	}
	
	
	static LevelGenerator()
	{
		InitFromCube(4);
		Chance = Chanses.Easy;
		
		var generatedLevel = Generate();
		var result = Parser.SerializeToString(generatedLevel);
		Debug.Log(result);
	}
	
	private static bool[,,] visited;
	private static char[,,] resultLevel;
	private static int length;
	private static int lenx;
	private static int leny;
	private static int lenz;
	
	private static bool AllVisited
	{
		get
		{
			foreach(var v in visited)
				if(!v)
					return false;
			return true;
		}
	}
	
	public static Chanses Chance{ get; set; }
	
	public static void InitFromCube(int cubeSide)
	{
		var mask = new char[cubeSide,cubeSide,cubeSide];
		for(var z = 0; z < cubeSide; z++)
		for(var y = 0; y < cubeSide; y++)
		for(var x = 0; x < cubeSide; x++)
		{
			mask[x,y,z] = '0';
		}
		InitFromMask(mask);
	}
	
	public static void InitFromMask(char[,,] mask)
	{
		length = mask.Length;
		lenx = mask.GetLength(0);
		leny = mask.GetLength(1);
		lenz = mask.GetLength(2);
		resultLevel = new char[lenx,leny,lenz];
		visited = new bool[lenx,leny,lenz];
		for(int z = 0; z < lenz; z++)
		for(int y = 0; y < leny; y++)
		for(int x = 0; x < lenx; x++)
		{
			//this is a bit odd but it works
			var rule = mask[x,y,z] == '_';
			resultLevel[x,y,z] = rule ? '_' : mask[x,y,z];
			visited[x,y,z] = rule;
		}
	}
	
	public static char[,,] Generate()
	{
		//while not all blocks are visited
		while(AllVisited == false)
		{
			//pick random position that is not visited
			var pos = Vector3.zero;
			do
			{
				pos.x = Random.Range(0, lenx);
				pos.y = Random.Range(0, leny);
				pos.z = Random.Range(0, lenz);
			}while(visited[(int)pos.x,(int)pos.y,(int)pos.z]);
			
			//generate new sequence
			var number = Chance.RandomNumber;
			var sequence = new List<Vector3>();
			Permutate(number, pos, sequence);
			number = sequence.Count;
			
			foreach(var p in sequence)
			{
				resultLevel[(int)p.x,(int)p.y,(int)p.z] = '0';
				visited[(int)p.x,(int)p.y,(int)p.z] = true;
				MarkNeighbours(p);
			}
			
			//pick one random index in it and put a number
			var index = sequence[Random.Range(0, number)];
			Debug.Log("number = " + number);
			resultLevel[(int)index.x,(int)index.y,(int)index.z] = char.Parse(number.ToString());
		}
		return resultLevel;
	}
	
	private static void Permutate(int number, Vector3 pos, List<Vector3> sequence)
	{
		if(sequence.Count == number || OutOfRange(pos) || visited[(int)pos.x,(int)pos.y,(int)pos.z] || sequence.Contains(pos))
			return;
		
		sequence.Add(pos);
		
		var dirs = RandomDirections;
		for(int i = 0; i < dirs.Count; i++)
		{
			if(sequence.Count == number)
				break;
			Permutate(number, pos + dirs[i], sequence);
		}
	}
	
	private static void MarkNeighbours(Vector3 pos)
	{
		foreach(var dir in directions)
		{
			var id = pos + dir;
			if(InRange(id))
			{
				visited[(int)id.x,(int)id.y,(int)id.z] = true;
			}
		}
	}
	
	private static bool InRange(Vector3 pos)
	{
		return pos.x >= 0 && pos.x < lenx &&
			   pos.y >= 0 && pos.y < leny &&
			   pos.z >= 0 && pos.z < lenz;
	}
	
	private static bool OutOfRange(Vector3 pos)
	{
		return !InRange(pos);
	}
}
