using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AmuletEffect/Gold Amulet")]
public class GoldAmuletEffect1 : AmuletEffect
{
    [SerializeField] private int _amount;

    public override void Apply(IInteracter player)
    {
        // «√∑π¿ÃæÓ ∞ÒµÂ »πµÊ∑Æ ¡ı∞°
        Debug.Log($"∞ÒµÂ »πµÊ«‚ {_amount} ∏∏≈≠ ¡ı∞°");
    }
}
