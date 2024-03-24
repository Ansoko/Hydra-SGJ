using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;
using System;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
	public float textSpeed;

    [SerializeField] private Image imgBackground;
    [SerializeField] private Image rightCharacter;
	private float fadeDuration = 1f;

	private int index;
    private bool isWaiting = false;

	public static Dialogue instance;
	private void Awake()
	{
		instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
		textComponent.text = string.Empty;
		LoadCSV();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetMouseButtonDown(0)||Input.GetKeyDown(KeyCode.Space)) && !isWaiting)
        {
            if (textComponent.text == dictionnaire[lines[index]])
            {
                switch (index)
                {
                    case 5:
						rightCharacter.gameObject.SetActive(false);
						StartCoroutine(FadeOutCoroutine());
						NextLine();
						break;

                    case 7: //wait for input "1" or "scroll"
                        StartCoroutine(WaitFor(()=>(Input.GetKeyDown(KeyCode.Alpha1))|| Input.mouseScrollDelta!=Vector2.zero));
                        break;

					case 8: //wait for input "E"
						StartCoroutine(WaitFor(() => Input.GetKeyDown(KeyCode.E)));
						break;

					case 9: //wait for moving
						StartCoroutine(WaitFor(() => (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)));
						break;

					case 12: //wait for input "E"
						StartCoroutine(WaitFor(() => Input.GetKeyDown(KeyCode.E)));
						break;

					default:
						NextLine();
                        break;
				}
                //NextLine();
            }
            else
            {
                StopAllCoroutines();
                if(index>=6) imgBackground.gameObject.SetActive(false);
				textComponent.SetText(dictionnaire[lines[index]]);
                RectTransform obj = GetComponentInChildren<LayoutGroup>().gameObject.transform as RectTransform;
				GetComponentInChildren<LayoutGroup>().SetLayoutHorizontal();
				GetComponentInChildren<LayoutGroup>().SetLayoutVertical();
				LayoutRebuilder.ForceRebuildLayoutImmediate(obj);
			}
        }
    }
    private IEnumerator WaitFor(Func<bool> untilThis){
        isWaiting = true;
		yield return new WaitUntil(untilThis);
		isWaiting = false;
		NextLine();
    }

	void StartDialogue(){
        index=0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine(){

        // Type each character 1 by 1 
        foreach (char c in dictionnaire[lines[index]].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length -1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
			finishTuto = true;
			gameObject.SetActive(false);
        }
    }


	private IEnumerator FadeOutCoroutine()
	{
		Color originalColor = imgBackground.color;
		float elapsedTime = 0f;

		while (elapsedTime < fadeDuration)
		{
			float completionPercentage = elapsedTime / fadeDuration;
			Color newColor = originalColor;
			newColor.a = Mathf.Lerp(originalColor.a, 0f, completionPercentage);
			imgBackground.color = newColor;
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		imgBackground.gameObject.SetActive(false);
	}

	private Dictionary<string, string> dictionnaire = new();
	public string csvFile;
	private bool finishTuto = false;

	void LoadCSV()
	{
		/*
		string filePath = Path.Combine(Application.streamingAssetsPath, csvFile);
		if (File.Exists(filePath))
		{
			string[] linesTile = reader.ReadLine().Split(',');

			for (int y = 0; y < linesTile.Length; y++)
			{
				string[] valuesTile = linesTile[y].Split(';');
				dictionnaire.Add(valuesTile[0], valuesTile[1]);
			}


			StartDialogue();
		}
		else
		{
			Debug.LogError("CSV file not found");
		}
		*/

		TextAsset textFile = Resources.Load<TextAsset>(csvFile);
		using StringReader reader = new StringReader(textFile.text);

		while (true)
		{
			string line = reader.ReadLine();
			if (line == string.Empty || line == null)
			{
				break;
			}
			string[] values = line.Split(';');
			dictionnaire.Add(values[0], values[1]);

		}
	}

	public string GetDialogueById(string id)
	{
		return dictionnaire[id];
	}

	public void InitOneDialogue(string txt)
	{
		if (!finishTuto) return;
		StopAllCoroutines();
		lines = new string[1];
		lines[0] = txt;
		gameObject.SetActive(true);
		textComponent.text = string.Empty;
		StartDialogue();
	}
	private int newDict = 0;
	public void InitOneNewDialogue(string txt)
	{
		if (!finishTuto) return;
		StopAllCoroutines();
		newDict--;
		dictionnaire.Add(newDict.ToString(), txt);
		lines = new string[1];
		lines[0] = newDict.ToString();
		gameObject.SetActive(true);
		textComponent.text = string.Empty;
		StartDialogue();
	}
}
