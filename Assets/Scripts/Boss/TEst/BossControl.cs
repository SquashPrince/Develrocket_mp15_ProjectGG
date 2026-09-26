using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BossControl : MonoBehaviour
{
    private Queue<Func<IEnumerator>> _patternQueue = new Queue<Func<IEnumerator>>();
    private Queue<int> _timeQueue = new Queue<int>();
    
    [SerializeField] private float _sequenceDelay = 0.1f;

    private void Start()
    {
        StartCoroutine(UsePattern());
    }

    // 패턴을 Queue에 추가
    public void AddPattern(Func<IEnumerator> pattern)
    {
        _patternQueue.Enqueue(pattern);
    }
    public void AddPattern(int value)
    {
        _timeQueue.Enqueue(value);
    }

    
    private int ran;

    // Queue에 있는 패턴 실행
    private IEnumerator UsePattern()
    {
        while (true)
        {
            if (_patternQueue.Count > 0)
            {
                Debug.Log($"[Dequeue 전] 현재 Queue 개수 : {_patternQueue.Count}");
                Func<IEnumerator> pattern = _patternQueue.Dequeue();
                int value = _timeQueue.Dequeue();
                //Debug.Log($"[Dequeue 후] 패턴 꺼냄 / 현재 Queue 개수 : {_patternQueue.Count}");
                //Debug.Log($"꺼낸 pattern null 여부 : {pattern == null}");
                // 현재 패턴이 끝날 때까지 기다림
                
                // value(반복 가능한 횟수) 최대를 5라고 가정
                //Debug.Log($"value : {value}");
                // 1, 2, 3, 4, 5 까지 나옴
                ran = UnityEngine.Random.Range(1, value + 1); 
                //Debug.Log($"ran :  {ran}");


                if (ran < 2)
                {
                    yield return StartCoroutine(pattern());    
                }
                else
                {
                    for (int i = 0; i < ran; i++)
                    {
                        yield return StartCoroutine(pattern());
                        yield return new WaitForSeconds(_sequenceDelay);
                    }
                }

                yield return new WaitForSeconds(1f);

                
                
                //yield return StartCoroutine(pattern);
                //Debug.Log(value);
                //Debug.Log($"[패턴 종료] 현재 Queue 개수 : {_patternQueue.Count}");

                // 다음 패턴 실행까지 대기
                //yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return null;
            }
        }
    }

    public void StopPattern()
    {
        StopAllCoroutines();
        _patternQueue.Clear();
        _timeQueue.Clear();
    }
}
