using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIScene : MonoBehaviour
{
    // Instance
    public static UIScene _instance;

    [Header("=== �г� & �˶� ===")]
    [SerializeField]
    private List<GameObject> _currOpenPanel;
    
    [SerializeField]
    private GameObject _pausePanel; // �Ͻ�����
    public LetterPanel _letter; // ������

    [Space(10.0f)]
    [SerializeField]
    private TMP_Text _alarmText;

    [Header("=== ������ ���� UI ===")]
    public SeedUI _seedUI;

    [Header("=== �÷��̾� ===")]
    public PlayerData _stat;

    [SerializeField]
    private PlayerDeath _playerDeath;

    [Header("=== Hp & Mp ===")]
    [SerializeField] 
    private Image _hpFill;

    [SerializeField]
    private Image _hpBorder;

    [SerializeField] 
    private TMP_Text _hpText;

    [Space(10.0f)]
    [SerializeField] 
    private Image _mpFill;

    [SerializeField]
    private Image _mpBorder;

    [SerializeField]
    private AudioClip _lowMPWarnSound;

    [SerializeField]
    private int _lowMPWarnRepeat;

    [SerializeField] 
    private TMP_Text _mpText;
    public enum ePercentageStat { HP, MP, }

    [Header("=== ��ȥ ȹ�� ī��Ʈ ===")]
    [SerializeField]
    private SoulCountUI _soulCount;

    [Header("=== �� �ٲٱ� ===")]
    [SerializeField] 
    private TMP_Text _mapName;

    [Header("=== ��ü�� �߰� �� UI ===")]
    public RectTransform _interactUI; // �ΰ��ӿ��� ��ȣ�ۿ� ������ ��ü ������ ���� �� �߰� �� �ؽ�Ʈ UI
    public RectTransform _dialogBallon; // ������ ��� ��ǳ�� UI

    [SerializeField]
    private GameObject _gaugeObj;

    [SerializeField]
    private Image _gaugeFill;

    private Color _originColor;
    private string _uiTag; // �ֱٿ� ��� UI�� �±׸� ����, ���� ������ ���� ���� ���� UI�� �±װ� �ٸ��ٸ� _context �� ���� getcomponent 
    private TMP_Text _context;
    private Vector3 _originGaugePos;

    [Header("=== ���� ===")]
    public BuffDataPackage _buffMgr;

    [Header("=== ��ų ===")]
    public SkillList _skillMgr;

    [Header("=== ������ ===")]
    [SerializeField]
    private DataApply _apply;

    // Field
    private AudioSource _audio;

    private void Awake()
    {
        _instance = this;

        _audio = GetComponent<AudioSource>();
        _currOpenPanel = new List<GameObject>();
        _mapName.text = EditMapName();
    }

    private void Start()
    {
        _originGaugePos = _gaugeObj.transform.position;
        _originColor = _gaugeFill.color;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel();
        }

        // �׽�Ʈ �� �ҿ� ������ 1000 ����
        if (Input.GetKeyDown(KeyCode.C))
        {
            UpdateSoulCount(1000.0f);
        }

        // �׽�Ʈ �� �ҿ� ������ 100 ����
        if (Input.GetKeyDown(KeyCode.V))
        {
            UpdateSoulCount(-100.0f);
        }
    }

    #region UI Panel

    /// <summary>
    /// UI�г��� Ű�� , �����ִ� ui�� ����Ʈ�� ����
    /// </summary>
    /// <param name="panel"></param>
    public void SetUIPanelActive(GameObject panel)
    {
        UIPanel uiPanel = panel.GetComponent<UIPanel>();

        panel.SetActive(!panel.activeSelf);
        _currOpenPanel.Add(panel);
        if (uiPanel != null)
        {
            uiPanel.PlaySetActiveSound(true, _audio);
        }
    }

    /// <summary>
    /// UI �г��� ����, ui ����Ʈ���� ��
    /// </summary>
    /// <param name="panel"></param>
    public void SetUIPanelDeactive(GameObject panel)
    {
        if (!_currOpenPanel.Contains(panel))
            return;

        panel.SetActive(false);
        _currOpenPanel.Remove(panel);
    }

    public void PausePanel()
    {
        UIPanel uiPanel;

        // �����ִ� UI�� ������ ��
        if (_currOpenPanel.Count != 0)
        {
            int index = _currOpenPanel.Count - 1;
            uiPanel = _currOpenPanel[index].GetComponent<UIPanel>();
            if (uiPanel != null)
            {
                uiPanel.PlaySetActiveSound(false, _audio);
            }

            _currOpenPanel[index].SetActive(false);
            _currOpenPanel.RemoveAt(index);
            return;
        }

        // PausePanel �� Ŵ
        uiPanel = _pausePanel.GetComponent<UIPanel>();
        if (uiPanel != null)
            uiPanel.PlaySetActiveSound(true, _audio);

        SetUIPanelActive(_pausePanel);
    }
    #endregion UI Panel

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // ���ø����̼� ����
#endif
    }

    /// <summary>
    /// ��ȥ ��ȭ ������ ���� ������ų �� ���� �޼���
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateSoulCount(float amount)
    {
        // UI ī��Ʈ ȿ��
        _soulCount.StartCount
            (
            CharacterDataPackage._cDataInstance._characterData._soulCount + amount,
            CharacterDataPackage._cDataInstance._characterData._soulCount
            );

        // ������ ����
        _stat._soulCount = (int)CharacterDataPackage._cDataInstance._characterData._soulCount + (int)amount;
        CharacterDataPackage._cDataInstance._characterData._soulCount = _stat._soulCount;
        DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");
    }

    /// <summary>
    /// ���� ���ѷ��� ������ų �� ���� �޼���
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateSeedCount(int amount)
    {
        _seedUI.EditSeedText(amount);
        _stat._seedCount = amount;

        CharacterDataPackage._cDataInstance._characterData._alreadySeedGet =
            CharacterDataPackage._cDataInstance._characterData._alreadySeedGet == false ? true : true;
        CharacterDataPackage._cDataInstance._characterData._seedCount = _stat._seedCount;
        DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");
    }

    #region HP & MP
    /// <summary>
    /// �ʿ��� Mp������ ������ Mp���� ������ ȣ���, Mp ���� UX ���� �޼����̴�.
    /// </summary>
    /// <returns></returns>
    public IEnumerator WarnLowMP()
    {
        Color originColor = Color.white;
        _audio.clip = _lowMPWarnSound;
        if (_audio.isPlaying)
            yield break;

        _audio.Play();
        for (int i = 0; i < _lowMPWarnRepeat; i++)
        {
            _mpBorder.color = Color.blue;
            yield return new WaitForSeconds(0.2f);

            _mpBorder.color = originColor;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void UpdateHPMP(ePercentageStat stat, float currValue, float maxValue, bool isDataEdit = true)
    {
        if (currValue > maxValue)
            currValue = maxValue;

        switch (stat)
        {
            case ePercentageStat.HP:
                _stat._currHP = currValue;
                _stat._maxHP = maxValue;
                _hpText.text = $"{currValue} / {maxValue}";
                _hpFill.fillAmount = currValue / maxValue;
                break;

            case ePercentageStat.MP:
                _stat._currMP = currValue;
                _stat._maxMP = maxValue;
                _mpText.text = $"{currValue} / {maxValue}";
                _mpFill.fillAmount = currValue / maxValue;
                break;
        }

        if (isDataEdit)
        {
            _apply.EditData();
        }
    }
    #endregion HP & MP

    #region Buff
    public void UpdatePlayerDamage(float amount)
    {
        _stat._basicAtkDamage = amount;
    }

    public void UpdatePlayerMovSpeed(float amount)
    {
        _stat._move._movSpeed = amount;
    }

    #endregion Buff

    // �� �ű�鼭 ������ �� ���Ӱ��� ����
    private string EditMapName()
    {
        string mapName;
        int sceneCount = SceneManager.sceneCount;
        List<string> sceneList = new List<string>();
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = SceneManager.GetSceneAt(i).name;
            if (!sceneName.Contains("_Map"))
                continue;

            sceneList.Add(sceneName);
        }

        mapName = sceneList[0];
        switch (mapName)
        {
            case "Castle_Map":
                return "��";

            case "LittleForest_Map":
                return "���� ��";
        }

        return "";
    }

    public void FloatInteractTextUI(RectTransform target, bool turnOn, Vector3 pos, string text)
    {
        if (!turnOn)
        {
            target.gameObject.SetActive(false);
            return;
        }

        if (_uiTag != target.tag)
        {
            _context = target.GetComponentInChildren<TMP_Text>();
            _uiTag = target.tag;
        }

        if (!_context.text.Equals(text) && !text.Equals(""))
        {
            _context.text = text;
            return;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        target.transform.position = pos;
        target.gameObject.SetActive(true);
    }

    public void FloatAlarmTextUI(string content)
    {
        _alarmText.gameObject.SetActive(false);
        _alarmText.gameObject.SetActive(true);
        _alarmText.text = content;
    }

    #region ������ UI
    public void SetGaugeUI(bool activeValue) => _gaugeObj.SetActive(activeValue);

    public void SetGaugeFill(float curr, float total) => _gaugeFill.fillAmount = curr / total;

    public IEnumerator ChangeGaugeColor(string htmlString)
    {
        Color changeColor;
        ColorUtility.TryParseHtmlString(htmlString, out changeColor);
        _gaugeFill.color = changeColor;

        yield return new WaitForSeconds(Time.deltaTime * 4.0f);

        _gaugeFill.color = _originColor;
    }

    public IEnumerator ShakeGaugeUI(float amount, float dur)
    {
        Vector3 originPos = _originGaugePos;
        while (dur > 0.0f)
        {
            Vector3 shakePos = Random.insideUnitCircle * amount;

            _gaugeObj.transform.position += shakePos;
            dur -= Time.deltaTime;
            yield return null;
        }

        _gaugeObj.transform.position = originPos;
    }
    #endregion ������ UI

    public void DeadPlayer()
    {
        // �÷��̾� �׾����� �гζ��� �ֱٿ� �����ߴ� ��ġ�� �� ���� �� ä��� �ű�� (�� �ٽ� �ҷ�����)
        _playerDeath.AnnouncePlayerDeath();
    }
}