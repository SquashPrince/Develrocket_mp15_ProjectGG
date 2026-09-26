using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MouseAimCamera : MonoBehaviour
{
    private Transform _player;

    [Header("마우스가 플레이어 위에 있을 때의 카메라 위치")]
    [SerializeField] private Vector2 _baseOffset = Vector2.zero;

    [Header("마우스 방향으로 이동할 최대 거리 (월드 단위)")]
    [SerializeField, Min(0f)] private float _maxLookAhead = 3f;

    [Header("카메라가 목표 위치를 따라가는 시간")]
    [SerializeField, Min(0.01f)] private float _smoothTime = 0.2f;

    private Camera cam;
    private Vector3 velocity;
    private float cameraY;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cameraY = transform.position.y;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.PlayerTransform != null);

        _player = GameManager.Instance.PlayerTransform;
    }

    private void LateUpdate()
    {
        if (_player == null) return;

        Vector3 mouseViewport = cam.ScreenToViewportPoint(Input.mousePosition);
        Vector3 playerViewport = cam.WorldToViewportPoint(_player.position);

        Vector2 mouseDirection = new Vector2(
            (mouseViewport.x - playerViewport.x) * 2f,
            (mouseViewport.y - playerViewport.y) * 2f
        );

        mouseDirection = Vector2.ClampMagnitude(mouseDirection, 1f);

        Vector3 targetPosition = new Vector3(
            _player.position.x + _baseOffset.x + mouseDirection.x * _maxLookAhead,
            cameraY,
            _player.position.z + _baseOffset.y + mouseDirection.y * _maxLookAhead
        ) ;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, _smoothTime);
    }
}