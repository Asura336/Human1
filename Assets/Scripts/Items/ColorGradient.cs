using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 如果 <see cref="masterUrl"/> 对应的 <see cref="ColorSpring"/> 组件值改动，将使颜色渐变
/// </summary>
public class ColorGradient : MonoBehaviour, IEventListener
{
    public string masterUrl;
    Renderer m_renderer;
    Material m_material;
    Light m_light;
    public Color endColor = Color.white;
    [SerializeField, Range(0, 1f)] float smooth = 0.1f;

    private void Start()
    {
        var u2p = GlobalHub.Instance.Url2Point;
        m_renderer = GetComponent<Renderer>();
        if (m_renderer != null) { m_material = m_renderer.material; }
        m_light = GetComponent<Light>();
        EventManager.Instance.AddListener(EVENT_TYPE.COLOR_GRADIENT, this);
        if (u2p.ContainsKey(masterUrl))
        {
            Gradient();
        }
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        if (eventType == EVENT_TYPE.COLOR_GRADIENT && masterUrl.Equals(param as string))
        {
            Gradient();
        }
    }

    void Gradient()
    {
        if (m_material != null) { StartCoroutine(GradientMaterial()); }
        if (m_light != null) { StartCoroutine(GradientLight()); }
    }

    IEnumerator GradientMaterial()
    {
        while (!m_material.color.Equals(endColor))
        {
            yield return null;
            m_material.color = Color.Lerp(m_material.color, endColor, smooth);
        }
    }

    IEnumerator GradientLight()
    {
        while (!m_light.color.Equals(endColor))
        {
            yield return null;
            m_light.color = Color.Lerp(m_light.color, endColor, smooth);
        }
    }
}
