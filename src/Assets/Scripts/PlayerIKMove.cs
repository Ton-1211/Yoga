using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]// Animatorコンポーネントが必要
public class PlayerIKMove : MonoBehaviour
{
    [Header("右手IK"), SerializeField] Transform rightIKTarget;
    [Header("左手IK"),SerializeField] Transform leftIKTarget;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (rightIKTarget == null) return;
        if (leftIKTarget == null) return;

        /* IKを有効化 */
        // 右手
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        // 左手
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        /* IKのターゲット設定 */
        // 右手
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightIKTarget.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightIKTarget.rotation);
        // 左手
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftIKTarget.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftIKTarget.rotation);
    }
}
