namespace LastSurvivor
{
    using UnityEngine;

    /// <summary>
    /// アイテムはこのインターフェースを実装する必要がある。
    /// </summary>
    public interface ICollectable
    {
        /// <summary>
        /// アイテムを収集する
        /// </summary>
        /// <param name="collector">収集したゲームオブジェクト</param>
        bool Collect(GameObject collector);
    }
}
