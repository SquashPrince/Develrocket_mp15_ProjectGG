using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicReviewTest : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private Transform _tr;

    private void Update()
    {
        _tr.Rotate(0f,_rotateSpeed * Time.deltaTime, 0f, Space.Self);
    }
}
