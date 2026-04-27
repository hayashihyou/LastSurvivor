namespace LastSurvivor
{
    using UnityEngine;

    /// <summary>
    /// 武器の情報を格納するScriptableObjectを作成する
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/Weapon Data")]

    /// <summary>
    /// 武器の名前、アイコン、弾薬数、フルオートかどうかなどの情報を格納するクラス
    /// </summary>
    public class WeaponData : ScriptableObject
    {
        public string WeaponName;
        public Sprite Icon;
        public int MaxAmmo;
        public int CurrentAmmo;
        public int ReserveAmmo;
        public bool IsFullAuto;
    }
}
