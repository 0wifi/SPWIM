using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CooldownTimer : MonoBehaviour
{
    [HideInInspector] public UnityEvent<float> StartCooldown;
    [HideInInspector] public UnityEvent Used;

    [SerializeField] private Image unavailableCross;
    [SerializeField] private Slider slider;

    private void Start()
    {
        StartCooldown.AddListener((length) => StartCoroutine(DoCooldownSlider(length)));
        Used.AddListener(SetZero);
    }

    public void SetZero()
    {
        slider.value = 0f;
    }
    public IEnumerator DoCooldownSlider(float length)
    {
        float elapsedTime = 0f;

        do {
            slider.value = Mathf.Clamp01(elapsedTime / length);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        while (elapsedTime < length);

        slider.value = 1.0f;
    }
}