using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VNCreator
{
    [RequireComponent(typeof(Button))]
    public class VNCreator_ButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [Header("Hover Settings")]
        public float hoverScaleMultiplier = 1.05f;
        public float animationSpeed = 15f;
        
        [Header("Audio")]
        public AudioClip clickSound;
        public AudioClip hoverSound;

        private Vector3 originalScale;
        private Vector3 targetScale;
        private Button button;

        private bool initialized = false;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (initialized) return;
            button = GetComponent<Button>();
            originalScale = transform.localScale;
            targetScale = originalScale;
            initialized = true;
        }

        private void OnEnable()
        {
            Initialize();
            targetScale = originalScale;
            transform.localScale = originalScale;
        }

        private void Update()
        {
            if (transform.localScale != targetScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!button.interactable) return;

            targetScale = originalScale * hoverScaleMultiplier;
            
            if (hoverSound != null && VNCreator_SfxSource.instance != null)
            {
                VNCreator_SfxSource.instance.PlayOneShot(hoverSound);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!button.interactable) return;

            targetScale = originalScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.interactable) return;

            if (clickSound != null && VNCreator_SfxSource.instance != null)
            {
                VNCreator_SfxSource.instance.PlayOneShot(clickSound);
            }
        }

        private void OnDisable()
        {
            if (initialized)
            {
                targetScale = originalScale;
                transform.localScale = originalScale;
            }
        }
    }
}
