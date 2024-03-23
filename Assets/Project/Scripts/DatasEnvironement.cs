using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

public class DatasEnvironement : MonoBehaviour
{
	public Tilemap tilemap;	
	[SerializeField] private TileType<string, int> TilesTypes;
	private Dictionary<int, TileBase> TilesTypesDict;

	public string csvFileTile;
	public string csvFileConduct;
	public string csvFileSand;
	public string csvFileAleas;
	public string csvFileCharbons;

	public Transform FlagsParent;

	public Dictionary<Vector2, TileData> tilesDatas = new Dictionary<Vector2, TileData>();

	public class TileData
	{
		public int type; // 255 eau, 1 bois, 2 marais, 3 ferme, 4 route, 10 prairie, 30 pont
		public float conductivite;
		public float sand;
		public int alea; //
		public int charbons;
		public bool conductRevealed;
		public bool sandRevealed;

		public TileData(int type, float conductivite, float sand, int alea, int charbons)
		{
			this.type = type;
			this.sand = sand;
			this.conductivite = conductivite;
			this.alea = alea; //10 route actuelles, 20 cables enterré, 30 ancienne route 
			this.charbons = charbons; // 0 rien, 100 sable, 1 limon, 101 sable + limon
		}
	}
	public void RevealConduct(Vector2 pos, bool reveal = true)
	{
		tilesDatas[pos].conductRevealed = reveal;
	}
	public void RevealSand(Vector2 pos, bool reveal = true)
	{
		tilesDatas[pos].sandRevealed = reveal;
	}
	public bool HasBeenPlanted(Vector2 pos)
	{
		return tilesDatas.ContainsKey(pos) && (tilesDatas[pos].conductRevealed || tilesDatas[pos].sandRevealed);
	}

	public static DatasEnvironement Instance;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		TilesTypesDict = TilesTypes.ToDictionary();
		LoadCSV();
	}

	void LoadCSV()
	{
		string filePathTile = Path.Combine(Application.streamingAssetsPath, csvFileTile);
		string filePathConduct = Path.Combine(Application.streamingAssetsPath, csvFileConduct);
		string filePathSand = Path.Combine(Application.streamingAssetsPath, csvFileSand);
		string filePathAlea = Path.Combine(Application.streamingAssetsPath, csvFileAleas);
		string filePathCharb = Path.Combine(Application.streamingAssetsPath, csvFileCharbons);

		if (File.Exists(filePathTile)&& File.Exists(filePathSand) && File.Exists(filePathAlea) && File.Exists(filePathConduct))
		{
			string[] linesTile = File.ReadAllLines(filePathTile);
			string[] linesConduct = File.ReadAllLines(filePathConduct);
			string[] linesSand = File.ReadAllLines(filePathSand);
			string[] linesAleas = File.ReadAllLines(filePathAlea);
			string[] linesCharb = File.ReadAllLines(filePathCharb);

			for (int y = 0; y < linesTile.Length; y++)
			{
				string[] valuesTile = linesTile[y].Split(',');
				string[] valuesConduct = linesConduct[y].Split(',');
				string[] valuesSand = linesSand[y].Split(';');
				string[] valuesAleas = linesAleas[y].Split(',');
				string[] valuesCharb = linesCharb[y].Split(',');

				for (int x = 0; x < valuesTile.Length; x++)
				{
					int tileIndexType = int.Parse(valuesTile[x]);
					float indexConduct = float.Parse(valuesConduct[x], CultureInfo.InvariantCulture);
					float tileIndexSand = float.Parse(valuesSand[x], CultureInfo.InvariantCulture);
					int indexAlea = int.Parse(valuesAleas[x]);
					int indexCharb = int.Parse(valuesCharb[x]);

					tilemap.SetTile(new Vector3Int(x, -y, 0), TilesTypesDict[tileIndexType]);
					tilesDatas.Add(new Vector2(x, -y), new(tileIndexType, indexConduct, tileIndexSand, indexAlea, indexCharb));
				}
			}
		}
		else
		{
			Debug.LogError("CSV file not found");
		}
	}

	public float GetConductValue(Vector2 pos)
	{
		return tilesDatas[pos].conductivite;
	}
	public float GetSandValue(Vector2 pos)
	{
		return tilesDatas[pos].sand;
	}
	public int GetCharbValue(Vector2 pos)
	{
		return tilesDatas[pos].charbons;
	}
	public float GetAlea(Vector2 pos)
	{
		return tilesDatas[pos].alea;
	}
	public bool IsWater(Vector2 pos)
	{
		return tilesDatas[pos].type.Equals(255);
	}
}


[System.Serializable]
public class TileType<TKey, TValue>
{
	public TileBase[] keys;
	public int[] values;

	public Dictionary<int, TileBase> ToDictionary()
	{
		var dictionary = new Dictionary<int, TileBase>();
		for (int i = 0; i < keys.Length; i++)
		{
			dictionary.Add(values[i], keys[i]);
		}
		return dictionary;
	}
}
