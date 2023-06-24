using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Revolver : SkillActive
{
    
    public GameObject objShoot;
    public GameObject objBoom;
    Vector2 dir;
    public float timeEnd;
    public float rotMultiple;

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        switch (state)
        {
            case E_SKILL_STATE.SHOOT:
                Move();
                break;
            case E_SKILL_STATE.BOOM:
                Boom();
                break;
            case E_SKILL_STATE.END:
                if (time > timeEnd)
                {
                    isAlive = false;
                    gameObject.SetActive(false);
                    InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
                }
                return;
        }
        if (time > lifetime)
        {
            isAlive = false;
            gameObject.SetActive(false);
            InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
        }
    }

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        transform.position = InGameManager.instance.manager_player.transform.position;

        time = 0;
        transform.position = InGameManager.instance.manager_player.transform.position;

        dir = InGameManager.instance.manager_player.controller_playerMoving.lastMove;
        dir = Quaternion.Euler(0, 0, rotMultiple * (indexMultiple - multiple / 2f)) * dir;
        dir = dir.normalized;

        transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        state = E_SKILL_STATE.SHOOT;
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        objBoom.SetActive(false);
        objShoot.SetActive(true);
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

        state = E_SKILL_STATE.BOOM;
    }

    void Move()
    {
        Vector2 nextVec = dir * speed * Time.fixedDeltaTime;
        rigid.MovePosition(new Vector2(transform.position.x + nextVec.x, transform.position.y + nextVec.y));
    }

    void Boom()
    {
        time = 0;
        PlaySkillAudio(E_SKILL_STATE.BOOM);
        state = E_SKILL_STATE.END;
        objShoot.SetActive(false);
        objBoom.SetActive(true);
    }
}
