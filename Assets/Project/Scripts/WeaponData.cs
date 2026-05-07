namespace LastSurvivor
{
    using UnityEngine;

    /// <summary>
    /// 武器の情報を格納するScriptableObjectを作成する
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/Weapon Data")]

    public class WeaponData : ScriptableObject
    {
        public string WeaponName;
        public Sprite Icon;
        public int MaxAmmo;
        public int CurrentAmmo;
        public int MaxReserveAmmo;
        public int ReserveAmmo;
        public float ReloadTime;
        public bool IsFullAuto;
    }
}
