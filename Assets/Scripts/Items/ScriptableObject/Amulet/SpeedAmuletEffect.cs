using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AmuletEffect/Speed Amulet")]
public class SpeedAmuletEffect : AmuletEffect
{
    [SerializeField] private float _amount;

    public override void Apply(IInteracter player)
    {
        // 플레이어 이동 속도 증가
        Debug.Log($"이동속도 {_amount} 증가");
    }
}
