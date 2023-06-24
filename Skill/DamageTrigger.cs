using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public enum E_DAMAGE_TYPE
    {
        SINGLE,//한번 공격하고 끝.
        SPLASH,//범위공격..
        MULTIPLE,//트리거엔터 여러명 공격.
        MULTIPLE_TARGETBYONE,//트리거엔터 여러명 + 한번 공격한 몬스터 공격안함
    }
    public E_DAMAGE_TYPE type;
    public SkillActive parent;
    public bool isUsed = false;
    public bool isDamageDelay = false;
    public float damageDelay = 1f;
    private List<string> listMonster;
    private List<GameObject> list_DelayMonster;
    private List<float> list_DelayDamage;
    private void Awake()
    {
        listMonster = new List<string>();
        list_DelayMonster = new List<GameObject>();
        list_DelayDamage = new List<float>();
    }
    private void OnEnable()
    {
        isUsed = false;
        listMonster.Clear();
        list_DelayMonster.Clear();
        list_DelayDamage.Clear();
    }

    private void FixedUpdate()
    {
        if (isDamageDelay == true)
            for (int i = 0; i < list_DelayDamage.Count; i++)
                list_DelayDamage[i] += Time.fixedDeltaTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Monster")
            return;
        if (collision.GetComponent<MonsterBase>() == false)
            return;

        if (isDamageDelay == true)
        {
            if (list_DelayMonster.Contains(collision.gameObject) == true)
            {
                int index = list_DelayMonster.IndexOf(collision.gameObject);
                if (index > list_DelayDamage.Count - 1)
                    return;

                if (list_DelayDamage[index] < damageDelay)
                    return;
                else
                    list_DelayDamage[index] = 0;
            }
        }

        collision.GetComponent<MonsterBase>().Damage(parent.type, (int)parent.damage, parent.knockBack, parent.color_PaintMonster);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isUsed == true)
            return;
        if (collision.tag != "Monster")
            return;
        if (collision.GetComponent<MonsterBase>() == false)
            return;
        if (isDamageDelay == true)
        {
            if (list_DelayMonster.Contains(collision.gameObject) == true)
            {
                int index = list_DelayMonster.IndexOf(collision.gameObject);
                if (index > list_DelayDamage.Count - 1)
                    return;

                if (list_DelayDamage[index] < damageDelay)
                    return;
                else
                    list_DelayDamage[index] = 0;
            }
            else
            {
                list_DelayMonster.Add(collision.gameObject);
                list_DelayDamage.Add(0);
            }
        }

        switch (type)
        {
            case  E_DAMAGE_TYPE.MULTIPLE_TARGETBYONE:
                if (listMonster.Contains(collision.gameObject.name) == true)
                    return;

                listMonster.Add(collision.gameObject.name);
                break;
        }
        collision.GetComponent<MonsterBase>().Damage(parent.type, (int)parent.damage, parent.knockBack, parent.color_PaintMonster);

        switch (type)
        {
            case E_DAMAGE_TYPE.SINGLE:
                isUsed = true;
                break;
        }
    }
}
