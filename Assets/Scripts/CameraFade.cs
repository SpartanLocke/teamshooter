using UnityEngine;
using System.Collections;

public class CameraFade : MonoBehaviour
{
    public bool fadingOut = false;
    private bool isFading;

    public float defaultFadeTime = 2;
    private float fadeTime;

    private static Texture2D Fade;
    public Color fadeColor = new Color(0, 0, 0, 0);

    public delegate void CameraFadeEvent(CameraFade fader);
    public event CameraFadeEvent fadeComplete;

    public static CameraFade GetCameraFade()
    {
        CameraFade inst = Camera.main.GetComponent<CameraFade>();
        if (!inst)
        {
            inst = Camera.main.gameObject.AddComponent<CameraFade>();
            inst.fadeColor = new Color(1, 1, 1, 0);
        }

        return inst;
    }

    public void Awake()
    {
        fadeColor.a = 1;
        BeginFade(false);
    }

    public void Start()
    {
        fadeTime = defaultFadeTime;
        if (!Fade)
            Fade = CreateFadeTexture();
        Fade.SetPixel(0, 0, new Color(1, 1, 1, 1));
    }

    private Texture2D CreateFadeTexture()
    {
        return new Texture2D(1, 1);
    }

    void OnGUI()
    {
        fadeColor.a = Mathf.Clamp01(fadeColor.a + ((Time.deltaTime / fadeTime) * (fadingOut ? 1 : -1)));
        if (fadeColor.a != 0)
        {
            GUI.depth = -10;
            GUI.color = fadeColor;
            GUI.DrawTexture(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), Fade, ScaleMode.StretchToFill, false);
        }
        if (fadeColor.a == 0 || fadeColor.a == 1 && isFading)
            EndFade();
    }

    public IEnumerator WaitForCameraFadeComplete()
    {
        if (isFading)
        {
            yield return new WaitForSeconds(Mathf.Abs((fadingOut ? 1 : 0) - fadeColor.a) * fadeTime);
            while (isFading)
            {
                yield return null;
            }
        }
    }

    public IEnumerator WaitForCameraFade(bool fadeOut)
    {
        BeginFade(fadeOut);
        yield return StartCoroutine(WaitForCameraFadeComplete());
    }

    public IEnumerator WaitForCameraFadeOutAndIn()
    {
        BeginFade(true);
        yield return StartCoroutine(WaitForCameraFadeComplete());
        BeginFade(false);
        yield return StartCoroutine(WaitForCameraFadeComplete());
    }

    private void EndFade()
    {
        isFading = false;
        if (fadeComplete != null)
            fadeComplete(this);
        fadeTime = defaultFadeTime;
    }

    public void BeginFade(bool fadeOut, float newFadeTime)
    {
        fadeTime = newFadeTime;
        BeginFade(fadeOut);
    }
    public void BeginFade(bool fadeOut)
    {
        isFading = true;
        fadingOut = fadeOut;
    }
}