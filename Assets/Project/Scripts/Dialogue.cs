using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.SetText(lines[index]);
                RectTransform obj = GetComponentInChildren<LayoutGroup>().gameObject.transform as RectTransform;
				GetComponentInChildren<LayoutGroup>().SetLayoutHorizontal();
				GetComponentInChildren<LayoutGroup>().SetLayoutVertical();
				LayoutRebuilder.ForceRebuildLayoutImmediate(obj);
			}
        }
    }

	void StartDialogue(){
        index=0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine(){

        // Type each character 1 by 1 
        foreach (char c in lines[index].ToCharArray())
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
            gameObject.SetActive(false);
        }
    }

}
