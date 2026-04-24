using System.Collections;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private float deleteDelay;
    [SerializeField] private Vector3 moveSpeed;
    [SerializeField] private Vector3 moveSpeedDecay;
    private GameObject playerObject;

    [SerializeField] private TMP_Text displayText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Finds the player for reference
        playerObject = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(DeleteDelay());
    }

    // Update is called once per frame
    void Update()
    {
        //Keep facing the player
        transform.LookAt(playerObject.transform.position);
    }

    private void FixedUpdate()
    {
        transform.position += moveSpeed;

        moveSpeed -= moveSpeedDecay;
    }

    private IEnumerator DeleteDelay()
    {
        yield return new WaitForSeconds(deleteDelay);
        Destroy(gameObject);
    }

    public void UpdateText(string text)
    {
        displayText.text = "-" + text;
    }
}
