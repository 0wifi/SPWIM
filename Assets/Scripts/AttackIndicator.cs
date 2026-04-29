using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AttackIndicator : MonoBehaviour
{
    GameObject playerObject;
    [SerializeField] private Image indicator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // orient towards player
        transform.LookAt(playerObject.transform.position);
    }

    public void ShowFor(float seconds)
    {
        StartCoroutine(EnableFor(seconds));
    }
    public void Cancel()
    {
        StopAllCoroutines(); indicator.gameObject.SetActive(false);
    }

    private IEnumerator EnableFor(float seconds)
    {
        indicator.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        indicator.gameObject.SetActive(false);
    }

    
}
