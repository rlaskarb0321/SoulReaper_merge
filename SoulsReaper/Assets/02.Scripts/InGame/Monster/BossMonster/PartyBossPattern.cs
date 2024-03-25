using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ��Ƽ ������ ���� ����� �����ϴ� �Լ�
/// </summary>
public class PartyBossPattern : MonoBehaviour
{
    #region ��ǳ�� ���� ���� ������
    [Header("=== Dialog Ballon ===")]
    [SerializeField]
    private TextAsset _dialogFile;

    [SerializeField]
    [Tooltip("��ǳ�� ��� ��ġ")]
    private Transform _floatPos;

    [SerializeField]
    [Tooltip("��ǳ�� ��� ���ӽð�")]
    private float _floatTime;

    [SerializeField]
    [Tooltip("Rest �� �� �� Ȯ��")]
    [Range(0.0f, 1.0f)]
    private float _restDialogPercentage;

    [SerializeField]
    [Tooltip("��ø��� �������⶧ �� �� Ȯ��")]
    [Range(0.0f, 1.0f)]
    private float _runShyDialogPercentage;

    private float _currFloatTime;
    private bool _isTalking;
    private bool _stopLettering;
    #endregion ��ǳ�� ���� ���� ������

    #region ���� �ݶ��̴� ���� ���� ������
    [Header("=== Attack Coll Arr ===")]
    [SerializeField]
    private GameObject[] _attackColls;

    private enum eAttackColl { Left, Right, Left_Right, Foot, Head, Drop_Kick, }
    #endregion ���� �ݶ��̴� ���� ���� ������

    #region ����ũ ���� ���� ���� ������
    [Header("=== Blink Particle ===")]
    [SerializeField]
    [Tooltip("����ũ �� �� & ����ũ �� Ŀ���� ������ ����Ʈ")]
    private GameObject[] _stoneHit;

    [SerializeField]
    [Tooltip("����ũ�� �ڿ��� ��Ÿ���� �÷��̾�� �̰��� �Ÿ�")]
    private float _blinkOffset;
    #endregion ����ũ ���� ���� ���� ������

    #region ���� ���� ���� ���� ������
    [Header("=== Jump Attack ===")]
    [SerializeField]
    [Tooltip("�� �� ������ ����")]
    private float _rayDist;

    [SerializeField]
    [Tooltip("���� ���� ��")]
    private float _jumpAccel;

    [SerializeField]
    [Tooltip("���� ����")]
    private float _jumpHeight;

    private float _jumpSpeedTimes;
    private bool _isJump;
    private Vector3 _endPos;
    private Vector3 _startPos;
    #endregion ���� ���� ���� ���� ������

    #region �̴� ���� ��ȯ ���� ���� ������
    [Header("=== Mini Boss Summon ===")]
    [SerializeField]
    [Tooltip("�̴� ���� ��ȯ �ǽ��� �ϴ� ��ġ = ����")]
    private Transform[] _summonCastPos;

    [SerializeField]
    [Tooltip("��ȯ��")]
    private MonsterSummon _summonObj;

    [SerializeField]
    [Tooltip("��ȯ ĳ���� ���� �� ���͸� ���ǵ�")]
    private float _letteringSpeed;

    [SerializeField]
    [Tooltip("��ȯ �ϱ����� �ʿ��� �ð� = (�ֹ��� ���� �� x ���͸� ���ǵ� ��)")]
    private float _castingTime;

    [Tooltip("���� ĳ���� �� �ð���")]
    public float _currCastingTime;

    [HideInInspector]
    [Tooltip("��ȯ�� �����ߴ���")]
    public bool _isSummonStart;

    [SerializeField]
    [Tooltip("��ȯ�� �� �¾����� ���׼� ���")]
    private string[] _fireHitReaction;

    [SerializeField]
    [Tooltip("��ȯ�� �¾��� �� �������� ��� ����")]
    private float[] _gaugeShakeAmount;

