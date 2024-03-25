using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public static string _nextScene;
    public Image _loadingFillImg;

    private void Start()
    {
        StartCoroutine(LoadSceneCor());
    }

    // �Ű������� �ҷ��� �� �̸����޴´�, �ε����� �ҷ����� �ε��� �ش罺ũ��Ʈ�� Start�Լ������� �Ű��������� �´� ���� �ҷ��´�.
    public static void LoadScene(string sceneName)
    {
        _nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private IEnumerator LoadSceneCor()
    {
        // �񵿱�� _nextScene�� �ҷ��´�.
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(_nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while (!op.isDone)
        {
            // �ε��� ä���ֱ�
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                _loadingFillImg.fillAmount = Mathf.Lerp(_loadingFillImg.fillAmount, op.progress, timer);
                if (_loadingFillImg.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }

            // �ε��� �� �Ǿ����� �߰������� UI���� ���� �ҷ��´�.
            else
            {
                _loadingFillImg.fillAmount = Mathf.Lerp(_loadingFillImg.fillAmount, 1f, timer);
                if (_loadingFillImg.fillAmount == 1.0f)
                {
                    yield return new WaitForSeconds(0.5f);

                    op.allowSceneActivation = true;
                    SceneManager.LoadScene("UI", LoadSceneMode.Additive);
                    yield break;
                }
            }
        }

    }
}