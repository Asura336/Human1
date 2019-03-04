using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="InteractObject"/> 的策略之一，参与变换颜色的组件
/// </summary>
public class ColorExchanger: IEnteract
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

    public string Url { get ; set; }

    public ColorExchanger(Component parent, Material material)
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

        COLOR_TYPE tmp = gi.PlayerColorType;
        if (Point.Equals(tmp)) { return 1; }
        gi.PlayerColorType = (COLOR_TYPE)Point;
        Point = (int)tmp;

        gi.Url2Point[Url] = Point;
        ei.PostNotification(EVENT_TYPE.COLOR_ACT, parent, Point);

        wrap.ActClose();
        ei.PostNotification(EVENT_TYPE.ENTERACT_UI, wrap, EnteractType);
        return 0;
    }
}
