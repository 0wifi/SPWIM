using UnityEngine;
public class ButtonScript : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainCanvas;    //Main Canvas
    [SerializeField] private GameObject howToPlayCanvas; //How 2 Play
    [SerializeField] private GameObject creditCanvas;    // Credits

    public void HowToPlay()
    {
        mainCanvas.SetActive(false);
        if (creditCanvas != null) creditCanvas.SetActive(false);
        howToPlayCanvas.SetActive(true);
    }

    public void Credits()
    {
        mainCanvas.SetActive(false);
        howToPlayCanvas.SetActive(false);
        if (creditCanvas != null) creditCanvas.SetActive(true);
    }

    public void Back()
    {
        howToPlayCanvas.SetActive(false);
        if (creditCanvas != null) creditCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnClick()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}