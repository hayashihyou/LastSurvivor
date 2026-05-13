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

        protected override void OnCollect(GameObject collector)
        {
            var inventory = collector.GetComponent<WeaponInventory>();
            inventory.AddReserveAmmo(_ammoCount);
        }
    }
}
