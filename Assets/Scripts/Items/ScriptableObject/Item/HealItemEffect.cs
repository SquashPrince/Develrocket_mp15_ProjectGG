using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/itemEffect/Heal Item")]
public class HealItemEffect : ItemEffect
{
    [SerializeField] private GameObject _model;

    public override void Apply(IInteractor interacter, float amount, float time)
    {
        interacter.Hp += Mathf.RoundToInt(amount);
    }
}
