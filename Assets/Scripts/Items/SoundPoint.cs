using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="MazePlayerChecker"/> 联动的声音提示器
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundPoint : MonoBehaviour, IEventListener
{
    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        var clips = GlobalHub.Instance.SoundClips;
        if (eventType == EVENT_TYPE.AUDIO && param.Equals(SOUND.POINT_OUT))
        {
            selfAudioSource.PlayOneShot(clips[(int)param]);
        }
    }

    AudioSource selfAudioSource;
    Transform selfTransform;

    public Vector3 Position
    {
        get { return selfTransform.position; }
        set { selfTransform.position = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        selfAudioSource = GetComponent<AudioSource>();
        selfTransform = transform;
        EventManager.Instance.AddListener(EVENT_TYPE.AUDIO, this);
    }
}
