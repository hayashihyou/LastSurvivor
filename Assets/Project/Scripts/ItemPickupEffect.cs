namespace LastSurvivor
{
    using Cysharp.Threading.Tasks;
    using System.Threading;
    using UnityEngine;

    /// <summary>
    /// アイテムを拾ったときのエフェクトを管理するクラス
    /// </summary>
    public class ItemPickupEffect : MonoBehaviour
    {
        [Header("アイテムを拾った時に再生するパーティクル"),SerializeField]
        private ParticleSystem _pickupParticle;

        [Header("パーティクルのオフセット"), SerializeField]
        private Vector3 _particleOffset = Vector3.zero;

        [Header("アイテムを拾った時に再生するサウンド"), SerializeField]
        private AudioClip _pickupSound;

        [Header("SEのボリューム"), SerializeField]
        [Range(0f, 1f)]
        private float _volume = 1f;

        [Header("SE、エフェクトを開始する前の待機時間"), SerializeField]
        private int _delay = 0;

        [Header("全演出終了後、呼び出し元に制御を返すまでの追加待機"), SerializeField]
        private int _postEffectDelay = 0;

        // NOTE:　hayashi　AudioSourceを実装したら以下のコメントアウトを解除して使用する。
        //private AudioSource _audioSource;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake()
        {
            // NOTE:　hayashi　AudioSourceを実装したら以下のコメントアウトを解除して使用する。
            //_audioSource = GetComponent<AudioSource>();

            //// PlayOnAwake を無効化（手動再生のみ）
            //_audioSource.playOnAwake = false;
        }

        /// <summary>
        /// アイテムを拾ったときのエフェクトを再生する。
        /// </summary>
        public async UniTask PlayAsync(CancellationToken ct)
        {
            if (_delay > 0)
                await UniTask.Delay(_delay, cancellationToken: ct);

            if (ct.IsCancellationRequested) return;

            ParticleSystem spawnedParticle = SpawnParticle();

            float seDuration = PlaySE();

            float particleDuration = GetParticleDuration(spawnedParticle);
            float waitSeconds = Mathf.Max(particleDuration, seDuration);

            if (waitSeconds > 0f)
                await UniTask.Delay(
                    Mathf.RoundToInt(waitSeconds * 1000f),
                    cancellationToken: ct
                );

            if (ct.IsCancellationRequested) return;

            if (_postEffectDelay > 0)
                await UniTask.Delay(_postEffectDelay, cancellationToken: ct);
        }

        /// <summary>
        /// パーティクルをスポーンして再生する。
        /// </summary>
        private ParticleSystem SpawnParticle()
        {
            if (_pickupParticle == null) return null;

            Vector3 spawnPos = transform.position + _particleOffset;
            ParticleSystem ps = Instantiate(_pickupParticle, spawnPos, Quaternion.identity);

            // 再生終了後に自動破棄
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax + 1f);

            ps.Play();
            return ps;
        }

        /// <summary>
        /// SEを再生する。
        /// </summary>
        /// <returns>SEの長さ（秒）。未設定なら 0f</returns>
        private float PlaySE()
        {
            if (_pickupSound == null) return 0f;

            // NOTE:　hayashi　AudioSourceを実装したら以下のコメントアウトを解除して使用する。
            //// PlayOneShot を使うことで連打されても重複再生に対応
            //_audioSource.PlayOneShot(_pickupSound, _volume);
            return _pickupSound.length;
        }

        /// <summary>
        /// パーティクルの再生時間を取得する。
        /// </summary>
        private float GetParticleDuration(ParticleSystem ps)
        {
            if (ps == null) return 0f;

            var main = ps.main;

            // ループ設定の場合は duration のみを再生時間とする
            if (main.loop) return main.duration;

            return main.duration + main.startLifetime.constantMax;
        }
    }
}
