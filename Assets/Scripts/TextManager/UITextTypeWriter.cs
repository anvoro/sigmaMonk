using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using yutokun;

// attach to UI Text component (with the full text already there)

public class UITextTypeWriter: MonoBehaviour
{    
    public TMP_Text txt;
    string story;
    public float delay;
    float delayOld;
    private void Start()
    {
        txt = GetComponent<TMP_Text>();
        txt.text = "";
        var fabula = CSVParser.LoadFromPath("/plot.csv");
        string speaker;
        string currentLine;
        
    }
    void Awake()
    {
        
        story = txt.text;   
        txt.text = "";
        StartCoroutine(PlayText());

    }

    IEnumerator PlayText()
    {
        
        for (int i = 0; i < story.Length; i++)
        {
            if (story[i] == '<')
            {
                while (story[i] != '>')
                {
                    
                    txt.text += story[i];
                    i++;
                }
            }
            
            txt.text += story[i];
            yield return new WaitForSeconds(delay);
        }
    }

}