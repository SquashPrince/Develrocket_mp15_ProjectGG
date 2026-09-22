using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    private void Update() => Move();

    private void Move()
    {
        transform.Translate(GetInput() * _moveSpeed * Time.deltaTime);
    }

    private Vector3 GetInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        return new Vector3(x, 0, z).normalized;
    }

}
