using Player;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    private PlayerValues _playerValues;
    public PlayerValues PlayerValues => _playerValues;
    public Transform PlayerTransform => _playerValues != null ? _playerValues.transform : null;
    public bool IsPaused { get; private set; }

    public bool IsCleared { get; private set; }

    public bool CanPlay => !IsPaused && !IsCleared &&
        UIManager.Instance.Window.EWindow == EWindowType.Battle &&
        UIManager.Instance.Window.NextEWindow == EWindowType.Battle;

    private float _timeScaleBeforePause = 1f;

    private void Start()
    {
        FindScenePlayer();
    }

    private void Update()
    {
        if (IsCleared || UIManager.Instance.Window.EWindow != EWindowType.Battle ||
            !UIManager.Instance.Window.WindowCompare()) return;
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
        if (paused && IsCleared) return;

        if (paused)
        {
            UIManager.Instance.PopUp.Open(EPopUpType.BattleExit);
            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            UIManager.Instance.PopUp.Close(EPopUpType.BattleExit);
            Time.timeScale = _timeScaleBeforePause;
        }

        IsPaused = paused;
        Debug.Log(paused ? "[GameManager] 일시정지" : "[GameManager] 일시정지 해제");
    }

    public void CompleteGame()
    {
        if (IsCleared) return;
        CreditsPanel credits = UIManager.Instance.GetComponentInChildren<CreditsPanel>(true);
        if (credits == null)
        {
            Debug.LogWarning("[GameManager] CreditsPanel is not connected in this scene.");
            return;
        }
        SetPaused(false);
        IsCleared = true;
        credits.Show();
    }

    public void ReturnToLobby()
    {
        SetPaused(false);
        IsCleared = false;
        UIManager.Instance.Window.NextEWindow = EWindowType.Lobby;
        UIManager.Instance.Window.OpenLoading();
    }

    private void FindScenePlayer()
    {
        if (_playerValues == null)
            RegisterPlayer(FindFirstObjectByType<PlayerValues>());
    }
}
