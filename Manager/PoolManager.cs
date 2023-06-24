using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs_Monster;
    // [NamedArray(typeof(E_SKILL_TYPE))]
    public GameObject[] prefabs_Skill;
    // [NamedArray(typeof(E_DropItem_Type))]
    public GameObject[] prefabs_DropItem;
    public GameObject[] prefabs_DamageTxt;
    public GameObject[] prefabs_MonsterWeapon;
    List<GameObject>[] pools_Monster;
    List<GameObject>[] pools_Skill;
    List<GameObject>[] pools_DropItem;
    List<GameObject>[] pools_DamageTxt;
    List<GameObject> pools_MonsterWeapon;

    void Awake()
    {
        pools_Monster = new List<GameObject>[prefabs_Monster.Length];
        pools_Skill = new List<GameObject>[prefabs_Skill.Length];
        pools_DropItem = new List<GameObject>[prefabs_DropItem.Length];
        pools_DamageTxt = new List<GameObject>[prefabs_DamageTxt.Length];
        pools_MonsterWeapon = new List<GameObject>();

        for (int index = 0; index < pools_Monster.Length; index++)
            pools_Monster[index] = new List<GameObject>();

        for (int index = 0; index < pools_Skill.Length; index++)
            pools_Skill[index] = new List<GameObject>();

        for (int index = 0; index < pools_DropItem.Length; index++)
            pools_DropItem[index] = new List<GameObject>();

        for (int index = 0; index < pools_DamageTxt.Length; index++)
            pools_DamageTxt[index] = new List<GameObject>();
    }

    public GameObject GetMonster()
    {
        GameObject select = null;

        foreach (GameObject item in pools_Monster[0])
        {
            if (item.activeSelf == false)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(prefabs_Monster[0], transform);
            pools_Monster[0].Add(select);
        }

        return select;
    }

    public GameObject GetMonster(int _index)
    {
        GameObject select = null;

        foreach (GameObject item in pools_Monster[_index])
        {
            if (item == null)
                continue;

            if (item.activeSelf == false)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(prefabs_Monster[_index], transform);
            pools_Monster[_index].Add(select);
        }

        return select;
    }

    public List<GameObject>[] GetAllMonsters()
    {
        return pools_Monster;
    }

    public void DestoryMonstersAndWeapons()
    {
        foreach (List<GameObject> list in pools_Monster)
        {
            if (list == null)
                continue;
            if (list.Count == 0)
                continue;

            foreach (GameObject item in list)
            {
                if (item == null)
                    continue;

                if (item.activeInHierarchy == false)
                    Destroy(item);
            }
        }

        foreach (GameObject item in pools_MonsterWeapon)
        {
            if (item == null)
                continue;

            if (item.activeInHierarchy == false)
                Destroy(item);
        }
    }

    public List<GameObject> GetDropItemsByType(E_DropItem_Type _type)
    {
        return pools_DropItem[(int)_type];
    }

    public void DestroyDropItemsByType(E_DropItem_Type _type)
    {
        foreach (GameObject item in pools_DropItem[(int)_type])
        {
            if (item == null)
                continue;

            if (item.activeInHierarchy == false)
                Destroy(item);
        }
    }

    public SkillActive GetSkill(E_SKILL_TYPE _type)
    {
        GameObject select = null;
        SkillActive skill = null;
        foreach (GameObject item in pools_Skill[(int)_type])
        {
            if (item == null)
                continue;

            skill = item.GetComponent<SkillActive>();

            switch (_type)
            {
                case E_SKILL_TYPE.회전큐브:
                case E_SKILL_TYPE.쿠라클A:
                case E_SKILL_TYPE.쿠라클B:
                case E_SKILL_TYPE.댄싱큐브:
                case E_SKILL_TYPE.슈퍼쿠라클:
                    if (skill.isAlive == true)
                        return null;
                    else
                        break;
            }
            if (skill.isAlive == false)
            {
                select = item;
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(prefabs_Skill[(int)_type], transform);
            skill = select.GetComponent<SkillActive>();
            pools_Skill[(int)_type].Add(select);
        }
        skill.isAlive = true;

        return skill;
    }

    public GameObject GetDropItem(E_DropItem_Type _type)
    {
        GameObject select = null;

        foreach (GameObject item in pools_DropItem[(int)_type])
        {
            if (item == null)
                continue;

            if (item.activeSelf == false)
            {
                if (_type == E_DropItem_Type.Exp)
                {
                    if (item.GetComponent<ExpItem>() == false)
                        continue;
                    else
                    {
                        if (item.GetComponent<ExpItem>().isGet == true)
                        {
                            select = item;
                            select.SetActive(true);
                            break;
                        }
                        else
                            continue;
                    }
                }

                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(prefabs_DropItem[(int)_type], transform);
            pools_DropItem[(int)_type].Add(select);
        }

        return select;
    }
    public GameObject GetDamageText(float _damage,Transform _trn)
    {
        GameObject select = null;
        int index = 0;
        if (_damage >= 1000)
            index++;
        if (_damage >= 100000)
            index++;
        foreach (GameObject item in pools_DamageTxt[index])
        {
            if (item.activeSelf == false)
            {
                select = item;
                select.GetComponent<CartoonFX.CFXR_ParticleText>().UpdateText(_damage.ToString());
                select.transform.position = _trn.position;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(prefabs_DamageTxt[index], transform);
            pools_DamageTxt[index].Add(select);
            select.GetComponent<CartoonFX.CFXR_ParticleText>().UpdateText(_damage.ToString());
            select.transform.position = _trn.position;
            select.SetActive(true);
        }

        return select;
    }
    public GameObject GetMonsterWeapon(E_Monster_Type _type)
    {
        GameObject select = null;

        if (pools_MonsterWeapon.Count > 0)
        {
            foreach (GameObject item in pools_MonsterWeapon)
            {
                if (item == null)
                    continue;
                if (item.GetComponent<MonsterBulletBase>() == false)
                    continue;
                if (item.GetComponent<MonsterBulletBase>().monsterType != _type)
                    continue;

                if (item.activeSelf == false)
                {
                    select = item;
                    select.SetActive(true);
                    break;
                }
            }
        }

        if (select == null)
        {
            foreach (GameObject item in prefabs_MonsterWeapon)
            {
                if (item.GetComponent<MonsterBulletBase>().monsterType == _type)
                    select = Instantiate(item, transform);
            }

            if (select != null)
            {
                pools_MonsterWeapon.Add(select);
                select.SetActive(true);
            }
        }

        return select;
    }

    public void ClearMonster(int _index)
    {
        foreach (GameObject item in pools_Monster[_index])
        {
            if (item == null)
                continue;

            item.SetActive(false);
        }
    }

    public void ClearSkill(E_SKILL_TYPE _type)
    {
        foreach (GameObject item in pools_Skill[(int)_type])
        {
            if (item == null)
                continue;

            item.GetComponent<SkillActive>().isAlive = false;
            item.SetActive(false);
        }
    }

    public void ClearDropItem(int _index)
    {
        foreach (GameObject item in pools_DropItem[_index])
        {
            if (item == null)
                continue;

            item.SetActive(false);
        }
    }

    public void ClearDamageText(int _index)
    {
        foreach (GameObject item in pools_DamageTxt[_index])
        {
            if (item == null)
                continue;

            item.SetActive(false);
        }
    }

    public void ClearMonsterWeapon()
    {
        foreach (GameObject item in pools_MonsterWeapon)
        {
            if (item == null)
                continue;

            item.GetComponent<MonsterBulletBase>().isAlive = false;
            item.SetActive(false);
        }
    }

    public void ClearAll()
    {
        for (int index = 0; index < pools_Monster.Length; index++)
        {
            if (pools_Monster[index] == null)
                continue;
            if (pools_Monster[index].Count == 0)
                continue;

            foreach (GameObject item in pools_Monster[index])
            {
                if (item == null)
                    continue;

                item.SetActive(false);
            }
        }

        for (int index = 0; index < pools_Skill.Length; index++)
        {
            if (pools_Skill[index] == null)
                continue;
            if (pools_Skill[index].Count == 0)
                continue;

            foreach (GameObject item in pools_Skill[index])
            {
                if (item == null)
                    continue;

                item.GetComponent<SkillActive>().isAlive = false;
                item.SetActive(false);
            }
        }

        for (int index = 0; index < pools_DropItem.Length; index++)
        {
            if (pools_DropItem[index] == null)
                continue;
            if (pools_DropItem[index].Count == 0)
                continue;

            foreach (GameObject item in pools_DropItem[index])
            {
                if (item == null)
                    continue;

                item.SetActive(false);
            }
        }

        for (int index = 0; index < pools_DamageTxt.Length; index++)
        {
            if (pools_DamageTxt[index] == null)
                continue;
            if (pools_DamageTxt[index].Count == 0)
                continue;

            foreach (GameObject item in pools_DamageTxt[index])
            {
                if (item == null)
                    continue;

                item.SetActive(false);
            }
        }

        ClearMonsterWeapon();
    }
}
