using UnityEngine;
using System;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Bloom : MonoBehaviour
{
    public bool ShowBloomOnly;
    [Range(0, 10)]
    public float intensity = 1;
    [Range(0, 10)]
    public float threshold = 1;
    [Range(0, 1)]
    public float softThreshold = 0.5f;
    [Range(1, 16)]
    public int iterations = 1;
    public Shader bloomShader;
    [NonSerialized]
    Material bloom;
    const int BoxDownPrefilterPass = 0;
    const int BoxDownPass = 1;
    const int BoxUpPass = 2;
    const int ApplyBloomPass = 3;
    const int ShowBloomOnlyPass = 4;
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (bloom == null)
        {
            bloom = new Material(bloomShader);
            bloom.hideFlags = HideFlags.HideAndDontSave;
        }
        float knee = threshold * softThreshold;
        Vector4 filter;
        filter.x = threshold;
        filter.y = filter.x - knee;
        filter.z = 2f * knee;
        filter.w = 0.25f / (knee + 0.00001f);
        bloom.SetVector("_Filter", filter);
        bloom.SetFloat("_Intensity", Mathf.GammaToLinearSpace(intensity));
        int width = source.width / 2;
        int height = source.height / 2;
        RenderTextureFormat format = source.format;
        RenderTexture[] textures = new RenderTexture[16];
        RenderTexture currentDestination = textures[0] = RenderTexture.GetTemporary(width, height, 0, format);

        //Graphics.Blit(source, destination);


        Graphics.Blit(source, currentDestination, bloom, BoxDownPrefilterPass);
        RenderTexture currentSource = currentDestination;


        int i = 1;
        for (; i < iterations; i++)
        {
            width /= 2;
            height /= 2;
            if (height < 2)
            {
                break;
            }
            currentDestination = textures[i] =
                RenderTexture.GetTemporary(width, height, 0, format);
            Graphics.Blit(currentSource, currentDestination, bloom, BoxDownPass);
            //RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }
        for (i -= 2; i >= 0; i--)
        {
            currentDestination = textures[i];
            textures[i] = null;
            Graphics.Blit(currentSource, currentDestination, bloom, BoxUpPass);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }


        bloom.SetTexture("_SourceTex", source);
        if (ShowBloomOnly)
        {
            Graphics.Blit(currentSource, destination, bloom, ShowBloomOnlyPass);
        }
        else
        {
            Graphics.Blit(currentSource, destination, bloom, ApplyBloomPass);
            RenderTexture.ReleaseTemporary(currentSource);
        }
    }
}