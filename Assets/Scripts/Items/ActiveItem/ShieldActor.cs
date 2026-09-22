using UnityEngine;

public class ShieldActor : MonoBehaviour, IDamagable
{
    public GameObject GameObject => gameObject;

    public void Initialize(IInteractor owner, float radius)
    {
        // TODO: 무효화 효과 구현. 전용 프리팹의 콜라이더와 소유자 설정.
        Debug.Log("무효화 효과 구현 필요: 쉴드 초기화 요청");
    }

    public void TakeDamage(int damage)
    {
        // TODO: 피해 1회를 막고 충돌 중지, 피격 무적 발동, 쉴드 제거 순서로 구현.
        Debug.Log($"무효화 효과 구현 필요: 쉴드 피해 요청 {damage}");
    }
}
