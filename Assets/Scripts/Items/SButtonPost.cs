using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 附在<see cref="StepButton.i_follower"/>字段，作为发送消息的组件
/// </summary>
public class SButtonPost : MonoBehaviour, IPhysicsInteract
{
    public void StateChange()
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.STEP_BUTTON, this, 
            new SBPoster()
            {
                dParam = dParam,
                strParam = strParam
            });
    }

    public void StateReset()
    {
        
    }

    public EVENT_TYPE eventType = EVENT_TYPE.STEP_BUTTON;
    public int dParam = 0;
    public string strParam = "LevelHubAlter_LG0";
}

public struct SBPoster
{
    public string strParam;
    public int dParam;
}
