using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Xml.Linq;

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
		public int type; // 255 eau, 1 bois, 2 marais, 3 ferme, 4/15/16/17/18/19/31/32/33 route, 10 prairie, 30 pont
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
			this.alea = alea; //10 route actuelles (unused), 20 cables enterr�, 30 ancienne route 
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

		TextAsset textFileTile = Resources.Load<TextAsset>(csvFileTile);
		TextAsset textFileConduct = Resources.Load<TextAsset>(csvFileConduct);
		TextAsset textFileSand = Resources.Load<TextAsset>(csvFileSand);
		TextAsset textFileAleas = Resources.Load<TextAsset>(csvFileAleas);
		TextAsset textFileCharb = Resources.Load<TextAsset>(csvFileCharbons);
		using StringReader readerTile = new StringReader(textFileTile.text);
		using StringReader readerConduct = new StringReader(textFileConduct.text);
		using StringReader readerSand = new StringReader(textFileSand.text);
		using StringReader readerAleas = new StringReader(textFileAleas.text);
		using StringReader readerCharb = new StringReader(textFileCharb.text);
		int y = 0;

		while (true)
		{
			string lineTile = readerTile.ReadLine();
			string lineConduct = readerConduct.ReadLine();
			string lineSand = readerSand.ReadLine();
			string lineAleas = readerAleas.ReadLine();
			string lineCharb = readerCharb.ReadLine();

			if (lineTile == string.Empty || lineTile == null)
			{
				break;
			}

			string[] valuesTile = lineTile.Split(',');
			string[] valuesConduct = lineConduct.Split(',');
			string[] valuesSand = lineSand.Split(',');
			string[] valuesAleas = lineAleas.Split(',');
			string[] valuesCharb = lineCharb.Split(',');

			for (int x = 0; x < valuesTile.Length; x++)
			{
				int tileIndexType = int.Parse(valuesTile[x]);
				float indexConduct = float.Parse(valuesConduct[x], CultureInfo.InvariantCulture);
				float tileIndexSand = float.Parse(valuesSand[x], CultureInfo.InvariantCulture);
				int indexAlea = int.Parse(valuesAleas[x]);
				int indexCharb = int.Parse(valuesCharb[x]);

				switch (tileIndexType)
				{
					case 10:
						tilemap.SetTile(new Vector3Int(x, -y, 0), TilesTypesDict[tileIndexType + Random.Range(0, 3)]);
						break;

					default:
						tilemap.SetTile(new Vector3Int(x, -y, 0), TilesTypesDict[tileIndexType]);
						break;
				}
				tilesDatas.Add(new Vector2(x, -y), new(tileIndexType, indexConduct, tileIndexSand, indexAlea, indexCharb));
			}
			y++;
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
		return !tilesDatas.ContainsKey(pos) || tilesDatas[pos].type.Equals(255);
	}
	private readonly HashSet<int> roadsTilesNumbers = new HashSet<int>() { 4, 15, 16, 17, 18, 19, 30, 31, 32, 33 };
	public bool IsRoad(Vector2 pos)
	{
		return roadsTilesNumbers.Contains(tilesDatas[pos].type);
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
