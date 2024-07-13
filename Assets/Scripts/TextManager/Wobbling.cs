using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
using System.IO;
using System.Linq;
using UnityEngine.UI;
public class Wobbling : MonoBehaviour
{
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

                for (int i = link.linkTextfirstCharacterIndex; i < link.linkTextfirstCharacterIndex + link.linkTextLength; i++)
                {
                    var charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible)
                    {
                        continue;
                    }
                    var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                    for (int j = 0; j < 4; ++j)
                    {
                        var orig = verts[charInfo.vertexIndex + j];
                        //verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time*2f + orig.x*0.01f) * 10f, 0);
                        verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.PerlinNoise1D(Time.time * 2f + orig.x * 0.1f) * 10f, 0);

                    }

                }
                for (int i = 0; i < textInfo.meshInfo.Length; ++i)
                {
                    var meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    textComponent.UpdateGeometry(meshInfo.mesh, i);

                }
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
