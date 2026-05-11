namespace LastSurvivor
{
    using UnityEngine;

    /// <summary>
    /// 弾の衝突を管理するクラス
    /// </summary>
    public class BulletCollision : MonoBehaviour
    {
        [Header("ダメージ"), SerializeField]
        private int _damage = 10;

        /// <summary>
        /// 弾が敵に衝突したときの処理
        /// </summary>
        /// <param name="other">衝突したオブジェクトのコライダー</param>
        private void OnTriggerEnter(Collider other)
        {
            // 衝突したオブジェクトが敵でない場合は、何もしない
            if (!other.CompareTag(TagConsts.Enemy))
            {
                return;
            }

            // 衝突したオブジェクトが敵であれば、ダメージを与える
            if (other.TryGetComponent<EnemyStatus>(out var enemyStatus))
            {
                enemyStatus.TakeDamageTask(_damage);
            }

            Destroy(gameObject);
        }
    }
}
