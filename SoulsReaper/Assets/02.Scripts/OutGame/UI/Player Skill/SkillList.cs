using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillList : MonoBehaviour, IPointerClickHandler
{
    [Header("=== Skill ===")]
    [SerializeField]
    private Image[] _skills;

    [SerializeField]
    private GraphicRaycaster _gr;

    [Header("=== Data ===")]
    [SerializeField]
    private DataApply _data;

    [Header("=== Player ===")]
    [SerializeField]
    private PlayerCombat _player;

    private KeyCode[] _keyCode = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };
    private List<PlayerSkill> _skillList;
    private string[] _skillDataList;
    private DummySkill _grabbedSkill;

    private void Awake()
    {
        InitSkillList();

        
    }

    private void Update()
    {
        UseSkillInTheSkillList();
    }

    /// <summary>
    /// �ε����� �ִ� ��ų�� ���
    /// </summary>
    private void UseSkillInTheSkillList()
    {
        if (!Input.anyKeyDown)
            return;

        for (int i = 0; i < _keyCode.Length; i++)
        {
            if (Input.GetKeyDown(_keyCode[i]))
            {
                if (_skillList[i] == null)
                {
                    print((i + 1) + "��°���� ��ų�� ����ֽ��ϴ�");
                    return;
                }

                _skillList[i].UseSkill();
            }
        }
    }

    private void InitSkillList()
    {
        if (_skillDataList == null)
            _skillDataList = new string[ConstData.SKILL_UI_COUNT];
        if (_skillList == null)
            _skillList = new List<PlayerSkill>();

        // Skills ���� ������Ʈ�� ������ PlayerSkill �� �ִ��� ���ο� ���� �۾�
        _skillList.Clear();
        for (int i = 0; i < _skills.Length; i++)
        {
            PlayerSkill skill = _skills[i].GetComponentInChildren<PlayerSkill>();

            // ��ų�� ���ٸ�, Ŭ�� �� ��ų UI�� �ٴ��� �������� raycastTarget�� ����
            if (skill == null)
            {
                _skillList.Add(null);
                _skillDataList[i] = "";
                _skills[i].raycastTarget = true;
                continue;
            }

            // ��ų�� �ִٸ�, ��ų UI �ٴ��� raycast�� ����, ��ų�� Ŭ���� �� �ֵ��� ��
            _skillList.Add(skill);
            _skillDataList[i] = skill._skillID;
            _skills[i].raycastTarget = false;
        }

        // ������ ����
        CharacterDataPackage._cDataInstance._characterData._skillArray = _skillDataList;
        DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");
    }

    /// <summary>
    /// ��ųUI�� �ִ� ��ų���� �ű涧 ���̴� �޼���
    /// </summary>
    private void SwapSkillIndex(DummySkill grabbedSkill, PlayerSkill skillB)
    {
        Transform grabbedParent = grabbedSkill._originParent.parent;
        Transform skillBParent = skillB.transform.parent;

        grabbedSkill.SwapSkillPos(skillBParent);
        skillB.transform.SetParent(grabbedParent);
        skillB.transform.SetAsFirstSibling();
        skillB.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    // Ŭ���� ���� ��ų�� �ְų� ��������� �ٲٰ�, ��ų ����Ʈ�� �ƴϸ� ���̸� ���� ��ġ�� �ű��
    public void OnPointerClick(PointerEventData eventData)
    {
        // ��ų�� ������� ��
        if (_grabbedSkill != null)
        {
            List<RaycastResult> rayResult = new List<RaycastResult>();
            _gr.Raycast(eventData, rayResult);

            // Skill �±װ� ���������͵��� �Ÿ�
            for (int i = rayResult.Count - 1; i >= 0; i--)
            {
                if (!rayResult[i].gameObject.tag.Contains("Skill"))
                    rayResult.RemoveAt(i);
            }

            if (rayResult.Count.Equals(1))
            {
                // 1���� ���� ����ִ� ��ų�� Cast �ƴٴ� ��, ����ִ� ��ų�� �����ش�.
                _grabbedSkill.ToOriginPos();
                _grabbedSkill = null;
                return;
            }
            else
            {
                // 2�� �̻��� ���� �ٸ� Skill �±װ� �ִ°� Ž���Ǿ��ٴ� ��, ����� ���� �ٸ� ��ų�̰ų� �� ��ųUI �̴�

                // Dummy ����
                for (int i = rayResult.Count - 1; i >= 0; i--)
                {
                    if (rayResult[i].gameObject.tag.Contains("Dummy"))
                        rayResult.RemoveAt(i);
                }

                // �ٸ� ��ų Ȥ�� �� ��ų UI�� ���Ե�
                for (int i = 0; i < rayResult.Count; i++)
                {
                    PlayerSkill skill = rayResult[i].gameObject.GetComponent<PlayerSkill>();
                    if (skill == null)
                    {
                        _grabbedSkill.SwapSkillPos(rayResult[i].gameObject.transform);
                    }
                    else
                    {
                        SwapSkillIndex(_grabbedSkill, skill);
                    }

                    InitSkillList();
                    _grabbedSkill.ToOriginPos();
                    _grabbedSkill = null;
                }
            }
            return;
        }

        // ��ųUI�� Ŭ�������� ��ų�� �ִ°��̸� ���̰� ���콺�� ��������� ��
        DummySkill dummySkill = eventData.pointerCurrentRaycast.gameObject.GetComponent<DummySkill>();
        if (dummySkill == null)
            return;

        dummySkill.FollowMouse(this.transform);
        _grabbedSkill = dummySkill;
    }

    public void AddSkill(GameObject addSkill, int index = -1)
    {
        if (index != -1)
        {
            GameObject skill = Instantiate(addSkill);

            skill.transform.SetParent(_skills[index].transform);
            skill.transform.SetParent(_skills[index].transform);
            skill.transform.SetAsFirstSibling();
            skill.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            skill.GetComponent<PlayerSkill>().Player = _player;

            InitSkillList();
            return;
        }

        for (int i = 0; i < _skillList.Count; i++)
        {
            if (_skillList[i] == null)
            {
                GameObject skill = Instantiate(addSkill);

                skill.transform.SetParent(_skills[i].transform);
                skill.transform.SetParent(_skills[i].transform);
                skill.transform.SetAsFirstSibling();
                skill.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                skill.GetComponent<PlayerSkill>().Player = _player;

                InitSkillList();
                return;
            }
        }

    }
}