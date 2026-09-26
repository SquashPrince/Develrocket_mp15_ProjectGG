using UnityEngine;

public class PassiveItem : Item, IPassivable
{
    [SerializeField] private float _amount;
    [SerializeField] private GameObject _model;
    [SerializeField] private AmuletEffect[] _effects;

    public override void Interact(IInteractor owner)
    {
        if (!CanInteract || owner == null || owner.Transform == null) return;
        var before = new ItemStatSnapshot(owner);
        CanInteract = false;
        transform.SetParent(owner.Transform, false);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        foreach (AmuletEffect effect in _effects)
        {
            if (effect != null) effect.Apply(owner, _amount);
        }

        before.LogChanges(owner, Name, this);
        SetOffGameObject();

        // TODO: PlayerBehavior에서 획득했거나 상호작용할 수 없는 아이템을 대상 목록에서 제거.
    }

    private void SetOffGameObject()
    {
        if (_model == null) return;
        GetComponent<Collider>().enabled = false;
        _model.SetActive(false);
    }
}
