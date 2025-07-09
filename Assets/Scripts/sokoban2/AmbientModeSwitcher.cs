using UnityEngine;
using UnityEngine.Rendering;

public class AmbientModeSwitcher : MonoBehaviour
{
    /// <summary>
    ///  skybox 모드로 변경
    /// </summary>
    private void SetSkyboxAmbient()
    {

        RenderSettings.ambientMode = AmbientMode.Skybox;
        //RenderSettings.ambientIntensity = 1.5f; // Intensity 밝기 조절하고 싶을때 사용
    }

    /// <summary>
    /// Color 모드로 변경
    /// </summary>
    /// <param name="color"></param>
    private void SetColorAmbient(Color color)
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        //RenderSettings.ambientLight = color;    // 칼라색상
        //RenderSettings.ambientIntensity = 1;    //Intensity 밝기 조절하고 싶을때 사용
    }

    public void InitializeAmbientMode(bool useSkybox, Color color)
    {
        if(useSkybox)
        {
            SetSkyboxAmbient();
        }
        else
        {
            SetColorAmbient(color);
        }
    }
}
