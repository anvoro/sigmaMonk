using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using System.IO;
using System.Linq;
using UnityEngine.UI;
public class LinkManager : MonoBehaviour
{
    public float delay;
    public TMP_Text textComponent;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {;
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;
        foreach (TMP_LinkInfo link in textComponent.textInfo.linkInfo)
        {
            if (link.GetLinkID() == "wobble")
            {
                Wobbling.Wobble(textComponent, link);
            }
            if (link.GetLinkID() == "fast")
            {
                var oldDelay = delay;

            }
            if (link.GetLinkID() == "pause")
            {
                
            }
            
        }

        
        

       
        




    }
    }
