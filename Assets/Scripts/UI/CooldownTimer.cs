using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CooldownTimer : MonoBehaviour
{
    [HideInInspector] public UnityEvent<float> StartCooldown;
    [HideInInspector] public UnityEvent Used;
    [HideInInspector] public UnityEvent Available;
    [HideInInspector] public UnityEvent Unavailable;
    [HideInInspector] public UnityEvent SwapIcon;

    public bool hasAlternateIcon;
    [ShowIf(nameof(hasAlternateIcon))] public Image primaryIcon;
    [ShowIf(nameof(hasAlternateIcon))] public Image alternateIcon;
    private bool swappedIcon;

    [SerializeField] private Image unavailableCross;
    [SerializeField] private Slider slider;

    private void Start()
    {
        StartCooldown.AddListener((length) => StartCoroutine(DoCooldownSlider(length)));
        Used.AddListener(SetZero);
        Unavailable.AddListener(SetUnavailable);
        Available.AddListener(SetAvailable);
        SwapIcon.AddListener(SwapIcons);
    }

    private void SetUnavailable()
    {
        unavailableCross.gameObject.SetActive(true);
        SetZero();
    }
    private void SetAvailable()
    {
        unavailableCross.gameObject.SetActive(false);
        SetFull();
    }
    public void SetZero()
    {
        slider.value = 0.0f;
    }
    public void SetFull()
    {
        slider.value = 1.0f;
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

        SetFull();
    }

    public void SwapIcons()
    {
        swappedIcon = !swappedIcon;

        primaryIcon.enabled = !swappedIcon;
        alternateIcon.enabled = swappedIcon;
    }
}