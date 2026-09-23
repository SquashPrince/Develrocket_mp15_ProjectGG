using System;
using System.Collections;
using UnityEngine;

public class BuffActor : MonoBehaviour
{
    private Action _removeBuff;

    public void StartBuff(Action applyBuff, Action removeBuff, float time)
    {
        StopAllCoroutines();
        RemoveBuff();
        if (!isActiveAndEnabled) return;
        _removeBuff = removeBuff;
        applyBuff();
        StartCoroutine(GetBuffRoutine(time));
    }

    private IEnumerator GetBuffRoutine(float time)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, time));
        RemoveBuff();
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        RemoveBuff();
        Destroy(gameObject);
    }

    private void RemoveBuff()
    {
        Action remove = _removeBuff;
        _removeBuff = null;
        remove?.Invoke();
    }
}
