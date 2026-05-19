namespace LastSurvivor
{
    using UnityEngine;

    public class HealthItem : BaseItem
    {
        [Header("回復量"), SerializeField]
        private int _healAmount = 30;

        [Header("回復アイテムの効果音"), SerializeField]
        private AudioClip _healSound;

        private AudioSource _audioSource;

        protected override void OnCollect(GameObject collector)
        {
            if(_audioSource == null)
            {
                _audioSource = collector.GetComponent<AudioSource>();
            }

            var presenter = collector.GetComponent<PlayerHealthPresenter>();
            presenter.Heal(_healAmount);

            if(_audioSource != null)
            {
                _audioSource.PlayOneShot(_healSound);
            }
        }
    }
}
