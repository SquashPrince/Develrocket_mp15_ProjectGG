using UnityEngine;

public abstract class BuffItemEffct : ItemEffect
{
    [SerializeField] private BuffActor _buffActor;

    protected BuffActor CreateBuff(IInteractor interacter)
    {
        if (interacter == null || interacter.Transform == null) return null;

        BuffActor buff = Instantiate(_buffActor, interacter.Transform);
        buff.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        return buff;
    }
}
