using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleManager : MonoBehaviour
{
    [SerializeField] List<ParticlesPlayer> particles = new List<ParticlesPlayer>();

    /// <summary>
    /// 攻撃吸収時のパーティクルを再生
    /// </summary>
    /// <param name="attackPosition">吸収した攻撃の位置</param>
    public void PlayParticle(Vector3 attackPosition)
    {
        ParticlesPlayer playParticle = new ParticlesPlayer();
        float? minDistance = null;

        // パーティクルを再生する箇所を指定（吸収箇所から一番近い手足で再生されるように）
        foreach (ParticlesPlayer particle in particles)
        {
            float distance = Vector3.Distance(attackPosition, particle.transform.position);// 距離を取得
            // 調べた中で一番近い距離だった場合
            if (minDistance == null || minDistance > distance)
            {
                minDistance = distance;// 最短距離を更新
                playParticle = particle;// 再生するパーティクルの箇所を指定
            }
        }

        playParticle.Play();// パーティクルを再生
    }
}
