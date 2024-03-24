using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;
using System;

public class DialogueBubble : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
	public float textSpeed;


    [SerializeField] private Image imgBackground;
    [SerializeField] private Image rightCharacter;
	private float fadeDuration = 1f;

	private int index;
    private bool isWaiting = false;

	// Start is called before the first frame update
	void Start()
    {
		textComponent.text = string.Empty;
		LoadCSV();
		gameObject.SetActive(false);
    }

    private IEnumerator WaitFor(Func<bool> untilThis){
        isWaiting = true;
		yield return new WaitUntil(untilThis);
		isWaiting = false;
		NextLine();
    }

	void  StartDialogue(){
        index=0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine(){
		Debug.Log(dictionnaire[lines[index]]);
        // Type each character 1 by 1 
        foreach (char c in dictionnaire[lines[index]].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
		
    }

	public void HideDialog(){
		gameObject.SetActive(false);
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


	void LoadCSV()
	{
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
