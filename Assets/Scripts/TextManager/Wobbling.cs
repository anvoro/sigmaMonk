using UnityEngine;
using TMPro;

public class Wobbling : MonoBehaviour
{
	public TMP_Text textComponent;

	private void Update()
	{
		textComponent.ForceMeshUpdate();
		
		var textInfo = textComponent.textInfo;
		foreach (var link in textComponent.textInfo.linkInfo)
		{
			if (link.GetLinkID() == "wobble")
			{
				for (var i = link.linkTextfirstCharacterIndex;
				     i < link.linkTextfirstCharacterIndex + link.linkTextLength;
				     i++)
				{
					var charInfo = textInfo.characterInfo[i];
					if (charInfo.isVisible == false)
					{
						continue;
					}
					
					var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
					for (var j = 0; j < 4; ++j)
					{
						var orig = verts[charInfo.vertexIndex + j];
						verts[charInfo.vertexIndex + j] =
							orig + new Vector3(
								0f,
								Mathf.PerlinNoise1D(Time.time * 2f + orig.x * 0.1f) * 10f,
								0f);
					}
				}

				for (var i = 0; i < textInfo.meshInfo.Length; ++i)
				{
					var meshInfo = textInfo.meshInfo[i];
					meshInfo.mesh.vertices = meshInfo.vertices;
					textComponent.UpdateGeometry(meshInfo.mesh, i);
				}
			}

			if (link.GetLinkID() == "fast")
			{
				//var oldDelay = delay;
			}

			if (link.GetLinkID() == "pause")
			{
			}
		}
	}
}