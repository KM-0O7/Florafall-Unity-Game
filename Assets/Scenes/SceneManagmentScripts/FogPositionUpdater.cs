using UnityEngine;

public class FogPositionUpdater : MonoBehaviour
{
    public Material fogMaterial;
    public Material fogmat2;
    public Transform characterTransform;

    private void Update()
    {
        Vector3 charPos = characterTransform.position;
        fogMaterial.SetVector("_CharacterPosition", new Vector4(charPos.x, charPos.y, 0, 0));
        fogmat2.SetVector("_CharacterPosition", new Vector4(charPos.x, charPos.y, 0, 0));
    }
}