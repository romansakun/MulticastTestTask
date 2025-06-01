using System;
using GameLogic.Bootstrapper;
using Infrastructure.Pools;
using Infrastructure.Services.Yandex.Leaderboards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.UI.Leaderboards
{
    public class PlayerLine : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        [Inject] private ColorsSettings _colorsSettings;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _backImage;
        [SerializeField] private Image _rankImage;
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI scoreText;

        private IMemoryPool _memoryPool;

        public void UpdateEntries(LBPlayerData data)
        {
            var rank = Mathf.Clamp(data.rank, 1, int.MaxValue);
            _rankImage.color = _colorsSettings.LeaderboardRankColors[rank > 3 ? 4 : rank];
            rankText.text = rank.ToString();
            nameText.text = data.name;
            scoreText.text = data.score.ToString();
            _backImage.color = _colorsSettings.ElementColors[ElementColor.LeaderboardOtherPlayerBack];
        }

        public void UpdateEntries(LBCurrentPlayerData data)
        {
            _backImage.color = _colorsSettings.ElementColors[ElementColor.LeaderboardMyPlayerBack];
            if (data == null)
            {
                _rankImage.color = _colorsSettings.LeaderboardRankColors[3];
                rankText.text = "-";
                nameText.text = "-";
                scoreText.text = "-";
            }
            else
            {
                var rank = Mathf.Clamp(data.rank, 1, int.MaxValue);
                _rankImage.color = _colorsSettings.LeaderboardRankColors[rank > 3 ? 4 : rank];
                rankText.text = rank.ToString();
                nameText.text = data.name;
                scoreText.text = data.score.ToString();
            }
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
            _memoryPool = null;
        }

        public void OnSpawned(IMemoryPool memoryPool)
        {
            _memoryPool = memoryPool;
            _rectTransform.anchoredPosition = Vector2.zero;
            _rectTransform.anchorMin = Vector2.one * 0.5f;
            _rectTransform.anchorMax = Vector2.one * 0.5f;
            _rectTransform.pivot = Vector2.one * 0.5f;
            gameObject.SetActive(true);
        }

        public void Dispose()
        {
            _memoryPool.Despawn(this);
        }

        public class Factory : PlaceholderFactory<PlayerLine>
        {
            [Inject] private DeferredMonoPool<PlayerLine> _deferredMonoPool;

            public override PlayerLine Create()
            {
                return _deferredMonoPool.Spawn();
            }
        }
    }
}