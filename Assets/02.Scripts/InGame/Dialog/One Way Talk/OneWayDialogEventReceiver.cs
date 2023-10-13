using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class OneWayDialogEventReceiver : MonoBehaviour, INotificationReceiver
{
    public DialogUI _oneWayDialogUI;

    private PlayableDirector _playable;
    private DialogMgr _dialogMgr;
    private float _letteringSpeed;
    private bool _isEndLine;
    private bool _isEndDialog;
    private float _lineEndTimer = 2.0f;

    private WaitForSeconds _letteringWS;
    private WaitUntil _endLineWU;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
        _dialogMgr = new DialogMgr();
        _endLineWU = new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || _lineEndTimer <= 0.0f);
    }

    private void Update()
    {
        if (_isEndLine && !_isEndDialog)
        {
            _lineEndTimer -= Time.deltaTime;
        }
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (!notification.id.Equals("One Way"))
            return;

        OneWayDialogMarker marker = notification as OneWayDialogMarker;
        IYOrNSelectOption selectOption = marker._dialogSelectOption.GetComponent<IYOrNSelectOption>();
        string[] lines = _dialogMgr.ParsingCSVLine(marker._oneWayDialog);

        _letteringSpeed = marker._letteringSpeed;
        _letteringWS = new WaitForSeconds(_letteringSpeed);
        PlayDialogType(lines);
    }

    private void PlayDialogType(string[] lines)
    {
        _oneWayDialogUI._activateUI.gameObject.SetActive(true);
        _playable.Pause();
        StartCoroutine(PlayOneWayDialog(lines));
    }

    private IEnumerator PlayOneWayDialog(string[] lines)
    {
        int index = 1;
        string speaker = "";
        string context = "";
        StringBuilder letterSb = new StringBuilder();
        _isEndDialog = false;

        // CSV 파일의 총 라인수만큼 반복
        while (index < lines.Length)
        {
            string[] line = lines[index].Split(','); // 라인들의 인덱스번째 라인을 ,로 나눔
            int letteringIndex = 0;

            speaker = line[0];
            context = _dialogMgr.ReplaceDialogSpecialChar(line[1]);
            letterSb.Clear();
            _isEndLine = false;

            // line 에서 화자가 ""가 아니면 바꿔줌
            if (speaker != null && !speaker.Equals("") && _oneWayDialogUI._speaker != null)
            {
                _oneWayDialogUI._speaker.text = speaker;
            }

            // 대화문을 한 글자씩 출력
            while (letteringIndex < context.Length)
            {
                // 대화문 도중 스페이스바 또는 마우스 좌클이 입력되면 바로 완성, 처음 몇 마디는 보여주게해야 자연스럽게 넘어가는 듯 보임
                if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && letteringIndex > 3)
                {
                    _oneWayDialogUI._context.text = context;
                    break;
                }

                letterSb.Append(context[letteringIndex]);
                _oneWayDialogUI._context.text = letterSb.ToString();

                letteringIndex++;
                yield return _letteringWS;
            }

            index++;
            _isEndLine = true;

            // 한 라인이 끝나고, 스페이스바 or 마우스좌클입력 또는 시간초가 지나가면 다음 라인으로
            yield return _endLineWU;
            _lineEndTimer = 2.0f;
        }

        _isEndDialog = true;
        _oneWayDialogUI._activateUI.gameObject.SetActive(false);
        _playable.Resume();
    }
}
