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

        [Header("カメラ"),SerializeField]
        private Camera _camera;

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
        /// 弾を発射する
        /// </summary>
        public void Shoot()
        {
            // マズルフラッシュエフェクトを再生
            _muzzleFlash.Play(); 

            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            // 着弾点を決定（何も当たらなければ遠方の点）
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(1000f);
            }

            // 銃口から着弾点への方向を計算
            Vector3 direction = (targetPoint - _firePoint.position).normalized;

            // 弾を発射
            var bullet = Instantiate(BulletPrefab, _firePoint.position, Quaternion.LookRotation(direction), _bullets.transform);

            var rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * _bulletSpeed;
            }

            Destroy(bullet, _bulletLifetime);
        }
    }
}
