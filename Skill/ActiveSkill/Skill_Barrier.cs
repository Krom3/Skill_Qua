using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Barrier : SkillActive
{
    public float delayAttack;

    private void Reset()
    {
        type = E_SKILL_TYPE.쿠아의정화;
    }

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        time = 0;
        transform.position = InGameManager.instance.manager_player.transform.position;

        InGameManager.instance.manager_playerSkill.objBarrier.SetActive(true);
        gameObject.SetActive(true);
        PlaySkillAudio(E_SKILL_STATE.SHOOT);

        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }

    private void FixedUpdate()
    {
        Move();
        CheckLifetime();
    }

    private void Move()
    {
        transform.position = InGameManager.instance.manager_player.transform.position;
    }
    private void CheckLifetime()
    {
        time += Time.fixedDeltaTime;
        if (time > lifetime)
        {
            isAlive = false;
            gameObject.SetActive(false);
        }
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.tag != "Monster")
    //        return;

    //    if (hit == true)
    //        return;
    //    hit = true;

    //    var monster = collision.GetComponent<MonsterBase>();
    //    monster.Damage(type,(int)damage);
    //}
}