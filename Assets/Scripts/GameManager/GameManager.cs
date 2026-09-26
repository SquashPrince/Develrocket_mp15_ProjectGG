using Player;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    private PlayerValues _playerValues;
    public PlayerValues PlayerValues => _playerValues;
    public Transform PlayerTransform => _playerValues != null ? _playerValues.transform : null;
    public bool IsPaused { get; private set; }

    private float _timeScaleBeforePause = 1f;

    private void Start()
    {
        FindScenePlayer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetPaused(!IsPaused);
    }

    public void RegisterPlayer(PlayerValues playerValues)
    {
        if (playerValues != null) _playerValues = playerValues;
    }

    public void SetPaused(bool paused)
    {
        if (Instance != this || IsPaused == paused) return;

        if (paused)
        {
            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = _timeScaleBeforePause;
        }

        IsPaused = paused;
        Debug.Log(paused ? "[GameManager] 일시정지" : "[GameManager] 일시정지 해제");
    }

    private void FindScenePlayer()
    {
        if (_playerValues == null)
            RegisterPlayer(FindFirstObjectByType<PlayerValues>());
    }
}
