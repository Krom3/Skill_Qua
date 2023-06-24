using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Dumbbell : SkillActive
{
    
    public Transform trnImg;
    public float timeEnd;
    private int cntMax;

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        Move();
        CheckHitMaxCount();
        if (time > lifetime)
        {
            isAlive = false;
            gameObject.SetActive(false);
        }
    }

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        cntMax = 0;

        transform.position = InGameManager.instance.manager_player.transform.position;
        transform.eulerAngles = new Vector3(0, 0, 0);
        state = E_SKILL_STATE.SHOOT;
        gameObject.SetActive(true);
        if(indexMultiple == 0)
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        isAlive = true;
        if (indexMultiple < 3)
        {
            trnImg.GetComponent<SpriteRenderer>().flipX = false;
            trnImg.GetComponent<SpriteRenderer>().flipY = false;
        }
        else if (indexMultiple == 3)
        {
            trnImg.GetComponent<SpriteRenderer>().flipX = false;
            trnImg.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            trnImg.GetComponent<SpriteRenderer>().flipX = true;
            trnImg.GetComponent<SpriteRenderer>().flipY = false;
        }
        transform.rotation = Quaternion.Euler(0, 0, indexMultiple * 60);
        trnImg.eulerAngles = Vector3.zero;
        rigid.AddForce(transform.forward * speed, ForceMode2D.Impulse);
        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }

    void Move()
    {
        Vector2 nextVec = (transform.up) * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void CheckHitMaxCount()
    {
        if (cntMax < 5)
            return;

        isAlive = false;
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Monster")
            return;
        if (collision.GetComponent<MonsterBase>() == false)
            return;
        collision.GetComponent<MonsterBase>().Damage(type, (int)damage, knockBack, color_PaintMonster);

        PlaySkillAudio(E_SKILL_STATE.BOOM);
        if (collision.tag == "Monster"
            || collision.tag == "벽") //TODO.
        {
            cntMax++;
        }
    }
}
