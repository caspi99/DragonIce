using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeController : MonoBehaviour
{
    [SerializeField] private List<Camera> cameras;

    [SerializeField]
    private Image black_screen_1;     //Black Screen Image component camera 1

    [SerializeField]
    private Image black_screen_2;     //Black Screen Image component camera 2

    [SerializeField] private List<RawImage> rawImageCamera;

    //method to change a GameObject visibility
    public void ChangeScreenTransparencyProgressive(float seconds, bool alpha_descending = false)
    {
        Color col = black_screen_1.color;

        float dt = Time.deltaTime;

        if (alpha_descending) { dt = -1 * dt; }

        if (seconds > 0)
        {
            col.a += dt / seconds;
        }
        else
        {
            if (alpha_descending) { col.a = 0; }
            else { col.a = 1; }
        }

        black_screen_1.color = col;
        black_screen_2.color = col;
    }

    //method to change a GameObject visibility
    public void CrossFadeProgressive(float seconds, int idx, bool alpha_descending = false)
    {
        Color col1 = rawImageCamera[2*idx].color;
        Color col2 = rawImageCamera[2*idx + 1].color;

        float dt = Time.deltaTime;

        if (alpha_descending) { dt = -1 * dt; }

        if (seconds > 0)
        {
            col1.a += dt / seconds;
            col2.a += dt / seconds;
        }
        else
        {
            if (alpha_descending) { col1.a = 0; col2.a = 0; }
            else { col1.a = 1; col2.a = 1; }
        }

        rawImageCamera[2 * idx].color = col1;
        rawImageCamera[2 * idx + 1].color = col2;
    }

    public void ScreenshotCamera(int idx)
    {
        int width = 1920; int height = 1080;
        Camera camera = cameras[idx];

        var renderTexture = new RenderTexture(width, height, 16);
        var texture2D = new Texture2D(width, height);

        var target = camera.targetTexture;
        camera.targetTexture = renderTexture;
        camera.Render();
        camera.targetTexture = target;

        var active = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        RenderTexture.active = active;

        texture2D.Apply();
        System.IO.File.WriteAllBytes("Assets/ScreenshotCamera"+idx.ToString()+".png", texture2D.EncodeToPNG());
    }
}
