using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Brick : SkillActive
{
    
    public float timeEnd;
    private int cntMax;
    public float force = 4f;

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        switch (state)
        {
            case E_SKILL_STATE.SHOOT:
                Move();
                if (time > timeEnd)
                {
                    state = E_SKILL_STATE.BOOM;
                    return;
                }
                break;
        }
        if (time > lifetime)
        {
            isAlive = false;
            gameObject.SetActive(false);
        }
    }
    float forceX;
    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        cntMax = 0;

        transform.position = InGameManager.instance.manager_player.transform.position;
        transform.eulerAngles = new Vector3(0, 0, 0);
        state = E_SKILL_STATE.SHOOT;
        gameObject.SetActive(true);
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        isAlive = true;
        forceX =1f;
        if (Random.Range(0,100)>50)
        forceX =-1f;
        rigid.AddForce(new Vector2(0, force) * speed, ForceMode2D.Impulse);
        transform.DORotate(new Vector3(0, 0, Random.Range(-15,15)), 0.5f).SetEase(Ease.OutCubic);
        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }

    void Move()
    {
        rigid.AddForce(new Vector2(forceX, 0)* 0.07f, ForceMode2D.Impulse);
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
        PlaySkillAudio(E_SKILL_STATE.BOOM);
        collision.GetComponent<MonsterBase>().Damage(type, (int)damage, knockBack, color_PaintMonster);
        cntMax++;
        CheckHitMaxCount();
    }
}