using DG.Tweening;
using KabreetGames.SceneReferences;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace KendaAi.Scripts.Ui
{
    public class CollectIconView : MonoBehaviour
    {
        [SerializeField, Self] private Image image;
        [SerializeField, Self] private RectTransform rectTransform;

        [SerializeField] private float duration = 0.5f;
        private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
            image.enabled = false;
        }

        public void Move(Vector3 startPos, RectTransform coinCollectTarget, Sprite icon,
            LinkedPool<CollectIconView> pool)
        {
            var screenSize = new Vector2(960f, 540f);
            var viewPos = cam.WorldToViewportPoint(startPos);
            var screenPos = new Vector2(viewPos.x * screenSize.x, viewPos.y * screenSize.y);
            rectTransform.anchoredPosition = screenPos;
            image.enabled = true;
            image.sprite = icon;

            rectTransform.DOMove(coinCollectTarget.position, duration)
                .SetEase(Ease.Linear).onComplete += () =>
            {
                image.enabled = false;
                pool.Release(this);
            };
        }
    }
}