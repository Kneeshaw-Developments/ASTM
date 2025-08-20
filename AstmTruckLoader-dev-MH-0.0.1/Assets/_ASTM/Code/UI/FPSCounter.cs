using UnityEngine;
using TMPro;

public sealed class FPSCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsText;
    [SerializeField] float updateInterval = 0.5f;

    float timer;
    int frameCount;

    void Update()
    {
        frameCount++;
        timer += Time.unscaledDeltaTime;

        if (timer >= updateInterval)
        {
            float fps = frameCount / timer;
            fpsText.text = $"{fps:0}";
            frameCount = 0;
            timer = 0f;
        }
    }
}
