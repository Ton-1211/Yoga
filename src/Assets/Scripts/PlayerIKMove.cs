using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]// Animatorコンポーネントが必要
public class PlayerIKMove : MonoBehaviour
{
    [Header("右手IK"), SerializeField] Transform rightHandIkTarget;
    [Header("左手IK"),SerializeField] Transform leftHandIkTarget;
    [Header("右足IK"), SerializeField] Transform rightFootIkTarget;
    [Header("左足IK"), SerializeField] Transform leftFootIkTarget;

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
        if (rightHandIkTarget == null) return;
        if (leftHandIkTarget == null) return;
        if(rightFootIkTarget == null) return;
        if(leftFootIkTarget == null) return;

        /* IKを有効化 */
        // 右手
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        // 左手
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        // 右足
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);
        // 左足
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);

        /* IKのターゲット設定 */
        // 右手
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIkTarget.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIkTarget.rotation);
        // 左手
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIkTarget.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIkTarget.rotation);
        // 右足
        animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootIkTarget.position);
        animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootIkTarget.rotation);
        // 左足
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootIkTarget.position);
        animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootIkTarget.rotation);
    }
}
