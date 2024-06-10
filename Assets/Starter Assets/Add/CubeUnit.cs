using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Unity.VisualScripting;

public class CubeUnit : MonoBehaviour
{
    [SerializeField] PlayableDirector timeLine;

    [SerializeField] float timeLineTime = 5.0f;
    float nowTime = 0;

    public void StartTimeLine()
    {
        timeLine.Play();
    }

    public void TimeReset()
    {
        nowTime = 0;
    }

    public void OnUpdate()
    {
        nowTime += Time.deltaTime;
        if(nowTime > timeLineTime)
        {
            CustomEvent.Trigger(gameObject, "TimeOver");
        }
    }
}
