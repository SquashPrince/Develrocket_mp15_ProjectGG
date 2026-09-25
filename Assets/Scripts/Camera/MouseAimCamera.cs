using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MouseAimCamera : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("마우스가 플레이어 위에 있을 때의 카메라 위치")]
    [SerializeField] private Vector2 baseOffset = Vector2.zero;

    [Header("마우스 방향으로 이동할 최대 거리 (월드 단위)")]
    [SerializeField, Min(0f)] private float maxLookAhead = 3f;

    [Header("카메라가 목표 위치를 따라가는 시간")]
    [SerializeField, Min(0.01f)] private float smoothTime = 0.2f;

    private Camera cam;
    private Vector3 velocity;
    private float cameraY;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cameraY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 mouseViewport = cam.ScreenToViewportPoint(Input.mousePosition);
        Vector3 playerViewport = cam.WorldToViewportPoint(player.position);

        Vector2 mouseDirection = new Vector2(
            (mouseViewport.x - playerViewport.x) * 2f,
            (mouseViewport.y - playerViewport.y) * 2f
        );

        mouseDirection = Vector2.ClampMagnitude(mouseDirection, 1f);

        Vector3 targetPosition = new Vector3(
            player.position.x + baseOffset.x + mouseDirection.x * maxLookAhead,
            cameraY,
            player.position.z + baseOffset.y + mouseDirection.y * maxLookAhead
        ) ;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}