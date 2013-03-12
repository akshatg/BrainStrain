using UnityEngine;
using System.Text;
using System.IO;

public static class Parser
{
	public static string SerializeToString(Block[,,] array)
	{
		var charArray = new char[array.GetLength(0),array.GetLength(1),array.GetLength(2)];
		for(int z = 0; z < array.GetLength(2); z++)
		{
			for(int y = 0; y < array.GetLength(1); y++)
			{
				for(int x = 0; x < array.GetLength(0); x++)
				{
					if(array[x,y,z] != null)
					{
						charArray[x,y,z] = array[x,y,z].ToChar();
					}
					else
					{
						charArray[x,y,z] = '_';	
					}
				}
			}
		}
		
		return SerializeToString(charArray);
	}
	
	public static string SerializeToString(char[,,] array)
	{
		var builder = new StringBuilder();
		builder.AppendLine("{");
		for(int z = 0; z < array.GetLength(0); z++)
		{
			builder.AppendLine("\t{");
			for(int y = 0; y < array.GetLength(1); y++)
			{
				builder.Append("\t\t{");
				for(int x = 0; x < array.GetLength(2) - 1; x++)
				{
					builder.AppendFormat("{0},", array[z,y,x]);
				}
				builder.AppendFormat("{0}", array[z,y,array.GetLength(2) - 1]);
				builder.AppendLine("},");
			}
			builder.AppendLine("\t},");
		}
		builder.AppendLine("}");
		return builder.ToString();
	}
	
	public static void SerializeToFile(char[,,] array, string path)
	{
		string str = SerializeToString(array);
		using(TextWriter tw = new StreamWriter(path))
		{
			tw.WriteLine(str);	
		}
	}
	
	public static char[,,] ParseFromString(string str)
	{
		RemoveWhiteSpace(ref str);
		RemoveComasAtTheEnd(ref str);
		var length = ReadArraySize(str);
		var x = (int)length.x;
		var y = (int)length.y;
		var z = (int)length.z;
		char[,,] array = new char[x,y,z];
		FillArray(array, str);
		return array;
    }
	
	public static char[,,] ParseFromFile(string path)
	{
		string str;
		using(TextReader reader = new StreamReader(path))
		{
			str = reader.ReadToEnd();
		}
		return ParseFromString(str);
    }
	
	private static void RemoveWhiteSpace(ref string str)
	{
		foreach(char c in str.ToString())
		{
			if(char.IsWhiteSpace(c))
			{
				str = str.Replace(c.ToString(), "");
			}
		}
	}
	
	private static void RemoveComasAtTheEnd(ref string str)
	{
		var s = str.ToString();
		
		for(int i = str.Length - 1; i >= 0; i--)
		{
			if(s[i] == '}' && s[i - 1] == ',')
			{
				str = str.Remove(i - 1, 1);
			}
		}
	}
	
	private static Vector3 ReadArraySize(string str)
	{
		int coordinate = -1;
		Vector3 vect = Vector3.zero;
		Vector3 length = Vector3.zero;
		
		foreach(char token in str)
		{
			switch(token)
			{
				case '{':
					coordinate++;
					break;
				case '}':
					length[coordinate] = vect[coordinate];
					vect[coordinate] = 0;
					coordinate--;
					break;
				case ',':
					vect[coordinate]++;
					break;
			}
		}
		return length + Vector3.one;
	}
	
	private static void FillArray(char[,,] array, string str)
	{
		int coordinate = -1;
		Vector3 vect = Vector3.zero;
		                          
		foreach(char token in str)
		{
			switch(token)
			{
				case '{':
					coordinate++;
					break;
				case '}':
					vect[coordinate] = 0;
					coordinate--;
					break;
				case ',':
					vect[coordinate]++;
					break;
				default:
					var x = (int)vect.x;
					var y = (int)vect.y;
					var z = (int)vect.z;
					array[x,y,z] = token;
					break;
			}
		}
	}
}
