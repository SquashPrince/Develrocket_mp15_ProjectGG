using UnityEngine;

public class ActiveItem : Item, IActivable
{
    public PlayerItemEnum targetSlot;
    [SerializeField, Min(0f)] private float _cooldown = 1f;
    public float Cooldown => Mathf.Max(0f, _cooldown);

    [System.Serializable]
    private class EffectData
    {
        public ItemEffect effect;
        public float amount;
        [Min(0f)] public float time;
    }

    [SerializeField] private EffectData[] _effects = new EffectData[0];
    [SerializeField] private GameObject _model;

    public override void Interact(IInteractor owner)
    {
        if (owner == null || owner.Transform == null) return;
        if (!CanInteract) return;

        if (!owner.TrySetItem(this, targetSlot)) return;

        transform.SetParent(owner.Transform);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        transform.localScale = Vector3.zero;

        CanInteract = false;
        foreach (var collider in GetComponentsInChildren<Collider>()) collider.enabled = false;

        // Destroy(gameObject);
    }

    public override void Use(IInteractor owner)
    {
        if (owner == null) return;
        var before = new ItemStatSnapshot(owner);
        foreach (EffectData data in _effects)
        {
            if (data == null || data.effect == null) continue;
            data.effect.Apply(owner, data.amount, data.time);
        }

        before.LogChanges(owner, Name, this);
        Destroy(gameObject);
    }

    private void SetOffGameObject()
    {
        if (_model == null) return;

        _model.SetActive(false);
    }
}
