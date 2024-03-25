using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Ϲ� ���� ��ȯ�� ������Ʈ Ǯ
/// </summary>
public class MonsterSummonPool : MonoBehaviour
{
    public GameObject[] _monsterPrefabs;
    public ObjectPooling _projectilePool;

    /// <summary>
    /// ���͸� ��ȯ�Ҷ� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="count"></param>
    public void SummonMonster(int count)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (count == 0)
                break;
            if (transform.GetChild(i).gameObject.activeSelf)
                continue;

            transform.GetChild(i).gameObject.SetActive(true);
            transform.GetChild(i).GetComponent<MonsterSummon>().StartSummon();
            count--;
        }

        if (count != 0)
        {
            InstantiateRemain(count);
        }
    }

    /// <summary>
    /// ��Ȱ��ȭ�� �ڽ� ������Ʈ�� �������ִ� ���ͺ��� ���� ���� ��ȯ�ؾ��Ҷ� ȣ���ϴ� �Լ�
    /// </summary>
    /// <param name="count"></param>
    private void InstantiateRemain(int count)
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            if (count == 0)
                return;

            // ���� ������Ʈ ����, ��ġ�� ����
            int randomValue = Random.Range(0, 2);
            GameObject monster = Instantiate(_monsterPrefabs[randomValue]);
            Transform monsterTr = monster.GetComponent<Transform>();

            monster.gameObject.SetActive(false);
            monsterTr.position = transform.GetChild(i % 4).transform.position;
            monsterTr.parent = this.transform;
            monsterTr.SetAsLastSibling();

            // ���Ͱ� ���Ÿ���� �߻�ü ������Ʈ Ǯ�� ����
            LongRange longRange = monster.GetComponentInChildren<LongRange>(true);
            if (longRange != null)
                longRange._projectilePool = _projectilePool;

            // ���� ������Ʈ ���ָ鼭 ��ȯ
            count--;
            monster.gameObject.SetActive(true);
        }
    }
}