using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Molotov : SkillActive
{
    
    private Vector3 target;

    [Header("Effect")]
    public GameObject objBoom;
    public ParticleSystem particleShoot;
    public List<ParticleSystem> listParticle;

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        var listTrnMolotov = InGameManager.instance.manager_playerSkill.listTrnMolotov;
        target = listTrnMolotov[indexMultiple].position;
        transform.position = InGameManager.instance.manager_player.transform.position;
        state = E_SKILL_STATE.SHOOT;
        objBoom.SetActive(false);

        gameObject.SetActive(true);
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        particleShoot.Play();
        isAlive = true;

        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }

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
        }
        if (time > lifetime)
        {
            isAlive = false;
            gameObject.SetActive(false);
            InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
        }
    }
    void Move()
    {
        if ((new Vector2(target.x,target.y) - rigid.position).magnitude < .2f)
        {
            rigid.position = target;
            state = E_SKILL_STATE.BOOM;
            return;
        }
        Vector2 nextVec = speed * Time.fixedDeltaTime * (new Vector2(target.x, target.y) - rigid.position).normalized;
        rigid.MovePosition(rigid.position + nextVec);
        transform.localEulerAngles -= new Vector3(0,0,10f);
    }
    void Boom()
    {
        time = 0;
        state = E_SKILL_STATE.END;
        PlaySkillAudio(E_SKILL_STATE.BOOM);
        objBoom.SetActive(true);
        foreach (var item in listParticle)
            item.Play();
    }

}
