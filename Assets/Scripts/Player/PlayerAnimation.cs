using System.Collections;
using UnityEngine;
using Player;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private string _moveX = "MoveX";
    [SerializeField] private string _moveZ = "MoveZ";
    [SerializeField] private string _dodge = "Base Layer.Dodge";
    [SerializeField] private string _reload = "Upper Body.Reload";

    private int _upperBodyLayer = -1;

    private int _moveXHash { get => Animator.StringToHash(_moveX); }
    private int _moveZHash { get => Animator.StringToHash(_moveZ); }
    private int _dodgeState { get => Animator.StringToHash(_dodge); }
    private int _reloadState { get => Animator.StringToHash(_reload); }

    private PlayerInputManager _playerInputMng;
    private PlayerValues _playerValues;
    private Vector3 _moveDirection;
    private bool _isDodging;
    private bool _isDodgeCooldown;

    private void Awake() => CacheComponent();

    private void OnEnable()
    {
        if (_playerInputMng == null) return;
        _playerInputMng.OnMove += OnMove;
        _playerInputMng.OnDodge += OnDodge;
    }

    private void OnDisable()
    {
        if (_playerInputMng != null)
        {
            _playerInputMng.OnMove -= OnMove;
            _playerInputMng.OnDodge -= OnDodge;
        }
        _moveDirection = Vector3.zero;
        if (_anim != null && _anim.runtimeAnimatorController != null && _upperBodyLayer >= 0)
            _anim.SetLayerWeight(_upperBodyLayer, 0f);
    }

    private void LateUpdate()
    {
        // PlayerBehavior.Update의 마우스 회전이 끝난 뒤 방향을 계산한다.
        if (_anim == null || _anim.runtimeAnimatorController == null) return;
        PlayerMoveAnimationUpdate();
        PlayerReloadAnimationUpdate();
    }

    private void PlayerReloadAnimationUpdate()
    {
        // 실행 중 Controller가 연결되어도 레이어를 찾을 수 있게 한다.
        _upperBodyLayer = _anim.GetLayerIndex("Upper Body");
        if (_upperBodyLayer < 0 || !_anim.HasState(_upperBodyLayer, _reloadState)) return;

        bool dodgeVisible = _isDodging || _anim.GetCurrentAnimatorStateInfo(0).fullPathHash == _dodgeState;
        if (_anim.IsInTransition(0))
            dodgeVisible |= _anim.GetNextAnimatorStateInfo(0).fullPathHash == _dodgeState;

        bool showReload = _playerValues != null && _playerValues.IsReloading && !dodgeVisible;
        _anim.SetLayerWeight(_upperBodyLayer, showReload ? 1f : 0f);
        if (!showReload) return;

        // 무기별 실제 장전 시간에 클립 진행률을 맞춘다.
        // 회피 후에도 처음부터 재시작하지 않고 현재 장전 진행률부터 표시한다.
        _anim.Play(_reloadState, _upperBodyLayer, _playerValues.ReloadProgress);
    }

    private void OnMove(Vector3 direction)
    {
        // 기존 이동 코드와 동일하게 회피 중에는 이동 방향을 유지한다.
        if (_isDodging) return;
        _moveDirection = direction;
    }

    private void PlayerMoveAnimationUpdate()
    {
        Vector3 localDirection = transform.InverseTransformDirection(_moveDirection);
        _anim.SetFloat(_moveXHash, localDirection.x);
        _anim.SetFloat(_moveZHash, localDirection.z);
    }

    private void OnDodge()
    {
        if (_isDodgeCooldown || _playerValues == null) return;
        StartCoroutine(DodgeCooldown());

        if (_anim == null || _anim.runtimeAnimatorController == null) return;
        if (!_anim.HasState(0, _dodgeState)) return;

        // 복귀와 재생 속도는 기존 Animator의 Dodge 상태 설정을 사용한다.
        _anim.CrossFadeInFixedTime(_dodgeState, 0.05f, 0, 0f);
    }

    private IEnumerator DodgeCooldown()
    {
        // PlayerBehavior가 회피 수락 여부를 공개하지 않으므로 같은 대기 순서를 사용한다.
        // 회피 규칙이 바뀌면 이 부분도 함께 맞춰야 한다.
        _isDodging = true;
        _isDodgeCooldown = true;
        yield return new WaitForSeconds(_playerValues.DodgeTime);
        _isDodging = false;
        yield return new WaitForSeconds(_playerValues.DodgeCoolDown - _playerValues.DodgeTime);
        _isDodgeCooldown = false;
    }

    private void CacheComponent()
    {
        _playerInputMng = GetComponent<PlayerInputManager>();
        _playerValues = GetComponent<PlayerValues>();
        if (_anim == null) _anim = GetComponentInChildren<Animator>();

        // 실제 위치 이동은 PlayerBehavior에서만 처리한다.
        if (_anim != null) _anim.applyRootMotion = false;
    }
}
