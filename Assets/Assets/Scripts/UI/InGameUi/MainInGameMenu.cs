using FMODUnity;
using KendaAi.Scripts.Ui;
using KabreetGames.EventBus.Interfaces;
using KabreetGames.SceneReferences;
using KabreetGames.UiSystem;
using KendaAi.Events;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace KendaAi.TestProject.InGameUi
{
    public class MainInGameMenu : MenuPanel
    {
        [field: SerializeField, Self] public RectTransform RectTransform { get; private set; }

        [SerializeField, Required] private CollectIconView collectIconViewPrefab;
        private LinkedPool<CollectIconView> collectIconViewPool;
        [SerializeField, Child(Flag.Editable)] private RectTransform coinCollectTraget;

        private LinkedPool<CollectIconView> CollectIconViewPool => collectIconViewPool ??=
            new LinkedPool<CollectIconView>(CreateFunc, Get, Return, DestroyItem);
        
        private int coinCount;
        private int lifeCount;
        [SerializeField, Child(Flag.Editable)] private TextMeshProUGUI coinText;
        
        [SerializeField,Child(Flag.Editable)] private Image [] lifeImages;

        protected override void OnEnable()
        {
            base.OnEnable();
            EventBus<CoinCollectedEvent>.Register(CoinCollect);
            EventBus<HitEvent>.Register(Hit);
            coinCount = PlayerPrefs.GetInt("CoinCount", 0);
            lifeCount = lifeImages.Length;
            UpdateLifeView();
            UpdateCoinTextView();
        }

        private void Hit()
        {
            lifeCount--;
            RuntimeManager.PlayOneShot("event:/Hit", transform.position);
            if (lifeCount <= 0)
            {
                lifeCount = 0;
                EventBus<DeathEvent>.Rise();
            }
            UpdateLifeView();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            EventBus<CoinCollectedEvent>.Deregister(CoinCollect);
            EventBus<HitEvent>.Deregister(Hit);
            PlayerPrefs.SetInt("CoinCount", coinCount);
        }

        private static void Get(CollectIconView obj)
        {
            obj.gameObject.SetActive(true);
        }

        private static void Return(CollectIconView obj)
        {
            obj.gameObject.SetActive(false);
        }

        private static void DestroyItem(CollectIconView obj)
        {
            Destroy(obj.gameObject);
        }

        private CollectIconView CreateFunc()
        {
            return Instantiate(collectIconViewPrefab, RectTransform);
        }


        [Button]
        private void CoinCollect(CoinCollectedEvent coinCollectedEvent)
        {
            coinCount++;
            UpdateCoinTextView();
            var startPos = coinCollectedEvent.position;
            var icon = coinCollectedEvent.sprite;
            var coin = CollectIconViewPool.Get();
            coin.Move(startPos, coinCollectTraget, icon, CollectIconViewPool);
            RuntimeManager.PlayOneShot("event:/Collect", transform.position);
        }
        
        private void UpdateCoinTextView()
        {
            coinText.text = coinCount.ToString();
        }
        
        private void UpdateLifeView()
        {
            for (var i = 0; i < lifeImages.Length; i++)
            {
                lifeImages[i].enabled = i < lifeCount;
            }
        }
    }
}