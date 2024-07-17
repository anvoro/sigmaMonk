using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TMP_Text))]
public class TextLinkProcessor : MonoBehaviour
{
    private TMP_Text _text;

    [SerializeField]
    private TextTypeWriter _textWriter;
    
    [Header("Shake")]
    public float AngleMultiplier = 1.0f;
    public float CurveScale = 1.0f;
    
    private bool _hasTextChanged;
    private TMP_MeshInfo[] cachedMeshInfo;
    private float _timeSinceLastJitter;
    
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();

        if (_textWriter != null)
        {
            _textWriter.OnPlayComplete += CheckTextChange;
        }
    }

    private void OnDestroy()
    {
        if (_textWriter != null)
        {
            _textWriter.OnPlayComplete -= CheckTextChange;
        }
    }

    private void CheckTextChange()
    {
        _hasTextChanged = true;
    }

    private void Update()
    {
        if (_textWriter != null && _textWriter.PlayComplete == false)
        {
            return;
        }
        
        var textInfo = _text.textInfo;
        if (textInfo.linkCount == 0)
        {
            return;
        }
        
        for (var index = 0; index < textInfo.linkCount; index++)
        {
            var link = textInfo.linkInfo[index];
            if (link.GetLinkID() == "wobble")
            {
                _text.ForceMeshUpdate();
                
                for (int i = link.linkTextfirstCharacterIndex;
                     i < link.linkTextfirstCharacterIndex + link.linkTextLength;
                     i++)
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
                        verts[charInfo.vertexIndex + j] = orig + new Vector3(0,
                            Mathf.PerlinNoise1D(Time.time * 2f + orig.x * 0.1f) * 10f, 0);
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
                if (_hasTextChanged)
                {
                    _text.ForceMeshUpdate();
                    cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
                    _hasTextChanged = false;
                }

                if (cachedMeshInfo == null)
                {
                    _text.ForceMeshUpdate();
                    cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
                }
                
                if (_timeSinceLastJitter > .1f)
                {
                    _timeSinceLastJitter = 0;
                    
                    for (int i = link.linkTextfirstCharacterIndex;
                     i < link.linkTextfirstCharacterIndex + link.linkTextLength;
                     i++)
                    {
                        var charInfo = textInfo.characterInfo[i];

                        if (!charInfo.isVisible)
                        {
                            continue;
                        }
                        
                        int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                        int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                        
                        Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
                        Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
                        Vector3 offset = charMidBasline;

                        Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                        destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                        destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                        destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                        destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                        Vector3 jitterOffset = new Vector3(Random.Range(-.25f, .25f), Random.Range(-.25f, .25f), 0);

                        var matrix = Matrix4x4.TRS(jitterOffset * CurveScale, Quaternion.Euler(0, 0, Random.Range(-5f, 5f) * AngleMultiplier), Vector3.one);

                        destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                        destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                        destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                        destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                        destinationVertices[vertexIndex + 0] += offset;
                        destinationVertices[vertexIndex + 1] += offset;
                        destinationVertices[vertexIndex + 2] += offset;
                        destinationVertices[vertexIndex + 3] += offset;
                    }
                    
                    for (int i = 0; i < textInfo.meshInfo.Length; i++)
                    {
                        textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                        _text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                    }
                }

                _timeSinceLastJitter += Time.deltaTime;
                
                // for (int i = link.linkTextfirstCharacterIndex;
                //      i < link.linkTextfirstCharacterIndex + link.linkTextLength;
                //      i++)
                // {
                //     if (!textInfo.characterInfo[i].isVisible)
                //         continue;
                //
                //     TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                //     int vertexIndex = charInfo.vertexIndex;
                //     int materialIndex = charInfo.materialReferenceIndex;
                //
                //     Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;
                //
                //     float offsetX = Random.Range(-Magnitude, Magnitude);
                //     float offsetY = Random.Range(-Magnitude, Magnitude);
                //
                //     sourceVertices[vertexIndex + 0] += new Vector3(offsetX, offsetY, 0);
                //     sourceVertices[vertexIndex + 1] += new Vector3(offsetX, offsetY, 0);
                //     sourceVertices[vertexIndex + 2] += new Vector3(offsetX, offsetY, 0);
                //     sourceVertices[vertexIndex + 3] += new Vector3(offsetX, offsetY, 0);
                // }
                //
                // for (int i = 0; i < textInfo.meshInfo.Length; i++)
                // {
                //     textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                //     _text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                // }
            }
        }
    }
}
