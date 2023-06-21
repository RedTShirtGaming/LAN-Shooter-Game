using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using TMPro;
using QFSW.QC;

public enum QualityPreset { UltraRT, Ultra, Balanced, Performance, UltraPerformance };
public enum AntiAliasing { None, FXAA, TAA, SMAA };

public class GameInfo : MonoBehaviour
{


    [SerializeField] TextMeshProUGUI fpsDisplay;
    [SerializeField] GameObject fpsDisplayObj;
    private int lastFrameIndex;
    private float[] frameDeltaTimeArray;

    public QualityPreset qualityPreset;
    public AdvancedQualityOptions advancedQualityOptions;

    public bool displayFpsCounter;
    [Command("limit-fps")]
    public static bool limitFrameRate;
    [Range(15, 512)] public int fpsLimit;

    [Command("set-target-framerate")]
    private static void set_target_framerate(int fps)
    {
        Application.targetFrameRate = fps;
        Debug.Log("Set application target frame rate to " + fps);
    }
    [Command("display-framerate-counter", "Display the FPS Counter", MonoTargetType.Single)]
    private void display_framerate_counter(bool state)
    {
        fpsDisplayObj.SetActive(state);
        Debug.Log("Set FPS counter state to " + state);
    }

    private void Awake()
    {
        frameDeltaTimeArray = new float[50];
    }

    private void Update()
    {
        frameDeltaTimeArray[lastFrameIndex] = Time.deltaTime;
        lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;

        /*if (limitFrameRate) { Application.targetFrameRate = fpsLimit; }
        fpsDisplayObj.SetActive(displayFpsCounter);*/

        fpsDisplay.text = Mathf.RoundToInt(FPSCounter()).ToString() + " FPS";
    }

    private void Start()
    {
        /*ApplyAdvancedQualityOptions(advancedQualityOptions);*/
    }

    private void LateUpdate()
    {
        /*ApplyAdvancedQualityOptions(advancedQualityOptions);*/
    }

    void ApplyAdvancedQualityOptions(AdvancedQualityOptions aqo)
    {
        Camera camera = Camera.main;
        HDAdditionalCameraData hdCameraData = camera.GetComponent<HDAdditionalCameraData>();

        if (camera == null || hdCameraData == null) { Debug.LogError("Camera or HD Additional Camera Data is missing.\n Not applying graphic settings."); }

        hdCameraData.antialiasing = aqo.antiAliasing;
        camera.farClipPlane = aqo.viewDistance;
    }

    float FPSCounter()
    {
        float total = 0f;
        foreach (float deltaTime in frameDeltaTimeArray)
        {
            total += deltaTime;
        }
        return frameDeltaTimeArray.Length / total;
    }
}

[System.Serializable]
public class AdvancedQualityOptions
{
    public HDAdditionalCameraData.AntialiasingMode antiAliasing;
    public Light light;
    public int viewDistance = 1000;
}
