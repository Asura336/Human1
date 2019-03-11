using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// see <see cref="PlayerAct"/>
/// </summary>
public class AnimationEvent : MonoBehaviour
{
    void OnLand()
    {
        var ei = EventManager.Instance;
        ei.PostNotification(EVENT_TYPE.ENTERACT_AUDIO, this, SOUND.LAND);
    }
}
