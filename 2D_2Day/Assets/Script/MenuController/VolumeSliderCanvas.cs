using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderCanvas : MonoBehaviour
{
    public OptionsMenu optionsMenu;
    public Canvas canvas;
    public Slider slider;
    public Text label;

    void Awake()
    {
        if (optionsMenu == null) optionsMenu = GetComponent<OptionsMenu>();
        if (canvas == null) canvas = GetComponentInChildren<Canvas>(true);
        if (canvas == null)
        {
            var go = new GameObject("OptionsCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            go.transform.SetParent(transform, false);
        }
        if (slider == null)
        {
            var panel = new GameObject("OptionsPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(canvas.transform, false);
            var rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(400, 150);
            rt.anchoredPosition = Vector2.zero;

            var sliderGO = new GameObject("VolumeSlider", typeof(RectTransform), typeof(Slider));
            sliderGO.transform.SetParent(panel.transform, false);
            var srt = sliderGO.GetComponent<RectTransform>();
            srt.anchorMin = new Vector2(0.5f, 0.5f);
            srt.anchorMax = new Vector2(0.5f, 0.5f);
            srt.sizeDelta = new Vector2(300, 30);
            srt.anchoredPosition = new Vector2(0, -10);
            slider = sliderGO.GetComponent<Slider>();

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderGO.transform, false);
            var fillRT = fillArea.GetComponent<RectTransform>();
            fillRT.anchorMin = new Vector2(0, 0.25f);
            fillRT.anchorMax = new Vector2(1, 0.75f);
            fillRT.offsetMin = new Vector2(10, 0);
            fillRT.offsetMax = new Vector2(-10, 0);

            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            var fillImg = fill.GetComponent<Image>();
            fillImg.color = new Color(0.2f, 0.7f, 1f);
            slider.fillRect = fill.GetComponent<RectTransform>();

            var bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(sliderGO.transform, false);
            var bgRT = bg.GetComponent<RectTransform>();
            bgRT.anchorMin = new Vector2(0, 0.25f);
            bgRT.anchorMax = new Vector2(1, 0.75f);
            bgRT.offsetMin = Vector2.zero;
            bgRT.offsetMax = Vector2.zero;
            var bgImg = bg.GetComponent<Image>();
            bgImg.color = new Color(0, 0, 0, 0.4f);

            var handleSlideArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleSlideArea.transform.SetParent(sliderGO.transform, false);
            var hsaRT = handleSlideArea.GetComponent<RectTransform>();
            hsaRT.anchorMin = new Vector2(0, 0);
            hsaRT.anchorMax = new Vector2(1, 1);
            hsaRT.offsetMin = new Vector2(10, 0);
            hsaRT.offsetMax = new Vector2(-10, 0);

            var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(handleSlideArea.transform, false);
            var handleImg = handle.GetComponent<Image>();
            handleImg.color = new Color(1, 1, 1);
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImg;

            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
        }
        if (label == null)
        {
            var textGO = new GameObject("VolumeLabel", typeof(RectTransform), typeof(Text));
            textGO.transform.SetParent(slider.transform.parent, false);
            var trt = textGO.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0.5f, 0.5f);
            trt.anchorMax = new Vector2(0.5f, 0.5f);
            trt.sizeDelta = new Vector2(300, 30);
            trt.anchoredPosition = new Vector2(0, 30);
            label = textGO.GetComponent<Text>();
            label.alignment = TextAnchor.MiddleCenter;
            label.fontSize = 24;
            label.color = Color.white;
            label.text = "Volume";
        }
        if (optionsMenu != null)
        {
            optionsMenu.masterVolumeSlider = slider;
            optionsMenu.volumeText = label;
        }
    }
}
