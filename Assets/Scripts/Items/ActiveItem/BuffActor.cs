using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffActor : MonoBehaviour
{
    public void StartBuff(PlayerContoller player, float increaseDmgValue, float increaseSpdValue, float time)
    {
        StartCoroutine(GetBuffRoutine(player, increaseDmgValue, increaseSpdValue, time));
    }
    private IEnumerator GetBuffRoutine(PlayerContoller player, float increaseDmgValue, float increaseSpdValue, float time)
    {
        if (increaseDmgValue != 0)
            Debug.Log($"{player.gameObject.name}의 공격력을 {increaseDmgValue} 만큼 증가");
        if (increaseSpdValue != 0)
            Debug.Log($"{player.gameObject.name}의 이동속도를 {increaseSpdValue} 만큼 증가");

        yield return new WaitForSeconds(time);

        if (increaseDmgValue != 0)
            Debug.Log($"{player.gameObject.name}의 공격력 원상 복귀");
        if (increaseSpdValue != 0)
            Debug.Log($"{player.gameObject.name}의 이동속도 원상 복귀");

        Destroy(gameObject);
    }
}
