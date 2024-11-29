using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Animator))]// Animatorコンポーネントが必要
public class PlayerIKMove : MonoBehaviour
{
    [Header("右手IK"), SerializeField] Transform rightHandIkTarget;
    [Header("左手IK"), SerializeField] Transform leftHandIkTarget;
    [Header("右足IK"), SerializeField] Transform rightFootIkTarget;
    [Header("左足IK"), SerializeField] Transform leftFootIkTarget;
    [Header("頭"), SerializeField] Transform head;
    [Header("尻"), SerializeField] Transform hip;
    [Header("Mocopiアバター"), SerializeField] Animator mocopiAvatarAnimator;

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
        if (!CheckIKSet()) return;// IKのポイントが1つでも設定していなければ実行を辞める

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

        //Quaternion hipRotate = hip.rotation * Quaternion.Euler(new Vector3(0f, 0f, -90f));
        //animator.SetBoneLocalRotation(HumanBodyBones.Hips, hipRotate);
        //SyncBoneRotation(HumanBodyBones.Spine);
        //SyncBoneRotation(HumanBodyBones.Chest);
        //SyncBoneRotation(HumanBodyBones.UpperChest);
        //SyncBoneRotation(HumanBodyBones.Neck);
        //animator.SetBoneLocalRotation(HumanBodyBones.Head, head.rotation);
    }

    bool CheckIKSet()
    {
        if (rightHandIkTarget == null) return false;
        if (leftHandIkTarget == null) return false;
        if (rightFootIkTarget == null) return false;
        if (leftFootIkTarget == null) return false;

        return true;
    }

    void SyncBoneRotation(HumanBodyBones boneType)
    {
        //animator.SetBoneLocalRotation(boneType, mocopiAvatarAnimator.GetBoneTransform(boneType).localRotation/* * Quaternion.Euler(new Vector3(0f, 90f, 0f))*/);
    }
}
