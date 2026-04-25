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

        [Header("CloneのBulletを格納する親オブジェクト"),SerializeField]
        private GameObject _bullets;

        [Header("マズルフラッシュのエフェクト"),SerializeField]
        private ParticleSystem _muzzleFlash;

        [Header("発射位置"),SerializeField]
        private Transform _firePoint;

        [Header("弾の速度"),SerializeField]
        private float _bulletSpeed = 50f;

        [Header("弾の寿命"),SerializeField]
        private float _bulletLifetime = 5f;
        /// <summary>
        /// 弾を発射するタスク
        /// </summary>
        public void Shoot()
        {
            // マズルフラッシュエフェクトを再生
            _muzzleFlash.Play(); 

            // 弾のプレハブをインスタンス化して発射位置と回転を設定
            var bullet = Instantiate(BulletPrefab, _firePoint.position, _firePoint.rotation, _bullets.transform);

            // 弾のRigidbodyコンポーネントを取得して、前方に速度を与える
            var rb = bullet.GetComponent<Rigidbody>();

            if(rb != null)
            {
                // 弾を前方に発射
                rb.linearVelocity = _firePoint.forward * _bulletSpeed;
            }

            // 5秒後に弾を破壊してメモリを解放
            Destroy(bullet, _bulletLifetime); 
        }
    }
}
