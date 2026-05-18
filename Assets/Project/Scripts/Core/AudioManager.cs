namespace LastSurvivor
{
    using UnityEngine;

    public class AudioManager : MonoBehaviour
    {
        public AudioClip ShootSE;
        public AudioClip ReloadSE;
        public AudioClip EnemyScreamSE;

        private AudioSource _seSource;

        private void Awake()
        {
            _seSource = GetComponent<AudioSource>();
        }

        public void PlaySE(AudioClip clip)
        {
            if(clip != null)
            {
                _seSource.PlayOneShot(clip);
            }
        }
    }
}
