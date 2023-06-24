using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Boomerang : SkillActive
{
    
    private Rigidbody2D target;
    public float TestturnSpd;
    Vector2 dir;
    public float timeEnd;

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        Move();
        switch (state)
        {
            case E_SKILL_STATE.SHOOT:
                if (time > timeEnd)
                {
                    SetBoom();
                }
                break;
            case E_SKILL_STATE.BOOM:
                Boom();
                break;
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
        target = null;
        target = GetNearestMonster();
        dir = Vector2.zero;
        if (target == null)
        {
            while (dir == Vector2.zero)
            {
                dir = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
            }
            dir += rigid.position;
        }
        else
            dir = target.position;

        InitRotate();
        state = E_SKILL_STATE.SHOOT;
        gameObject.SetActive(true);
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        isAlive = true;

        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }

    void Move()
    {
        Vector2 nextVec = transform.up * speed * Time.fixedDeltaTime;
        rigid.MovePosition(new Vector2(transform.position.x + nextVec.x, transform.position.y + nextVec.y));
    }
    private void InitRotate()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Monster")
            return;
        if (collision.GetComponent<MonsterBase>() == false)
            return;
        collision.GetComponent<MonsterBase>().Damage(type, (int)damage, knockBack, color_PaintMonster);
    }
    float z;
    void SetBoom()
    {
        z = transform.eulerAngles.z;
        state = E_SKILL_STATE.BOOM;
        time = 0;
    }

    void Boom()
    {
         transform.eulerAngles += new Vector3(0, 0, TestturnSpd) * Time.fixedDeltaTime;
        if (z < 180)
        {
            if (transform.eulerAngles.z >= (z + 180))
                state = E_SKILL_STATE.END;
        }
        if (z > 180)
        {
            if (transform.eulerAngles.z <= 180 && transform.eulerAngles.z >= z-180)
                state = E_SKILL_STATE.END;
        }
    }
}
