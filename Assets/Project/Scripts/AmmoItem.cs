namespace LastSurvivor
{
    using UnityEngine;

    /// <summary>
    /// 弾薬アイテムを表すクラス
    /// </summary>
    public class AmmoItem : BaseItem
    {
        [Header("補充する予備弾薬数"), SerializeField]
        private int _ammoCount = 30;

        [Header("弾薬補充アイテムの効果音"), SerializeField]
        private AudioClip _ammoSound;

        private AudioSource _audioSource;

        protected override void OnCollect(GameObject collector)
        {
            if(_audioSource == null)
            {
                _audioSource = collector.GetComponent<AudioSource>();
            }

            var inventory = collector.GetComponent<WeaponInventory>();
            inventory.AddReserveAmmo(_ammoCount);

            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(_ammoSound);
            }
        }
    }
}
