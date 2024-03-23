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

	public Transform FlagsParent;

	public Dictionary<Vector2, TileData> tilesDatas = new Dictionary<Vector2, TileData>();

	public class TileData
	{
		public int type;
		public int conductivite;
		public float sand;
		public int alea;
		public bool conductRevealed;
		public bool sandRevealed;

		public TileData(int type, int conductivite, float sand, int alea)
		{
			this.type = type;
			this.sand = sand;
			this.conductivite = conductivite;
			this.alea = alea;
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

		if (File.Exists(filePathTile)&& File.Exists(filePathSand) && File.Exists(filePathAlea) && File.Exists(filePathConduct))
		{
			string[] linesTile = File.ReadAllLines(filePathTile);
			string[] linesConduct = File.ReadAllLines(filePathConduct);
			string[] linesSand = File.ReadAllLines(filePathSand);
			string[] linesAleas = File.ReadAllLines(filePathAlea);

			for (int y = 0; y < linesTile.Length; y++)
			{
				string[] valuesTile = linesTile[y].Split(',');
				string[] valuesConduct = linesTile[y].Split(',');
				string[] valuesSand = linesSand[y].Split(';');
				string[] valuesAleas = linesTile[y].Split(',');

				for (int x = 0; x < valuesTile.Length; x++)
				{
					int tileIndexType = int.Parse(valuesTile[x]);
					int indexConduct = int.Parse(valuesTile[x]);
					float tileIndexSand = float.Parse(valuesSand[x], CultureInfo.InvariantCulture);
					int indexAlea = int.Parse(valuesTile[x]);

					tilemap.SetTile(new Vector3Int(x, -y, 0), TilesTypesDict[tileIndexType]);
					tilesDatas.Add(new Vector2(x, -y), new(tileIndexType, indexConduct, tileIndexSand, indexAlea));
				}
			}
		}
		else
		{
			Debug.LogError("CSV file not found");
		}
	}

	public float GetSandValue(Vector2 pos)
	{
		return tilesDatas[pos].sand;
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
