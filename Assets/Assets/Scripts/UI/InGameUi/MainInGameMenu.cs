using KendaAi.Scripts.Ui;
using KabreetGames.EventBus.Interfaces;
using KabreetGames.SceneReferences;
using KabreetGames.UiSystem;
using KendaAi.Events;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace KendaAi.TestProject.InGameUi
{
    public class MainInGameMenu : MenuPanel
    {
        [field: SerializeField, Self] public RectTransform RectTransform { get; private set; }

        [SerializeField, Required] private CollectIconView collectIconViewPrefab;
        private LinkedPool<CollectIconView> collectIconViewPool;

        private LinkedPool<CollectIconView> CollectIconViewPool => collectIconViewPool ??=
            new LinkedPool<CollectIconView>(CreateFunc, Get, Return, DestroyItem);


        private int coinCount = 0;
        [SerializeField, Child(Flag.Editable)] private TextMeshProUGUI coinText;

        protected override void OnEnable()
        {
            base.OnEnable();
            EventBus<CoinCollectedEvent>.Register(CoinCollect);
            coinCount = PlayerPrefs.GetInt("CoinCount", 0);

            UpdateCoinTextView();
        }



        protected override void OnDisable()
        {
            base.OnDisable();
            EventBus<CoinCollectedEvent>.Deregister(CoinCollect);
            
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


        private void CoinCollect(CoinCollectedEvent coinCollectedEvent)
        {
            coinCount++;
            UpdateCoinTextView();
            var startPos = coinCollectedEvent.position;
            var icon = coinCollectedEvent.sprite;
            var coin = CollectIconViewPool.Get();
            coin.Move(startPos, icon, CollectIconViewPool);
        }
        
        private void UpdateCoinTextView()
        {
            coinText.text = coinCount.ToString();
        }
    }
}