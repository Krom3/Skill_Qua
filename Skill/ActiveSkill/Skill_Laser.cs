using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Laser : SkillActive
{
    
    public float timeEnd;
    public List<Rigidbody2D> listSkill;
    private float dir;
    private int cntTurn;

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        switch (state)
        {
            case E_SKILL_STATE.SHOOT:
                Move();
                break;
            case E_SKILL_STATE.BOOM:
                Rotate();
                break;
        }
        if (time > lifetime)
        {
            isAlive = false;
            gameObject.SetActive(false);
        }
    }

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        transform.position = InGameManager.instance.manager_player.transform.position;
        foreach (var item in listSkill)
            item.transform.position = InGameManager.instance.manager_player.transform.position;

        dir = 1;
        cntTurn = 1;
        state = E_SKILL_STATE.SHOOT;
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        gameObject.SetActive(true);
        isAlive = true;

        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Monster")
            return;
        if (collision.GetComponent<MonsterBase>() == false)
            return;
        PlaySkillAudio(E_SKILL_STATE.BOOM);
        collision.GetComponent<MonsterBase>().Damage(type, (int)damage, knockBack, color_PaintMonster);
    }
    void Move()
    {
        for (int i = 0; i < listSkill.Count; i++)
        {
            Vector2 nextVec;
            switch (i)
            {
                case 0:
                    nextVec = new Vector2(dir, 0.2f) * speed * Time.fixedDeltaTime;
                    listSkill[i].MovePosition(listSkill[i].position + nextVec);
                    break;
                case 1:
                    nextVec = new Vector2(-dir, 0.2f) * speed * Time.fixedDeltaTime;
                    listSkill[i].MovePosition(listSkill[i].position + nextVec);
                    break;
                case 2:
                    nextVec = new Vector2(dir, -0.2f) * speed * Time.fixedDeltaTime;
                    listSkill[i].MovePosition(listSkill[i].position + nextVec);
                    break;
                case 3:
                    nextVec = new Vector2(-dir, -0.2f) * speed * Time.fixedDeltaTime;
                    listSkill[i].MovePosition(listSkill[i].position + nextVec);
                    break;
            }
        }
        //if(Mathf.Abs(transform.position.x - listSkill[0].position.x) >3f)
        if (time>timeEnd*cntTurn)
        {
            state = E_SKILL_STATE.BOOM;
            PlaySkillAudio(E_SKILL_STATE.SHOOT);
        }
    }
    void Rotate()
    {
        cntTurn+=2;
        state = E_SKILL_STATE.SHOOT;
        dir *= -1;
    }
}
