using UnityEngine;

public class HandROM : MonoBehaviour
{
    // 子オブジェクトのTransformをSerializeFieldで参照
    [SerializeField] private Transform childObject;

    // 親オブジェクトのSphereColliderをSerializeFieldで参照
    [SerializeField] private SphereCollider parentCollider;

    void Start()
    {
        // 親オブジェクトのSphereColliderが設定されているかチェック
        if (parentCollider == null)
        {
            Debug.LogError("親オブジェクトのSphereColliderが設定されていません。");
        }
    }

    void Update()
    {
        // 子オブジェクトと親オブジェクトのSphereColliderが設定されている場合
        if (childObject != null && parentCollider != null)
        {
            // 子オブジェクトの位置を親オブジェクトのSphereCollider内に制限する
            Vector3 clampedPosition = ClampPosition(childObject.position);
            childObject.position = clampedPosition;
        }
    }

    // 子オブジェクトの位置を親オブジェクトのSphereCollider内に制限するメソッド
    Vector3 ClampPosition(Vector3 position)
    {
        // 親オブジェクトのSphereColliderの中心と半径を取得
        Vector3 center = parentCollider.transform.position + parentCollider.center;
        float radius = parentCollider.radius;

        // 子オブジェクトの位置と親オブジェクトの中心の距離を計算
        Vector3 direction = position - center;
        if (direction.magnitude > radius)
        {
            // 距離が半径を超える場合、半径の範囲内にクランプ
            direction = direction.normalized * radius;
            position = center + direction;
        }

        return position;
    }
}
