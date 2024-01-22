using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 캐릭터에 대한 정보를 담고있는 객체
/// </summary>
[Serializable]
public class CharacterData
{
    public CData _characterData;

    /// <summary>
    /// 캐릭터에 관한 정보를 저장하는 구조체
    /// </summary>
    [Serializable]
    public struct CData
    {
        public CData
            (
            Vector3 pos,                                                                    // 플레이어의 위치
            Quaternion rot,                                                                 // 플레이어의 회전값
            string mapName,                                                                 // 플레이어의 마지막 저장할 때 있던 맵이름
            float currHp,                                                                   // 현재 남은 hp
            float maxHp,                                                                    // 최대 hp
            float currMp,                                                                   // 현재 남은 mp
            float maxMp,                                                                    // 최대 mp
            int seedCount,                                                                  // 씨앗을 몇개 갖고있는지
            int soulCount,                                                                  // 플레이어가 가지고 있는 영혼
            float basicAtkDamage,                                                           // 플레이어의 맨 몸 공격력
            string[] skillList,                                                             // 스킬 리스트
            bool isPlayerDead,                                                              // 플레이어가 죽고 데이터를 불러오는지 여부
            float movSpeed,                                                                 // 플레이어의 이동 속도
            bool alreadySeedGet                                                                // 플레이어가 생명 씨앗을 처음 발견했는지 여부
            )
        {
            _pos = pos;
            _rot = rot;
            _mapName = mapName;
            _currHP = currHp;
            _maxHP = maxHp;
            _currMP = currMp;
            _maxMP = maxMp;
            _seedCount = seedCount;
            _soulCount = soulCount;
            _basicAtkDamage = basicAtkDamage;
            _skillArray = skillList;
            _isPlayerDead = isPlayerDead;
            _movSpeed = movSpeed;
            _alreadySeedGet = alreadySeedGet;
        }

        public Vector3 _pos;
        public Quaternion _rot;
        public string _mapName;
        public float _currHP;
        public float _maxHP;
        public float _currMP;
        public float _maxMP;
        public int _seedCount;
        public int _soulCount;
        public float _basicAtkDamage;
        public string[] _skillArray;
        public bool _isPlayerDead;
        public float _movSpeed;
        public bool _alreadySeedGet;
    }

    /// <summary>
    /// 매개변수 없이 선언되면 기본값으로 CData 초기화
    /// </summary>
    public CharacterData()
    {
        _characterData = new CData(new Vector3(-116.14f, -4.67f, -65.99f), Quaternion.identity, "LittleForest_Map", 100, 100, 50, 50, 5, 10, 0.0f, new string[ConstData.SKILL_UI_COUNT], false, 6.2f, false);
    }

    /// <summary>
    /// 매개변수 있으면 있는값으로 CData 초기화
    /// </summary>
    /// <param name="cData"></param>
    public CharacterData(CData cData)
    {
        _characterData = cData;
    }
}

[Serializable]
public class BuffData
{
    public List<PlayerBuff> _buffDataList; // 플레이어에게 적용되고 있는 버프를 저장
    public List<float> _remainDurList; // 플레이어에게 적용되고 있는 버프의 지속시간을 따로 저장, 불러올 때 버프객체의 지속시간값이 자꾸 초깃값으로 초기화되서 따로 만들어놓음

    public BuffData()
    {
        _buffDataList = new List<PlayerBuff>();
        _remainDurList = new List<float>();
    }

    public BuffData(List<PlayerBuff> buffDataList, List<float> remainDurList)
    {
        _buffDataList = buffDataList;
        _remainDurList = remainDurList;
    }
}