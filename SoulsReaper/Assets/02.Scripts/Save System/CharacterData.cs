using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ĳ���Ϳ� ���� ������ ����ִ� ��ü
/// </summary>
[Serializable]
public class CharacterData
{
    public CData _characterData;

    /// <summary>
    /// ĳ���Ϳ� ���� ������ �����ϴ� ����ü
    /// </summary>
    [Serializable]
    public struct CData
    {
        public CData
            (
            Vector3 pos,                                                                    // �÷��̾��� ��ġ
            Quaternion rot,                                                                 // �÷��̾��� ȸ����
            string mapName,                                                                 // �÷��̾��� ������ ������ �� �ִ� ���̸�
            float currHp,                                                                   // ���� ���� hp
            float maxHp,                                                                    // �ִ� hp
            float currMp,                                                                   // ���� ���� mp
            float maxMp,                                                                    // �ִ� mp
            int seedCount,                                                                  // ������ � �����ִ���
            int soulCount,                                                                  // �÷��̾ ������ �ִ� ��ȥ
            float basicAtkDamage,                                                           // �÷��̾��� �� �� ���ݷ�
            string[] skillList,                                                             // ��ų ����Ʈ
            bool isPlayerDead,                                                              // �÷��̾ �װ� �����͸� �ҷ������� ����
            float movSpeed,                                                                 // �÷��̾��� �̵� �ӵ�
            bool alreadySeedGet                                                                // �÷��̾ ���� ������ ó�� �߰��ߴ��� ����
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
    /// �Ű����� ���� ����Ǹ� �⺻������ CData �ʱ�ȭ
    /// </summary>
    public CharacterData()
    {
        _characterData = new CData(new Vector3(-116.14f, -4.67f, -65.99f), Quaternion.identity, "LittleForest_Map", 100, 100, 50, 50, 5, 1000, 0.0f, new string[ConstData.SKILL_UI_COUNT], false, 6.2f, false);
    }

    /// <summary>
    /// �Ű����� ������ �ִ°����� CData �ʱ�ȭ
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
    public List<PlayerBuff> _buffDataList; // �÷��̾�� ����ǰ� �ִ� ������ ����
    public List<float> _remainDurList; // �÷��̾�� ����ǰ� �ִ� ������ ���ӽð��� ���� ����, �ҷ��� �� ������ü�� ���ӽð����� �ڲ� �ʱ갪���� �ʱ�ȭ�Ǽ� ���� ��������

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