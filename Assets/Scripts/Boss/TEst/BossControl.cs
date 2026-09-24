using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControl : MonoBehaviour
{
    private Queue<IEnumerator> _patternQueue = new Queue<IEnumerator>();

    private void Start()
    {
        StartCoroutine(UsePattern());
    }

    // 패턴을 Queue에 추가
    public void AddPattern(IEnumerator pattern)
    {
        //Debug.Log($"넣으려는 pattern null 여부 : {pattern == null}");
        _patternQueue.Enqueue(pattern);
        //Debug.Log($"[Enqueue] 패턴 추가 / 현재 Queue 개수 : {_patternQueue.Count}");

    }

    // Queue에 있는 패턴 실행
    private IEnumerator UsePattern()
    {
        while (true)
        {
            if (_patternQueue.Count > 0)
            {
                //Debug.Log($"[Dequeue 전] 현재 Queue 개수 : {_patternQueue.Count}");
                IEnumerator pattern = _patternQueue.Dequeue();
                //Debug.Log($"[Dequeue 후] 패턴 꺼냄 / 현재 Queue 개수 : {_patternQueue.Count}");
                //Debug.Log($"꺼낸 pattern null 여부 : {pattern == null}");
                // 현재 패턴이 끝날 때까지 기다림
                yield return StartCoroutine(pattern);
                //Debug.Log($"[패턴 종료] 현재 Queue 개수 : {_patternQueue.Count}");

                // 다음 패턴 실행까지 대기
                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return null;
            }
        }
    }
}
