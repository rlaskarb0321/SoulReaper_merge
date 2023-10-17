using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScene : MonoBehaviour
{
    [Header("=== Panel ===")]
    [SerializeField] private GameObject _pausePanel;
    public GameObject _letterPanel;
    [SerializeField] private List<GameObject> _currOpenPanel;

    [Header("=== Player ===")]
    [SerializeField] private PlayerData _stat;

    public enum ePercentageStat { Hp, Mp, }
    [Header("=== Hp & Mp ===")]
    [SerializeField] private Image _hpFill;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private Image _mpFill;
    [SerializeField] private TMP_Text _mpText;

    [Header("=== Map Name ===")]
    [SerializeField] private TMP_Text _mapName;

    [Header("=== Interact ===")]
    public GameObject _interactUI;

    private void Awake()
    {
        EditMapName();
        _currOpenPanel = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel();
        }
    }

    // UI패널을 키고, 켜져있는 ui들 리스트에 넣음
    public void SetUIPanelActive(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
        _currOpenPanel.Add(panel);
    }

    public void UpdateHPMP(ePercentageStat stat, float currValue, float maxValue)
    {
        switch (stat)
        {
            case ePercentageStat.Hp:
                _hpText.text = $"{currValue} / {maxValue}";
                _hpFill.fillAmount = currValue / maxValue;
                break;
            case ePercentageStat.Mp:
                _mpText.text = $"{currValue} / {maxValue}";
                _mpFill.fillAmount = currValue / maxValue;
                break;
        }
    }

    private void EditMapName()
    {

    }

    public void PausePanel()
    {
        // 켜져있는 UI가 있으면 끔
        if (_currOpenPanel.Count != 0)
        {
            int index = _currOpenPanel.Count - 1;

            _currOpenPanel[index].SetActive(false);
            _currOpenPanel.RemoveAt(index);
            return;
        }

        SetUIPanelActive(_pausePanel);
    }
}