    [SerializeField]
    [Tooltip("��ȯ�� �¾��� �� ������ ���� ���� �ð�")]
    private float[] _gaugeShakeDur;

    private WaitForSeconds _ws;
    private MiniBossAuraAnimCtrl _auraAnimCtrl;
    private bool _summonReady; // ������ �̴� ���� ��ȯ �غ� ������
    private int _summonPosIndex; // �������� ��ġ �̵��� �ε���
    private bool _isFireHit; // �� �¾Ҵ���
    #endregion �̴� ���� ��ȯ ���� ���� ������

    #region ����ġ�� & ��ø��� ���� ���� ������

    [Header("=== Run & shy ===")]
    [SerializeField]
    [Tooltip("�������� ����ĥ �� �ּ� ����")]
    private float _runRangeMin;

    [SerializeField]
    [Tooltip("�������� ����ĥ �� �ִ� ����")]
    private float _runRangeMax;

    #endregion ����ġ�� & ��ø��� ���� ���� ������

    #region �Ϲ� ���� ��ȯ ���� ���� ������

    [Header("=== �Ϲ� ���� ��ȯ ===")]
    [SerializeField]
    private MonsterSummonPool _normalMonster;

    [SerializeField]
    [Tooltip("�ѹ��� ��ȯ�ϴ� �Ϲ� ���� ��")]
    private int _summonCount;

    #endregion �Ϲ� ���� ��ȯ ���� ���� ������

    // Field
    private GameObject _target;
    private Rigidbody _rbody;
    private Animator _animator;
    private MonsterBase_1 _monsterBase;
    private BossDialog _bossDialog;
    private List<IndexingDict> _dialogData;

    private enum eDialogSituation 
    { 
        SummonPlace,            // �̴� ���� ��ȯ ��ҷ� �̵��ϸ鼭
        StartSummon,            // �̴� ���� ��ȯ�� ������ ��
        Summoning,              // �̴� ���� ��ȯ ��
        Complete_Summon,        // �̴� ���� ��ȯ �Ϸ�
        Run_Shy,                // �������� or ��ø���
        Take_a_Breath,          // ��������
    }

