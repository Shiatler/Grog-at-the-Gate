using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Christina.UI
{
    public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
    {
        [Header("Slider Setup")]
        [SerializeField, Range(0, 1f)] private float slidervalue;
        public bool CurrentValue { get; private set; }

        private Slider _slider;

        [Header("Animation")]
        [SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve SlideEase =
            AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        private Coroutine _animateSliderCoroutine;

        [Header("Events")]
        [SerializeField] private UnityEvent onToggleOn;
        [SerializeField] private UnityEvent onToggleOff;

        private ToggleSwitchGroupManager _toggleSwitchGroupManager;

        protected void OnValidate()
        {
            SetupToggleComponents();

            _slider.value = slidervalue;
        }

        private void SetupToggleComponents()
        {
            if (_slider != null)
                return;
            
            SetupSliderComponents();
        }

        private void SetupSliderComponents()
        {
            _slider = GetComponent<Slider>();

            if (_slider == null)
            {
                Debug.Log("No Slider Found!", this);
                return;
            }

            _slider.interactable = false;
            var sliderColors = _slider.colors;
            sliderColors.disabledColor = Color.white;
            _slider.colors = sliderColors;
            _slider.transition = Selectable.Transition.None;
        }

        public void SetupForManager(ToggleSwitchGroupManager manager)
        {
            _toggleSwitchGroupManager = manager;
        }

        private void Awake()
        {
            SetupToggleComponents();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }

        private void Toggle()
        {
            if (_toggleSwitchGroupManager != null)
                _toggleSwitchGroupManager.ToggleGroup(this);
            else
                SetStateAndStartAnimation(!CurrentValue);
        }

        public void ToggleGroupManager(bool valueToSetTo)
        {
            SetStateAndStartAnimation(valueToSetTo);
        }
        
        private bool previousValue;
        private void SetStateAndStartAnimation(bool state)
        {
            previousValue = CurrentValue;
            CurrentValue = state;

            if (previousValue != CurrentValue)
            {
                if (CurrentValue)
                    onToggleOn?.Invoke();
                else
                    onToggleOff?.Invoke();
            }

            if (_animateSliderCoroutine != null)
                StopCoroutine(_animateSliderCoroutine);
            
            _animateSliderCoroutine = StartCoroutine(AnimateSlider());
        }

        private IEnumerator AnimateSlider()
        {
            float startValue = _slider.value;
            float endValue = CurrentValue ? 1f : 0f;

            float time = 0f;
            if (animationDuration > 0f)
            {
                while (time < animationDuration)
                {
                    time += Time.deltaTime;

                    float lerpFactor = SlideEase.Evaluate(time / animationDuration);
                    _slider.value = Mathf.Lerp(startValue, endValue, lerpFactor);

                    yield return null;
                }
            }

            _slider.value = endValue;
        }
    }
}
