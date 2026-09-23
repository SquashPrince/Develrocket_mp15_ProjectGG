using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(x, 0, y).normalized;

        transform.Translate(direction * _speed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"데미지 입음 : {damage}");
    }
}
