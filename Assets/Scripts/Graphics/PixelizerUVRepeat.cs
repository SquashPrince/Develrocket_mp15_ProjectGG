using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(Renderer))]
public class PixelizerUVRepeat : MonoBehaviour
{
    [Header("UV Channel")]
    [Tooltip("UV channel used by the Pixelizer Base Map. Usually 0.")]
    [Range(0, 3)]
    public int uvChannel = 0;

    private static readonly int AutoUVMinID = Shader.PropertyToID("_AutoUVMin");
    private static readonly int AutoUVSizeID = Shader.PropertyToID("_AutoUVSize");

    private Renderer targetRenderer;
    private MaterialPropertyBlock propertyBlock;

    private Mesh cachedMesh;
    private int cachedUVChannel = -1;

    private Vector2 uvMin = Vector2.zero;
    private Vector2 uvSize = Vector2.one;

    private void OnEnable()
    {
        UpdateUVBounds(true);
    }

    private void OnValidate()
    {
        uvChannel = Mathf.Clamp(uvChannel, 0, 3);
        UpdateUVBounds(true);
    }

    private void Reset()
    {
        UpdateUVBounds(true);
    }

    private void Update()
    {
#if UNITY_EDITOR
        // In Edit Mode, automatically react when the assigned mesh changes.
        if (!Application.isPlaying)
        {
            Mesh mesh = GetMesh();

            if (mesh != cachedMesh || cachedUVChannel != uvChannel)
                UpdateUVBounds(true);
        }
#endif
    }

    [ContextMenu("Refresh UV Bounds")]
    public void RefreshUVBounds()
    {
        UpdateUVBounds(true);
    }

    private void UpdateUVBounds(bool force)
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer == null)
            return;

        Mesh mesh = GetMesh();

        if (mesh == null)
        {
            ApplyUVBounds(Vector2.zero, Vector2.one);
            return;
        }

        if (!force && mesh == cachedMesh && cachedUVChannel == uvChannel)
            return;

        cachedMesh = mesh;
        cachedUVChannel = uvChannel;

        Vector2[] uvs = GetUVs(mesh, uvChannel);

        if (uvs == null || uvs.Length == 0)
        {
            Debug.LogWarning(
                $"[{nameof(PixelizerUVRepeat)}] '{name}' has no UV{uvChannel} data. " +
                "Using the full 0-1 UV range.",
                this
            );

            ApplyUVBounds(Vector2.zero, Vector2.one);
            return;
        }

        Vector2 min = uvs[0];
        Vector2 max = uvs[0];

        for (int i = 1; i < uvs.Length; i++)
        {
            Vector2 uv = uvs[i];

            min.x = Mathf.Min(min.x, uv.x);
            min.y = Mathf.Min(min.y, uv.y);

            max.x = Mathf.Max(max.x, uv.x);
            max.y = Mathf.Max(max.y, uv.y);
        }

        Vector2 size = max - min;

        // Avoid division by zero in the shader.
        size.x = Mathf.Max(size.x, 0.000001f);
        size.y = Mathf.Max(size.y, 0.000001f);

        ApplyUVBounds(min, size);
    }

    private void ApplyUVBounds(Vector2 min, Vector2 size)
    {
        uvMin = min;
        uvSize = size;

        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        targetRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetVector(
            AutoUVMinID,
            new Vector4(uvMin.x, uvMin.y, 0.0f, 0.0f)
        );

        propertyBlock.SetVector(
            AutoUVSizeID,
            new Vector4(uvSize.x, uvSize.y, 0.0f, 0.0f)
        );

        targetRenderer.SetPropertyBlock(propertyBlock);
    }

    private Mesh GetMesh()
    {
        if (TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
            return meshFilter.sharedMesh;

        if (targetRenderer is SkinnedMeshRenderer skinnedMeshRenderer)
            return skinnedMeshRenderer.sharedMesh;

        return null;
    }

    private static Vector2[] GetUVs(Mesh mesh, int channel)
    {
        switch (channel)
        {
            case 0:
                return mesh.uv;

            case 1:
                return mesh.uv2;

            case 2:
                return mesh.uv3;

            case 3:
                return mesh.uv4;

            default:
                return mesh.uv;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Intentionally empty.
        // Keeping this method makes it easy to add UV-bound visualization later.
    }
#endif
}