    // Anim Params
    private readonly int _hashBlink = Animator.StringToHash("Blink Trigger");
    private readonly int _hashBlinkBack = Animator.StringToHash("Blink Back Pos");
    private readonly int _hashSliding = Animator.StringToHash("Sliding Trigger");
    private readonly int _hashJump = Animator.StringToHash("Jump Trigger");
    private readonly int _hashJumpEnd = Animator.StringToHash("Jump End");
    private readonly int _hashFist = Animator.StringToHash("Fist Trigger");
    private readonly int _hashPush = Animator.StringToHash("Push Trigger");
    private readonly int _hashDropKick = Animator.StringToHash("Drop Kick Trigger");
    private readonly int _hashCeremony = Animator.StringToHash("Ceremony Trigger");
    private readonly int _hashIsFireHit = Animator.StringToHash("isFireHit");
    private readonly int _hashCompleteSummon = Animator.StringToHash("Complete Summon");
    private readonly int _hashFailSummon = Animator.StringToHash("Failed Summon");
    private readonly int _hashRest = Animator.StringToHash("Rest");
    private readonly int _hashRun = Animator.StringToHash("Run");
    private readonly int _hashNormalSummon = Animator.StringToHash("Normal Summon");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _monsterBase = GetComponent<MonsterBase_1>();
        _bossDialog = new BossDialog();
        _rbody = GetComponent<Rigidbody>();
        _target = _monsterBase._target;
        _dialogData = _bossDialog.DialogParsing(_dialogFile);
        _auraAnimCtrl = _summonObj.GetComponent<MiniBossAuraAnimCtrl>();
        _ws = new WaitForSeconds(_letteringSpeed);
    }

    private void Update()
    {
        // ���� ���� ��
        JumpParbola();

        // �̴� ���� ��ȯ�Ҷ��� ����
        GoSummonCastPos();

        // �̴� ���� ��ȯ��
        Summon();

        // ��ǳ�� ���� ������Ű��
        MaintainDialog();
    }

    #region Anim �������� ���������� ���̴� �޼���

    #region 1. ���ݿ� ������ �ʿ��� �� ����ϴ� �������� �޼���

    /// <summary>
    /// �� �޼��带 ȣ���ϴ� �������� ������ �Ѵ�. ������ ������ ������ ��ǥ������ �Ÿ���, �Ǽ��� ������ �Ÿ����� ���µ� �ɸ��� �� �ð����̴�.
    /// </summary>
    /// <param name="myEvent"></param>
    public void AddRush(AnimationEvent myEvent)
    {
        int y0 = myEvent.intParameter;
        float time = myEvent.floatParameter;

        StartCoroutine(Rush(y0, time));
    }

    private IEnumerator Rush(int y0, float time)
    {
        int count = (int)(time / Time.fixedDeltaTime);
        for (int i = 0; i < count; i++)
        {
            float force = -(y0 / count) * i + y0;
            _rbody.AddForce(force * transform.forward, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion 1. ���ݿ� ������ �ʿ��� �� ����ϴ� �������� �޼���

    #region 2. ������ �� �ݸ����� Ű�� ���� �޼���

    // index ��°�� ���ӿ�����Ʈ�� ���������� ���ְ�, ���������� ���ֱ�
    public void SetAttackCollActive(int collIndex)
    {
        bool activeSelf = _attackColls[collIndex].activeSelf;
        _attackColls[collIndex].SetActive(!activeSelf);
    }

    public void SetOffAllColl()
    {
        for (int i = 0; i < _attackColls.Length; i++)
        {
            if (_attackColls[i] != null)
                _attackColls[i].gameObject.SetActive(false);
        }
    }

    #endregion 2. ������ �� �ݸ����� Ű�� ���� �޼���

    #region 3. ��ǳ��

    /// <summary>
    /// ���� �ϰ��ϴ� �޼���
    /// </summary>
    private void ShowDialog(string text, bool turnOn)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatPos.position);

        _isTalking = turnOn;
        _currFloatTime = _floatTime;
        UIScene._instance.FloatInteractTextUI(UIScene._instance._dialogBallon, turnOn, pos, text);
    }

    /// <summary>
    /// ���� �� ��, UI �� ���� �ϴ� �޼���
    /// </summary>
    private void MaintainDialog()
    {
        if (!_isTalking)
            return;
        if (_currFloatTime <= 0.0f)
        {
            _currFloatTime = _floatTime;
            _isTalking = false;
            UIScene._instance.FloatInteractTextUI(UIScene._instance._dialogBallon, false, Vector3.zero, "");
            return;
        }

        Vector3 pos = Camera.main.WorldToScreenPoint(_floatPos.position);
        UIScene._instance.FloatInteractTextUI(UIScene._instance._dialogBallon, true, pos, "");
        _currFloatTime -= Time.deltaTime;
    }

    /// <summary>
    /// �ѱ��ھ� ��ȭ���� �����
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private IEnumerator LetteringDialog(string text)
    {
        StringBuilder sb = new StringBuilder();
        int index = 0;

        ShowDialog("", false);
        while (true)
        {
            if (_stopLettering)
            {
                _stopLettering = false;
                break;
            }

            if (index >= text.Length)
            {
                ShowDialog("", false);
                index = 0;
                sb.Clear();
            }

            // ���߿� �ҿ� ������
            if (_isFireHit)
            {
                int randomValue = Random.Range(0, _fireHitReaction.Length);
                string reaction = _fireHitReaction[randomValue];

                _isFireHit = false;
                sb.Append(reaction);
                ShowDialog(sb.ToString(), true);
                index++;

                continue;
            }

            sb.Append(text[index]);
            ShowDialog(sb.ToString(), true);
            index++;

            yield return _ws;
        }
    }

    #endregion 3. ��ǳ��

    #region 4. �÷��̾�� �̵��ϱ�

    public void GoToTarget()
    {

    }

    #endregion 4. �÷��̾�� �̵��ϱ�

    #endregion Anim �������� ���������� ���̴� �޼���

    #region �Ϲ����� ������ �� ������ ��ų��

    #region ����ũ ����

    public void Blink()
    {
        _animator.SetTrigger(_hashBlink);
    }

    /// <summary>
    /// ����ũ�� ��Ÿ���� �ִϸ��̼� ��ȯ�� Ʈ����
    /// </summary>
    public void BlinkAppear()
    {
        _animator.SetTrigger(_hashBlinkBack);
    }

    /// <summary>
    /// ����ũ �� �� ������ ��ƼŬ ����Ʈ�� ���ִ� �ִϸ��̼� ��������Ʈ
    /// </summary>
    public void ActiveBlinkParticle(int index)
    {
        _stoneHit[index].transform.position = transform.position + Vector3.up * 2.0f;
        _stoneHit[index].SetActive(true);
    }

    /// <summary>
    /// Ÿ���� �ڳ� ������ �����̵�
    /// </summary>
    public void MoveToTargetBehind()
    {
        Vector3 blinkPos = _target.transform.position + (_target.transform.forward * -_blinkOffset);
        RaycastHit hit;
        bool groundHit = Physics.Raycast(blinkPos, new Vector3(blinkPos.x, -1.0f, blinkPos.z) - blinkPos, out hit, 1 << LayerMask.NameToLayer("Ground"));

        if (!groundHit || hit.transform.CompareTag("Non Blink"))
            blinkPos = _target.transform.position + (_target.transform.forward * _blinkOffset);

        transform.position = blinkPos;
        transform.forward = (_target.transform.position - blinkPos).normalized;
    }

    

    #endregion ����ũ ����

    #region �̴� ���� ��ȯ�ϱ�

    public void SummonMiniBoss()
    {
        int randomValue = Random.Range(0, _dialogData[(int)eDialogSituation.SummonPlace]._dialogs.Count);
        string text = _dialogData[(int)eDialogSituation.SummonPlace]._dialogs[randomValue];

        _summonReady = true;
        _monsterBase._nav.enabled = true;
        _animator.SetBool(_hashFailSummon, false);
        ShowDialog(text, true);
    }

    /// <summary>
    /// �̴� ���� ��ȯ�������� �̵� & ��ȯ �ǽ� ����
    /// </summary>
    private void GoSummonCastPos()
    {
        if (!_summonReady)
            return;

        float dist = Vector3.Distance(transform.position, _summonCastPos[_summonPosIndex].position);
        if (dist <= _monsterBase._nav.radius * 0.5f)
        {
            transform.position = _summonCastPos[_summonPosIndex].position;
            switch (_summonPosIndex)
            {
                case 0:
                    _summonPosIndex++;
                    return;

                // ��ȯ �ǽ� ����
                case 1:
                    int randomValue = Random.Range(0, _dialogData[(int)eDialogSituation.StartSummon]._dialogs.Count);
                    string text = _dialogData[(int)eDialogSituation.StartSummon]._dialogs[randomValue];

                    _monsterBase._animator.SetBool(_monsterBase._hashMove, false);
                    _monsterBase._nav.enabled = false;
                    _animator.SetTrigger(_hashCeremony);
                    _animator.ResetTrigger(_hashIsFireHit);
                    _summonReady = false;
                    ShowDialog(text, true);
                    return;
            }
        }

        _monsterBase.Move(_summonCastPos[_summonPosIndex].position, (_monsterBase._stat.movSpeed / (_summonPosIndex + 1)) + _summonPosIndex);
    }

    /// <summary>
    /// �ǽ� ���� �ִϸ��̼��� ������ �����ӿ� �޾Ƴ��� event
    /// </summary>
    public void StartMiniBossSummon()
    {
        _summonObj.StartSummon();
        ContinueSummon(1);
    }

    /// <summary>
    /// �ǽ� ���� �ִϸ��̼��� ������ �����ӿ� �޾Ƴ��� animation event
    /// </summary>
    public void ContinueSummon(int value)
    {
        bool isContinue = value == 1 ? true : false;
        _summonPosIndex = 0;
        _isSummonStart = isContinue;

        UIScene._instance.SetGaugeUI(isContinue);
        if (value != 1 && value != 0)
            return;

        if (isContinue)
        {
            StartCoroutine(LetteringDialog(_dialogData[(int)eDialogSituation.Summoning]._dialogs[0]));
        }
        else
        {
            _currFloatTime = _floatTime;
            _isTalking = false;
            UIScene._instance.FloatInteractTextUI(UIScene._instance._dialogBallon, false, Vector3.zero, "");
        }
    }

    /// <summary>
    /// ��ȯ �������϶� ȣ���ϴ� �޼���
    /// </summary>
    private void Summon()
    {
        if (!_isSummonStart)
        {
            _currCastingTime = 0.0f;
            return;
        }

        if (_currCastingTime >= _castingTime)
        {
            CompleteSummonMiniBoss();
            return;
        }

        _currCastingTime += Time.deltaTime;
        UIScene._instance.SetGaugeFill(_currCastingTime, _castingTime);
    }

    /// <summary>
    /// ��ȯ ���� ������ ���� �Լ�
    /// </summary>
    public void HitDuringSummon(bool isFire, float decreaseCasting)
    {
        float shakeAmount;
        float shakeDur;

        if (isFire)
        {
            shakeAmount = _gaugeShakeAmount[(int)eArrowState.Fire];
            shakeDur = _gaugeShakeDur[(int)eArrowState.Fire];

            _animator.SetTrigger(_hashIsFireHit);
            _isFireHit = true;
        }
        else
        {
            shakeAmount = _gaugeShakeAmount[(int)eArrowState.Normal];
            shakeDur = _gaugeShakeDur[(int)eArrowState.Normal];
        }

        _currCastingTime -= decreaseCasting * Time.deltaTime;
        // �¾Ƽ� ��ȯ�� �������� ��
        if (_currCastingTime < 0.0f)
        {
            _animator.SetBool(_hashFailSummon, true);
            _isFireHit = false;
            _stopLettering = true;
            _currCastingTime = 0.0f;
            _auraAnimCtrl.SummonFail();
            _summonObj.SetMonsterOff();
            ContinueSummon(0);
            return;
        }

        StartCoroutine(UIScene._instance.ChangeGaugeColor("FFFFFF"));
        StartCoroutine(UIScene._instance.ShakeGaugeUI(shakeAmount, shakeDur));
    }

    /// <summary>
    /// ��ȯ �Ϸ�� ȣ���ϴ� �޼���
    /// </summary>
    private void CompleteSummonMiniBoss()
    {
        int randomValue = Random.Range(0, _dialogData[(int)eDialogSituation.Complete_Summon]._dialogs.Count);
        string text = _dialogData[(int)eDialogSituation.Complete_Summon]._dialogs[randomValue];

        _animator.SetTrigger(_hashCompleteSummon);
        _currCastingTime = 0.0f;
        _stopLettering = true;
        _isFireHit = false;
        
        _auraAnimCtrl.SummonSuccess();

        ShowDialog(text, true);
        ContinueSummon(-1);
    }

    #endregion �̴� ���� ��ȯ�ϱ�

    #region ���ű ����

    public void DropKick()
    {
        _animator.SetTrigger(_hashDropKick);
    }

    #endregion ���ű ����

    #region �Ϲ� ���� ��ȯ�ϱ�

    public void SummonNormalMonster()
    {
        _animator.SetTrigger(_hashNormalSummon);
        _normalMonster.SummonMonster(_summonCount);
    }

    #endregion �Ϲ� ���� ��ȯ�ϱ�

    #region �����̵� ����

    public void Sliding()
    {
        _animator.SetTrigger(_hashSliding);
    }

    #endregion �����̵� ����

    #region ���� ����

    // ���� �� �������� ���� �˷��ִ� �׸��� ������Ʈ�� �ʿ�

    public void Jump()
    {
        _animator.SetBool(_hashJumpEnd, false);
        _animator.SetTrigger(_hashJump);
        _isJump = false;
    }

    /// <summary>
    /// JumpStart Anim�� ���� �����ӿ� ���� delegate �Լ�
    /// </summary>
    public void JumpStart()
    {
        _isJump = true;
        _startPos = transform.position;
        _endPos = _target.transform.position;
        _jumpSpeedTimes = 0.0f;
    }

    /// <summary>
    /// Update ���� ������ ü������ �� ���� ���� ���θ� �Ǵ��ϴ� �޼���
    /// </summary>
    private void JumpParbola()
    {
        if (!_isJump)
            return;

        Vector3 startPos = _startPos;
        Vector3 endPos = _endPos;
        Vector3 center = (startPos + endPos) * 0.5f;

        center = new Vector3(center.x, center.y - _jumpHeight, center.z);
        startPos = startPos - center;
        endPos = endPos - center;

        transform.position = Vector3.Slerp(startPos, endPos, _jumpSpeedTimes);
        center.y += _jumpHeight * Mathf.Sin(Mathf.PI * _jumpSpeedTimes);
        transform.position += center;

        _jumpSpeedTimes += Time.deltaTime * _jumpAccel;
        _animator.SetBool(_hashJumpEnd, _jumpSpeedTimes >= 0.9f);
        if (_jumpSpeedTimes >= 1.0f)
        {
            _isJump = false;
            _jumpSpeedTimes = 1.0f;
        }
    }

    #endregion ���� ����

    #region �ָ� ��� ����

    public void Fist()
    {
        _animator.SetTrigger(_hashFist);
    }

    #endregion �ָ� ��� ����

    #region �б� ����

    public void Push()
    {
        _animator.SetTrigger(_hashPush);
    }

    #endregion �б� ����

    #endregion �Ϲ����� ������ �� ������ ��ų��

    #region ��ģ ������ �� ������ ��ų��

    #region �޽� �ϱ�

    public void Rest()
    {
        if (Random.Range(0.0f, 1.0f) <= _restDialogPercentage)
        {
            int randomValue = Random.Range(0, _dialogData[(int)eDialogSituation.Take_a_Breath]._dialogs.Count);
            string text = _dialogData[(int)eDialogSituation.Take_a_Breath]._dialogs[randomValue];
            ShowDialog(text, true);
        }

        _animator.SetTrigger(_hashRest);
    }

    #endregion �޽� �ϱ�

    #region ���� ġ��

    public void Run()
    {
        _animator.SetTrigger(_hashRun);
    }

    public void MoveToRandomPos()
    {
        float range = Random.Range(_runRangeMin, _runRangeMax);
        Vector3 randomPos = transform.position + Random.insideUnitSphere * range;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPos, out hit, range, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            transform.forward = _monsterBase._target.transform.position - transform.position;
        }
    }

    public void Say(int value)
    {
        int randomValue = Random.Range(0, _dialogData[(int)eDialogSituation.Run_Shy]._dialogs.Count);
        string text = _dialogData[(int)eDialogSituation.Run_Shy]._dialogs[randomValue];
        bool isOn = value == 1 ? true : false;

        ShowDialog(text, isOn);
    }

    #endregion ���� ġ��

    #endregion ��ģ ������ �� ������ ��ų��
}