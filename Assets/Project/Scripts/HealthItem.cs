namespace LastSurvivor
{
    using UnityEngine;

    public class HealthItem : BaseItem
    {
        [Header("回復量"), SerializeField]
        private int _healAmount = 30;

        protected override void OnCollect(GameObject collector)
        {
            var presenter = collector.GetComponent<PlayerHealthPresenter>();
            presenter.Heal(_healAmount);
        }
    }
}
