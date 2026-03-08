using UnityEngine;

public class UIData : MonoBehaviour
{
    public static UIData Instance { get; private set; }

    public Canvas mainCanvas;
    public Canvas worldCanvas;
    private RectTransform _mainCanvasRT;
    public RectTransform mainCanvasRT { get => _mainCanvasRT; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        if (mainCanvas != null)
        {
            _mainCanvasRT = mainCanvas.GetComponent<RectTransform>();
        }
    }
}
