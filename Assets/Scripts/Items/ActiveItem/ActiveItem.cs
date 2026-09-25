using UnityEngine;

public class ActiveItem : Item, IActivable
{
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

        CanInteract = false;

        foreach (EffectData data in _effects)
        {
            if (data == null || data.effect == null) continue;
            data.effect.Apply(owner, data.amount, data.time);
        }

        Destroy(gameObject);
    }

    private void SetOffGameObject()
    {
        if (_model == null) return;

        _model.SetActive(false);
    }
}
