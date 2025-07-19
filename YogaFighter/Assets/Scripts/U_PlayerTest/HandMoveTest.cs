using UnityEngine;
using UnityEngine.InputSystem;

public class HandMoveTest : MonoBehaviour
{
    // 2軸入力を受け取るAction
    [SerializeField] private InputActionProperty _moveAction;

    // 移動の速さ
    [SerializeField] private float _speed = 1;

    private void OnDestroy()
    {
        _moveAction.action.Dispose();
    }

    private void OnEnable()
    {
        _moveAction.action.Enable();
    }

    private void OnDisable()
    {
        _moveAction.action.Disable();
    }

    private void Update()
    {
        // 2軸入力読み込み
        var inputValue = _moveAction.action.ReadValue<Vector2>();

        // xy軸方向で移動
        transform.Translate(inputValue * (_speed * Time.deltaTime));
    }
}
