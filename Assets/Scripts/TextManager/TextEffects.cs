using UnityEngine;
using TMPro;

public static class TextEffects
{
    public static void Wobble(TMP_Text textComponent, TMP_LinkInfo link)
    {
        textComponent.ForceMeshUpdate();
        
        var textInfo = textComponent.textInfo;
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
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}