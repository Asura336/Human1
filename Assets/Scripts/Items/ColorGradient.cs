using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(InteractObject))]
public class ColorGradient : MonoBehaviour
{
    Renderer m_renderer;
    Material m_material;
    public Color endColor = Color.white;
    [SerializeField, Range(0, 1f)] float smooth = 0.1f;

    private void Start()
    {
        m_renderer = GetComponent<Renderer>();
        m_material = m_renderer.material;
    }

    public void StartGradient()
    {
        StartCoroutine(Gradient());
    }

    IEnumerator Gradient()
    {
        while (!m_material.color.Equals(endColor))
        {
            yield return null;
            m_material.color = Color.Lerp(m_material.color, endColor, smooth);
        }
    }
}
