using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TMP_Text))]
public class TextLinkProcessor : MonoBehaviour
{
    private TMP_Text _text;
    
    [Header("Shake")]
    public float Magnitude = 2f;
    
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    
    private void Update()
    {
        _text.ForceMeshUpdate();
        var textInfo = _text.textInfo;
        
        foreach (TMP_LinkInfo link in textInfo.linkInfo)
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
                        verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.PerlinNoise1D(Time.time * 2f + orig.x * 0.1f) * 10f, 0);
                    }
                }
        
                for (int i = 0; i < textInfo.meshInfo.Length; ++i)
                {
                    var meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    _text.UpdateGeometry(meshInfo.mesh, i);
                }
            }

            if (link.GetLinkID() == "shake")
            {
                for (int i = link.linkTextfirstCharacterIndex; i < link.linkTextfirstCharacterIndex + link.linkTextLength; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible)
                        continue;

                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    int vertexIndex = charInfo.vertexIndex;
                    int materialIndex = charInfo.materialReferenceIndex;

                    Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;

                    float offsetX = Random.Range(-Magnitude, Magnitude);
                    float offsetY = Random.Range(-Magnitude, Magnitude);

                    sourceVertices[vertexIndex + 0] += new Vector3(offsetX, offsetY, 0);
                    sourceVertices[vertexIndex + 1] += new Vector3(offsetX, offsetY, 0);
                    sourceVertices[vertexIndex + 2] += new Vector3(offsetX, offsetY, 0);
                    sourceVertices[vertexIndex + 3] += new Vector3(offsetX, offsetY, 0);
                }

                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    _text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }
            }

            if (link.GetLinkID() == "fast")
            {
            }
            
            if (link.GetLinkID() == "pause")
            {
            }
        }
    }
}
