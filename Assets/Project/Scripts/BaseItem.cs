namespace LastSurvivor
{
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using R3;
    using System.Threading;

    /// <summary>
    /// アイテムの基本クラス
    /// </summary>
    public abstract class BaseItem : MonoBehaviour, ICollectable
    {
        [Header("アイテム設定"), SerializeField]
        private string _itemName = "Item";

        [Header("プレイヤーのレイヤー"), SerializeField]
        private LayerMask _playerLayer;

        [Header("アイテムの回転"), SerializeField]
        private float _rotationSpeed = 90f;

        [Header("アイテムの浮遊振幅"), SerializeField]
        private float _floatHeight = 0.3f;

        [Header("アイテムの浮遊周期"), SerializeField]
        private float _floatSpeed = 1.5f;

        [Header("アイテムを拾えるようになるまでの時間"), SerializeField]
        private int _effectDurationMs = 600;

        // 既に収集されたかどうか確認するフラグ
        private bool _isCollected = false;

        // アイテムの初期位置を保存するための変数
        private float _initialY;

        // アイテムの回転と浮遊のための変数
        private CancellationTokenSource _cts;

        // アイテムが収集されたときのイベントを発行するためのSubject
        private readonly Subject<string> _onCollectedSubject = new Subject<string>();

        // アイテムが収集されたときに発行されるObservable
        public Observable<string> OnCollctedAsObservable => _onCollectedSubject;

        // NOTE: hayashi アイテム取得時のエフェクトを実装するまではコメントアウトにしておく
        //private ItemPickupEffect _pickupEffect;

        /// <summary>
        /// アイテムの初期化
        /// </summary>
        protected virtual void Awake()
        {
            _cts = new CancellationTokenSource();
            //_pickupEffect = GetComponent<ItemPickupEffect>();
            _initialY = transform.position.y;

            // アイテムのコライダーがトリガーになるように設定
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        protected virtual void Update()
        {
            if (_isCollected)
            {
                return;
            }

            AnimateFloat();
            AnimateRotate();
        }

        /// <summary>
        /// アイテムの破棄処理
        /// </summary>
        protected virtual void OnDestroy()
        {
            // UniTaskのキャンセル
            _cts.Cancel();
            _cts.Dispose();

            // Subjectの破棄
            _onCollectedSubject.Dispose();
        }

        /// <summary>
        /// アイテムを収集する処理
        /// </summary>
        /// <param name="collector">アイテムを収集するオブジェクト</param>
        public bool Collect(GameObject collector)
        {
            if (_isCollected)
            {
                return false;
            }

            _isCollected = true;

            CollectTask(collector, _cts.Token).Forget();
            return true;
        }

        /// <summary>
        /// アイテムを収集する非同期処理
        /// </summary>
        /// <param name="collector">アイテムを収集するオブジェクト</param>
        /// <param name="ct">キャンセルトークン</param>
        private async UniTaskVoid CollectTask(GameObject collector, CancellationToken ct)
        {
            SetVisiblity(false);

            // NOTE: hayashi アイテム取得時のエフェクトを実装するまではコメントアウトにしておく
            //if (_pickupEffect != null)
            //{
            //    await _pickupEffect.PlayAsync(ct);
            //}
            //else
            //{
            //    await UniTask.Delay(effectDurationMs, cancellationToken: ct);
            //}

            // キャンセルされていないか確認
            if (ct.IsCancellationRequested)
            {
                return;
            }

            // アイテムの収集処理を実行
            OnCollect(collector);

            // アイテムが収集されたことを通知
            _onCollectedSubject.OnNext(_itemName);

            // アイテムを破棄
            Destroy(gameObject);
        }

        /// <summary>
        /// アイテムが収集されたときの処理を実装する抽象メソッド
        /// </summary>
        /// <param name="collector">アイテムを収集するオブジェクト</param>
        protected abstract void OnCollect(GameObject collector);

        /// <summary>
        /// アイテムのトリガーに入ったときの処理
        /// </summary>
        /// <param name="other">トリガーに入ったオブジェクトのコライダー</param>
        private void OnTriggerEnter(Collider other)
        {
            // プレイヤー以外のオブジェクトがトリガーに入った場合は無視する
            if (((1 << other.gameObject.layer) & _playerLayer) == 0)
            {
                return;
            }

            // アイテムを収集する処理を実行
            Collect(other.gameObject);
        }

        /// <summary>
        /// アイテムの浮遊アニメーションを実装するメソッド
        /// </summary>
        private void AnimateFloat()
        {
            var pos = transform.position;
            pos.y = _initialY + Mathf.Sin(Time.time * _floatSpeed) * _floatHeight;
            transform.position = pos;
        }

        /// <summary>
        /// アイテムの回転アニメーションを実装するメソッド
        /// </summary>
        private void AnimateRotate()
        {
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.World);
        }

        /// <summary>
        /// アイテムの可視性を設定するメソッド
        /// </summary>
        /// <param name="isVisible"></param>
        private void SetVisiblity(bool isVisible)
        {
            // アイテムの全てのレンダラーの可視性を設定
            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = isVisible;
            }
        }

        /// <summary>
        /// アイテムの名前を取得するメソッド
        /// </summary>
        /// <returns></returns>
        public string GetItemName() => _itemName;
    }
}
