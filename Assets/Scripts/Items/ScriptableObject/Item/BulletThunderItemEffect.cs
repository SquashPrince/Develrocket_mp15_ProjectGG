using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/itemEffect/Barrier Thunder Item")]
public class BulletThunderItemEffect : ItemEffect
{
    [SerializeField] private BulletBarrier _bulletBarrier;

    public override void Apply(IInteractor interacter, float amount, float time)
    {
        if (interacter == null || interacter.Transform == null) return;
        if (_bulletBarrier == null)
        {
            Debug.LogError("BulletThunderItemEffect: BulletBarrier 프리팹이 필요합니다.");
            return;
        }
        // 부모를 지정하지 않아 사용 위치에 남고 플레이어의 Scale에 영향을 받지 않는다.
        Instantiate(_bulletBarrier, interacter.Transform.position, Quaternion.identity);
    }
}
