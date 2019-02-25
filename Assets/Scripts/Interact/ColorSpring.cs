using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="InteractObject"/> 的策略之一，提供颜色并开启新关卡
/// </summary>
public class ColorSpring : IEnteract
{
    Material selfmaterial;
    public Component parent;
    public InteractObject wrap;
    public ENTERACT_TYPE EnteractType { get; set; }
    public ColorGradient p_colorGradient;

    COLOR_TYPE _colorType;
    public int Point
    {
        get { return (int)_colorType; }
        set
        {
            _colorType = (COLOR_TYPE)value;
            selfmaterial.color =
                GlobalHub.Instance.colorTypes[(COLOR_TYPE)value];
        }
    }

    public string Url { get; set; }

    public ColorSpring(Component parent, Material material)
    {
        this.parent = parent;
        EnteractType = ENTERACT_TYPE.CLR_EXC;
        selfmaterial = material;
    }

    public void ActAwake()
    {
        GlobalHub.Instance.p_enteract = wrap;
    }

    public void ActClose()
    {
        GlobalHub.Instance.p_enteract = null;
    }

    public int ActDo()
    {
        var gl = GlobalHub.Instance;
        if (gl.PlayerColorType == COLOR_TYPE.NULL && _colorType != COLOR_TYPE.NULL)
        {
            gl.PlayerColorType = (COLOR_TYPE)Point;
            _colorType = COLOR_TYPE.NULL;
            p_colorGradient.StartGradient();  // 颜色消失的效果
            return 0;  // 成功互动
        }
        return 1; // 失败互动
    }
}
