namespace LastSurvivor
{
    using UnityEngine;

    /// <summary>
    /// 弾の発射を制御するクラス
    /// </summary>
    public class BulletController : MonoBehaviour
    {
        [Header("設定"),SerializeField]
        public GameObject BulletPrefab;

        // 弾の発射位置
        public Transform FirePoint;

        // 弾の速度
        public float BulletSpeed = 50f;

        /// <summary>
        /// 弾を発射するタスク
        /// </summary>
        public void ShootTask()
        {
            // 弾のプレハブをインスタンス化して発射位置と回転を設定
            GameObject bullet = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);

            // 弾のRigidbodyコンポーネントを取得して、前方に速度を与える
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if(rb != null)
            {
                // 弾を前方に発射
                rb.linearVelocity = FirePoint.forward * BulletSpeed;
            }

            // 5秒後に弾を破壊してメモリを解放
            Destroy(bullet, 5f); 
        }
    }
}
