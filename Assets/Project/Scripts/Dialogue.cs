using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
	public float textSpeed;

    [SerializeField] private Image imgBackground;
    [SerializeField] private Image rightCharacter;
    [SerializeField] private TMP_Text textSpaceToContinue;
    [SerializeField] private GameObject buttonSkipTuto;
	private float fadeDuration = 1f;

	private int index;

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
		if (Input.anyKeyDown || Input.GetMouseButton(0) || Input.mouseScrollDelta != Vector2.zero)
		{
			switch (index)
			{
				case 7: //wait for input "1" or "scroll"
					if ((Input.GetKeyDown(KeyCode.Alpha1)) || Input.mouseScrollDelta != Vector2.zero)
					{
						PassTextContinue(false);
						SpaceDialogue();
					}
					break;

				case 8:
					PassTextContinue(false);
					goto case 12;
				case 12: //wait for input "E"
					if (Input.GetKeyDown(KeyCode.E))
					{
						SpaceDialogue();
					}
					break;

				case 9: //wait for moving
					if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
					{
						PassTextContinue(false);
						SpaceDialogue();
					}
					break;

				case 6:
				case 11:
					if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
					{
						PassTextContinue(false);
						SpaceDialogue();
					}
					break;
				default:
					if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
					{
						PassTextContinue();
						SpaceDialogue();
					}
					break;
			}
		}
    }
	private void SpaceDialogue()
	{
		if (textComponent.text == dictionnaire[lines[index]]) //prochain texte
		{
			switch (index)
			{
				case 5:
					rightCharacter.gameObject.SetActive(false);
					StartCoroutine(FadeOutCoroutine());
					break;
			}
			NextLine();
		}
		else //passer le texte
		{
			StopAllCoroutines();
			if (index >= 6) imgBackground.gameObject.SetActive(false);
			textComponent.SetText(dictionnaire[lines[index]]);
			RectTransform obj = GetComponentInChildren<LayoutGroup>().gameObject.transform as RectTransform;
			GetComponentInChildren<LayoutGroup>().SetLayoutHorizontal();
			GetComponentInChildren<LayoutGroup>().SetLayoutVertical();
			LayoutRebuilder.ForceRebuildLayoutImmediate(obj);
		}
	}
	private void PassTextContinue(bool show = true) {
		if (!show /*&& textComponent.text == dictionnaire[lines[index]]*/)
		{
			textSpaceToContinue.text = " ";
			return;
		}

		textSpaceToContinue.text = "<b>Espace</b> pour continuer";
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
			buttonSkipTuto.SetActive(false);
			gameObject.SetActive(false);
        }
    }
	public void SkipTuto()
	{
		rightCharacter.gameObject.SetActive(false);
		//StartCoroutine(FadeOutCoroutine());
		imgBackground.gameObject.SetActive(false);
		buttonSkipTuto.SetActive(false);
		index = lines.Length;
		finishTuto = true;
		gameObject.SetActive(false);
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
}
