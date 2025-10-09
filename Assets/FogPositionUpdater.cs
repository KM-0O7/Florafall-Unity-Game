using UnityEngine;

public class FogPositionUpdater : MonoBehaviour
{
    public Material fogMaterial;
    public Transform characterTransform;

    void Update()
    {
        Vector3 charPos = characterTransform.position;
        fogMaterial.SetVector("_CharacterPosition", new Vector4(charPos.x, charPos.y, 0, 0));
    }
}
