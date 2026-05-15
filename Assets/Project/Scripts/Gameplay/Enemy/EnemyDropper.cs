namespace LastSurvivor
{
    using UnityEngine;
    using R3;

    public class EnemyDroper : MonoBehaviour
    {
        [Header("回復アイテム"), SerializeField]
        private GameObject _healItemPrefab;

        [Header("弾薬アイテム"), SerializeField]
        private GameObject _ammoItemPrefab;

        [Header("ドロップ率"), SerializeField]
        private float _dropRate = 0.5f;

        [Header("ドロップ位置のオフセット"), SerializeField]
        private Vector3 _dropOffset = new Vector3(0, 0.5f, 0);

        [Header("敵のステータス"), SerializeField]
        private EnemyStatus _enemyStatus;

        private void Start()
        {
            _enemyStatus.IsDead
                 .Where(isDead => isDead)
                 .Take(1)
                 .Subscribe(_ => TryDrop())
                 .AddTo(this);
        }

        private void TryDrop()
        {
            if (Random.value > _dropRate)
            {
                return;
            }

            var prefab = Random.Range(0, 2) == 0 ? _healItemPrefab : _ammoItemPrefab;

            var spawnPos = transform.position + _dropOffset;
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}
