using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class PlayerBuff : ScriptableObject
{
    [SerializeField]
    private Sprite _buffImg;

    [Header("=== ���� ���� ===")]
    public string _buffName;
    public string _description;
    public string _buffComment;
    public GameObject _effect;

    [Header("=== ���� ���ӽð� ===")]
    [SerializeField]
    protected float _buffDur;

    [SerializeField]
    protected float _remainBuffDur;

    public Sprite BuffImg { get { return _buffImg; } set { _buffImg = value; } }
    public string BuffName { get { return _buffName; } }
    public float RemainBuffDur { get { return _remainBuffDur; } set { _remainBuffDur = value; } }
    public float BuffDur { get { return _buffDur; } }

    /// <summary>
    /// �÷��̾��� �پ��� ������ ���� �����ִ� �Լ�
    /// </summary>
    public abstract void BuffPlayer();

    /// <summary>
    /// �÷��̾�� ������ ������ ����Ǳ� ������ ������ �Լ�
    /// </summary>
    public abstract void ResetBuff();
}