using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DarkDeer : SkillActive
{
    
    public Transform trnImage;
    public float TestturnSpd;
    private int num;
    public float timeEnd;
    public TrailRenderer trail;
    public ParticleSystem particle;

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        SetImageRotate();
        switch (state)
        {
            case E_SKILL_STATE.SHOOT:
                InitStart();
                break;
            case E_SKILL_STATE.BOOM:
                Move();
                if (time > timeEnd)
                {
                    isAlive = false;
                    gameObject.SetActive(false);
                    InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
                }
                break;
        }
    }

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        transform.position = InGameManager.instance.manager_player.transform.position;
        num = indexMultiple;

        state = E_SKILL_STATE.SHOOT;
        gameObject.SetActive(true);
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        isAlive = true;

        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }


    void InitStart()
    {
        trail.Clear();
        particle.Stop();
        var trnPlayer = InGameManager.instance.manager_player.transform;
        if (num == 0)
        {
            transform.localEulerAngles = Vector3.zero;
            transform.position = trnPlayer.position + new Vector3(.3f, 0);
        }
        if (num == 1)
        {
            transform.localEulerAngles = new Vector3(0, 0, 180);
            transform.position = trnPlayer.position - new Vector3(.3f, 0);
        }

        trail.Clear();
        state = E_SKILL_STATE.BOOM;
        particle.Play();
    }
    public float ff = 1f;
    public float gg = 0.7f;
    void Move()
    {
        Vector2 nextVec = transform.up * speed *(ff+time*gg)* Time.fixedDeltaTime;
        transform.eulerAngles += new Vector3(0, 0, TestturnSpd) * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
    void SetImageRotate()
    {
        trnImage.eulerAngles = Vector3.zero;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Monster")
            return;
        if (collision.GetComponent<MonsterBase>() == false)
            return;
        collision.GetComponent<MonsterBase>().Damage(type, (int)damage, knockBack, color_PaintMonster);
    }
}
