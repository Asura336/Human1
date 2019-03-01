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

    COLOR_TYPE _colorType;
    public int Point
    {
        get { return (int)_colorType; }
        set
        {
            _colorType = (COLOR_TYPE)value;
            Color newColor = GlobalHub.Instance.colorTypes[(COLOR_TYPE)value];
            newColor.a = selfmaterial.color.a;
            selfmaterial.color = newColor;
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
        var gi = GlobalHub.Instance;
        var ei = EventManager.Instance;
        if (gi.PlayerColorType == COLOR_TYPE.NULL && _colorType != COLOR_TYPE.NULL)
        {
            ei.PostNotification(EVENT_TYPE.GET_KEY, wrap, Point);
            gi.PlayerColorType = (COLOR_TYPE)Point;
            _colorType = COLOR_TYPE.NULL;
            ei.PostNotification(EVENT_TYPE.COLOR_GRADIENT, wrap, Url);
            gi.Url2Point[Url] = Point;

            ActClose();
            ei.PostNotification(EVENT_TYPE.ENTERACT_UI, wrap, EnteractType);
            return 0;  // 成功互动
        }
        return 1; // 失败互动
    }
}
