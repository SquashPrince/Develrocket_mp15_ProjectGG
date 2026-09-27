using UnityEngine;

// GameScene의 UIManager 아래에만 배치하는 로컬 엔딩 패널.
public class CreditsPanel : MonoBehaviour
{
    private float _previousTimeScale = 1f;

    public void Show()
    {
        _previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }

    public void ReturnToLobby()
    {
        Time.timeScale = _previousTimeScale;
        gameObject.SetActive(false);
        GameManager.Instance.ReturnToLobby();
    }
}
