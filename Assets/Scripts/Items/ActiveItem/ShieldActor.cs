using UnityEngine;

// 선택적인 쉴드 표시 오브젝트. 실제 차단 상태는 소유자에게 있다.
public class ShieldActor : MonoBehaviour, IDamagable
{
    public GameObject GameObject => gameObject;
    private IInteractor _owner;
    public void Initialize(IInteractor owner, float radius)
    {
        _owner = owner;
        if (_owner == null) { Destroy(gameObject); return; }
        _owner.DamageShieldCharges = 1;
        if (TryGetComponent<SphereCollider>(out var sphere)) sphere.radius = Mathf.Max(0f, radius);
    }
    public void TakeDamage(int damage)
    {
        if (damage <= 0 || _owner == null || !_owner.TryConsumeDamageShield()) return;
        foreach (var collider in GetComponentsInChildren<Collider>()) collider.enabled = false;
        Destroy(gameObject);
    }
    private void Update()
    {
        if (_owner == null || _owner.Transform == null || _owner.DamageShieldCharges == 0)
            Destroy(gameObject);
    }
}
