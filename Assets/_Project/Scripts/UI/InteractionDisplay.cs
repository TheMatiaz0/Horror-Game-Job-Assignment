using DG.Tweening;
using UnityEngine;

namespace AE
{
    public class InteractionDisplay : MonoBehaviour
    {
        [SerializeField] private PlayerContext context;
        [SerializeField] private RectTransform indicator;
        [SerializeField] private InteractablePrompt requiredPrompt;

        [Header("Start Settings")]
        [SerializeField] private float startDuration = 1.2f;
        [SerializeField] private Ease startEase;
        [Header("Destination Settings")]
        [SerializeField] private Vector2 endPosition;
        [SerializeField] private float endDuration = 2f;
        [SerializeField] private Ease endEase;

        private Vector2 startPosition;
        private Tween startTween;
        private Tween endTween;

        private void Awake()
        {
            startPosition = indicator.anchoredPosition;
            context.InteractionController.OnTargetChanged += OnInteractionAvailable;
        }

        private void OnInteractionAvailable(IInteractable interactable)
        {
            if (interactable != null)
            {
                EnableIndicator(interactable);
            }
            else
            {
                DisableIndicator();
            }
        }

        private bool CanEnableIndicator(IInteractable interactable)
        {
            return startTween.IsActive() ||
                !interactable.IsInteractable ||
                requiredPrompt != interactable.InteractionPrompt ||
                (context.InteractionController.HeldItem != null && context.InteractionController.HeldItem.InteractionPrompt == interactable.InteractionPrompt);
        }

        private void EnableIndicator(IInteractable interactable)
        {
            if (CanEnableIndicator(interactable))
            {
                return;
            }

            if (endTween.IsActive())
            {
                endTween.Kill();
            }

            startTween = indicator.DOAnchorPos(endPosition, endDuration)
                .SetEase(startEase)
                .OnStart(EnableObject)
                .SetLink(this.gameObject);
        }

        private void DisableIndicator()
        {
            if (endTween.IsActive())
            {
                return;
            }
            if (startTween.IsActive())
            {
                startTween.Kill();
            }

            endTween = indicator.DOAnchorPos(startPosition, startDuration)
                .SetEase(endEase)
                .OnComplete(DisableObject)
                .SetLink(this.gameObject);
        }

        private void EnableObject()
        {
            indicator.gameObject.SetActive(true);
        }

        private void DisableObject()
        {
            indicator.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            context.InteractionController.OnTargetChanged -= OnInteractionAvailable;
        }
    }
}
