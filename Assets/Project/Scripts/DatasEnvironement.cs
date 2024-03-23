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
	public string csvFileSand;

	public Dictionary<Vector2, TileData> tilesDatas = new Dictionary<Vector2, TileData>();

	public struct TileData
	{
		public int type;
		public float sand;

		public TileData(int type, float sand)
		{
			this.type = type;
			this.sand = sand;
		}
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
		string filePathSand = Path.Combine(Application.streamingAssetsPath, csvFileSand);

		if (File.Exists(filePathTile)&& File.Exists(filePathSand))
		{
			string[] linesTile = File.ReadAllLines(filePathTile);
			string[] linesSand = File.ReadAllLines(filePathSand);

			for (int y = 0; y < linesTile.Length; y++)
			{
				string[] valuesTile = linesTile[y].Split(',');
				string[] valuesSand = linesSand[y].Split(';');

				for (int x = 0; x < valuesTile.Length; x++)
				{
					int tileIndexType = int.Parse(valuesTile[x]);
					float tileIndexSand = float.Parse(valuesSand[x], CultureInfo.InvariantCulture);
					tilemap.SetTile(new Vector3Int(x, -y, 0), TilesTypesDict[tileIndexType]);
					tilesDatas.Add(new Vector2(x, -y), new(tileIndexType, tileIndexSand));
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
