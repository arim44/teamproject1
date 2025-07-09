using System.Collections;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    Renderer targetRenderer;
    void Start()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    private IEnumerator FadeAsync(Color start, Color end, float speed)
    {
        float elapsed = 0;
        while( true )
        {
            elapsed += Time.deltaTime / speed;
            Color color =Color.Lerp(start, end, elapsed);
            Material material = targetRenderer.material;
            material.SetColor("_BaseColor", color);
            material.SetColor("_EmissionColor", color);
            if (elapsed >= 1.0f)
                break;
            yield return null;
        }
    }

    private void Fade( Color start, Color end, float speed = 1 )
    {
        StartCoroutine( FadeAsync(start, end, speed));
    }

    public void FadeIn()
    {
        Fade(Color.black, Color.white, 1);
    }
    public void FadeOut()
    {
        Fade(Color.white, Color.black, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.A ))
        {
            FadeIn();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            FadeOut();
        }
    }
}
