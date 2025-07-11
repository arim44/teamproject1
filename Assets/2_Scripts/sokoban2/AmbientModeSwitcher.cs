using UnityEngine;
using UnityEngine.Rendering;

public class AmbientModeSwitcher : MonoBehaviour
{
    /// <summary>
    ///  skybox ���� ����
    /// </summary>
    private void SetSkyboxAmbient()
    {

        RenderSettings.ambientMode = AmbientMode.Skybox;
        //RenderSettings.ambientIntensity = 1.5f; // Intensity ��� �����ϰ� ������ ���
    }

    /// <summary>
    /// Color ���� ����
    /// </summary>
    /// <param name="color"></param>
    private void SetColorAmbient(Color color)
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        //RenderSettings.ambientLight = color;    // Į�����
        //RenderSettings.ambientIntensity = 1;    //Intensity ��� �����ϰ� ������ ���
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
