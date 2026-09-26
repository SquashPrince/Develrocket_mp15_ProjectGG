using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BulletBarrier : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyBulletMask;
    [SerializeField, Min(0f)] private float _initialSize = 10f;
    [SerializeField, Min(0f)] private float _finalSize = 20f;
    [SerializeField, Min(0f)] private float _growDuration = 1f;
    private static readonly HashSet<BulletBarrier> ActiveBarriers = new HashSet<BulletBarrier>();
    private readonly List<GunBullet> _pooledBullets = new List<GunBullet>();
    private readonly List<Bullet> _legacyBullets = new List<Bullet>();
    private SphereCollider _sphere;
    private float _elapsed;
    private Vector3 Center => transform.TransformPoint(_sphere.center);
    private float Radius => _sphere.radius * Mathf.Abs(transform.lossyScale.x);

    private void Awake()
    {
        _sphere = GetComponent<SphereCollider>();
        if (_enemyBulletMask.value == 0) _enemyBulletMask = LayerMask.GetMask("EnemyBullet");
        _sphere.isTrigger = true;
        _sphere.enabled = false;
    }

    private void OnEnable()
    {
        _elapsed = 0f;
        transform.localScale = Vector3.one * _initialSize;
        ActiveBarriers.Add(this);
        ClearInside();
    }

    private void OnDisable() => ActiveBarriers.Remove(this);

    private void Update()
    {
        if (Time.timeScale <= 0f) return;
        _elapsed += Time.deltaTime;
        float progress = _growDuration > 0f ? Mathf.Clamp01(_elapsed / _growDuration) : 1f;
        transform.localScale = Vector3.one * Mathf.Lerp(_initialSize, _finalSize, progress);
        ClearInside();
        if (progress >= 1f)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void ClearInside()
    {
        // 콜라이더 유무와 무관하게 탄환을 검사한다. 풀 반환 중 목록 변경에 대비해 복사한다.
        _pooledBullets.Clear();
        _pooledBullets.AddRange(GunBullet.ActiveBullets);
        foreach (var bullet in _pooledBullets)
        {
            if (bullet != null && bullet.isActiveAndEnabled && IsEnemyBulletLayer(bullet.gameObject.layer)
                && (bullet.transform.position - Center).sqrMagnitude <= Radius * Radius)
                bullet.ReturnToPool();
        }
        _legacyBullets.Clear();
        _legacyBullets.AddRange(Bullet.ActiveBullets);
        foreach (var bullet in _legacyBullets)
        {
            if (bullet == null || !bullet.isActiveAndEnabled || !IsEnemyBulletLayer(bullet.gameObject.layer)
                || (bullet.transform.position - Center).sqrMagnitude > Radius * Radius) continue;
            bullet.gameObject.SetActive(false);
            Destroy(bullet.gameObject);
        }
    }

    // 빠른 탄환이 한 프레임에 범위를 통과해도 이동 구간으로 판정한다.
    private bool IsEnemyBulletLayer(int layer) => (_enemyBulletMask.value & (1 << layer)) != 0;

    public static bool BlocksSegment(Vector3 start, Vector3 end, int bulletLayer, float bulletRadius = 0f)
    {
        foreach (var barrier in ActiveBarriers)
        {
            if (barrier == null || !barrier.isActiveAndEnabled || !barrier.IsEnemyBulletLayer(bulletLayer)) continue;
            Vector3 segment = end - start;
            float t = segment.sqrMagnitude > 0f
                ? Mathf.Clamp01(Vector3.Dot(barrier.Center - start, segment) / segment.sqrMagnitude) : 0f;
            float radius = barrier.Radius + Mathf.Max(0f, bulletRadius);
            if ((start + segment * t - barrier.Center).sqrMagnitude <= radius * radius) return true;
        }
        return false;
    }
}

