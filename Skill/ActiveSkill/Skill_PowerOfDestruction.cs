using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_PowerOfDestruction : SkillActive
{
    public GameObject objShoot;
    public GameObject objBoom;
    private Rigidbody2D target;
    Vector2 dir;
    public float timeEnd;

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
            state = E_SKILL_STATE.BOOM;
        }
    }


    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        transform.position = InGameManager.instance.manager_player.transform.position;
        target = null;
        target = GetNearestMonster();
        dir = Vector2.zero;
        if (target == null)
        {
            while (dir == Vector2.zero)
            {
                dir = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
            }
        }
        else
        {
            dir = target.position- rigid.position;
        }
        dir = dir.normalized;

        state = E_SKILL_STATE.SHOOT;
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        objBoom.SetActive(false);
        objShoot.SetActive(true);
        Rotate();
        gameObject.SetActive(true);
        isAlive = true;

        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }

    void Move()
    {
        Vector2 nextVec = dir * speed * Time.fixedDeltaTime;
        rigid.MovePosition(new Vector2(transform.position.x + nextVec.x, transform.position.y + nextVec.y));
    }
    private void Rotate()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        transform.eulerAngles += new Vector3(0, 0, -180);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Monster")
            return;
        if (collision.GetComponent<MonsterBase>() == false)
            return;
        if (state == E_SKILL_STATE.SHOOT)
            state = E_SKILL_STATE.BOOM;
        return;
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
