using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class A3RoomMgr : QuestRoom
{
    [SerializeField] private int _sealCount;
    [SerializeField] private MapTeleport _portal_A4;
    [SerializeField] private TimelineAsset _cutScene;

    [SerializeField] private PlayableDirector _playableDirector;

    public override void SolveQuest()
    {
        if (--_sealCount == 0)
        {
            print("solve");
            ProductionMgr.StartProduction(_playableDirector);
            RewardQuest();
        }
    }

    public override void RewardQuest()
    {
        _portal_A4.gameObject.SetActive(true);
    }
}
