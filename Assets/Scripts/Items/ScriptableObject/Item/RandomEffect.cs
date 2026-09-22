using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/itemEffect/Random Item")]
public class RandomEffect : ItemEffect
{
    [SerializeField] private ItemEffect[] _randomEffects;
    public override void Apply(IInteractor interacter, float amount, float time)
    {
        // 등록된 이벤트중 랜덤 하게 발동
        int rand = Random.Range(0, _randomEffects.Length);

        _randomEffects[rand].Apply(interacter, amount, time);
    }
}
