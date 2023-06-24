using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Rocket : SkillActive
{
    
    public GameObject objShoot;
    public GameObject objBoom;
    private Rigidbody2D target;
    Vector2 dir;
    public float timeEnd;

    public void OnEnable()
    {
        objBoom.transform.localScale = Vector3.one*2f;
    }

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        switch (state)
        {
            case E_SKILL_STATE.SHOOT:
                if (time > lifetime)
                {
                    state = E_SKILL_STATE.BOOM;
                    return;
                }
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
    }

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        transform.position = InGameManager.instance.manager_player.transform.position;
        target = GetNearestMonster();
        if (target == null)
            dir = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        else
            dir = target.position - rigid.position;
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

    void Move()
    {
        //if ((target.position - rigid.position).magnitude < 1)
        //{
        //    transform.position = target.position;
        //    state = E_SKILL_STATE.BOOM;
        //    return;
        //}
        Vector2 nextVec = dir * speed * Time.fixedDeltaTime;
        rigid.MovePosition(new Vector2(transform.position.x + nextVec.x, transform.position.y + nextVec.y));
    }

    void Boom()
    {
        PlaySkillAudio(E_SKILL_STATE.BOOM);
        time = 0;
        state = E_SKILL_STATE.END;
        objShoot.SetActive(false);
        objBoom.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Monster")
            return;
        if (collision.GetComponent<MonsterBase>() == false)
            return;

        state = E_SKILL_STATE.BOOM;
    }
}
