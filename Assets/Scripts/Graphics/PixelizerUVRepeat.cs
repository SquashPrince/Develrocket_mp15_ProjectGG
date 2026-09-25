using UnityEngine;

public class PixelizerUVRepeat : MonoBehaviour
{
    [SerializeField] private Vector2 uvMin = new Vector2(0.25f, 0.20f);
    [SerializeField] private Vector2 uvMax = new Vector2(0.50f, 0.45f);

    [SerializeField] private Vector2 repeat = new Vector2(4f, 4f);

    private Renderer rend;
    private Material mat;

    private void Awake()
    {
        rend = GetComponent<Renderer>();

        // 이 오브젝트만 사용하는 Material 생성
        mat = rend.material;

        mat.SetVector("_UVMin", uvMin);
        mat.SetVector("_UVMax", uvMax);
        mat.SetVector("_Repeat", repeat);

        // 텍스처가 영역 밖으로 반복되도록
        Texture tex = mat.GetTexture("_MainTex");

        if (tex != null)
        {
            tex.wrapMode = TextureWrapMode.Repeat;
        }
    }

}
