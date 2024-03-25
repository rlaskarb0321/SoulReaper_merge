using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTeleport : MonoBehaviour, IInteractable
{
    [Header("=== Interact ===")]
    [SerializeField] private string _interactName;
    [SerializeField] private Transform _floatUIPos;

    [Header("=== Scene ===")]
    [SerializeField] private string _nextMap;
    [SerializeField] private Animator _sceneOutPanel;

    private Image _sceneOutPanelImg;
    private PlayerData _playerData;

    private void Awake()
    {
        _sceneOutPanelImg = _sceneOutPanel.gameObject.GetComponent<Image>();
    }

    public void Interact()
    {
        _sceneOutPanel.gameObject.SetActive(true);
        _sceneOutPanel.enabled = true;

        // ���⼭ CharacterDataPackage._characterData �� ���� �����ͼ� hp, mp ���� �����ϰ�
        // ���� ���� �̸������� �÷��̾��� ��ġ�� ȸ������ �ٲ��ְ�, CharacterDataPackage._characterData �� ���Ž�Ű��
        CharacterData.CData data = CharacterDataPackage._cDataInstance._characterData;
        switch (_nextMap)
        {
            case "Castle_Map":
                data._pos = new Vector3(ConstData.FROM_FOREST_TO_CASTLE_POS_X, ConstData.FROM_FOREST_TO_CASTLE_POS_Y, ConstData.FROM_FOREST_TO_CASTLE_POS_Z);
                data._rot = new Quaternion(ConstData.FROM_FOREST_TO_CASTLE_ROT_X, ConstData.FROM_FOREST_TO_CASTLE_ROT_Y, ConstData.FROM_FOREST_TO_CASTLE_ROT_Z, ConstData.FROM_FOREST_TO_CASTLE_ROT_W);
                data._mapName = "Castle_Map";
                break;

            case "LittleForest_Map":
                data._pos = new Vector3(ConstData.FROM_CASTLE_TO_FOREST_POS_X, ConstData.FROM_CASTLE_TO_FOREST_POS_Y, ConstData.FROM_CASTLE_TO_FOREST_POS_Z);
                data._rot = new Quaternion(ConstData.FROM_CASTLE_TO_FOREST_ROT_X, ConstData.FROM_CASTLE_TO_FOREST_ROT_Y, ConstData.FROM_CASTLE_TO_FOREST_ROT_Z, ConstData.FROM_CASTLE_TO_FOREST_ROT_W);
                data._mapName = "LittleForest_Map"; 
                break;
        }

        // �����ų �����Ϳ� ���簪�� ����
        data._currHP = _playerData._currHP;
        data._maxHP = _playerData._maxHP;
        data._currMP = _playerData._currMP;
        data._maxMP = _playerData._maxMP;

        // ������ ���� �ڷ���Ʈ ��Ŵ
        CharacterDataPackage._cDataInstance._characterData = data;
        DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");
        StartCoroutine(TeleportScene());
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractTextUI(UIScene._instance._interactUI, value, pos, _interactName);
    }

    private IEnumerator TeleportScene()
    {
        while (_sceneOutPanelImg.color.a < 1.0f)
        {
            if (_sceneOutPanelImg.color.a >= 1.0f)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        LoadingScene.LoadScene(_nextMap);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_playerData == null)
        {
            _playerData = other.GetComponent<PlayerData>();
        }
        if (Input.GetKey(KeyCode.F))
        {
            Interact();
            return;
        }

        SetActiveInteractUI(true);
    }
}