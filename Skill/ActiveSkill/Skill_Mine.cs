using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Mine : SkillActive
{
    
    public int cntMax;
    public static int cntCurr;
    public GameObject objShoot;
    public GameObject objBoom;
    public List<ParticleSystem> listParticle;
    public float timeEnd;
    private bool isHit;

    public void OnEnable()
    {
        transform.localScale = Vector3.zero;
        state = E_SKILL_STATE.SHOOT;
    }

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        switch (state)
        {
            case E_SKILL_STATE.SHOOT:
                OnScale();
                break;
            case E_SKILL_STATE.BOOM:
                Boom();
                break;
            case E_SKILL_STATE.END:
                if (time > timeEnd)
                {
                    cntCurr--;
                    isAlive = false;
                    gameObject.SetActive(false);
                    InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
                }
                return;
        }
        if (time > lifetime)
            state = E_SKILL_STATE.BOOM;
    }

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        transform.position = InGameManager.instance.manager_player.transform.position;
        transform.position += new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f));

        if (cntCurr >= cntMax)
            return;

        isHit = false;
        objShoot.SetActive(true);
        objBoom.SetActive(false);
        gameObject.SetActive(true);
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        cntCurr++;
        isAlive = true;

        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }
    public void OnScale()
    {
        transform.localScale += Vector3.one * Time.deltaTime;
        if (transform.localScale.x > size)
        {
            transform.localScale = Vector3.one * size;
            state = E_SKILL_STATE.NONE;
            return;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Monster")
            return;
        if (collision.GetComponent<MonsterBase>() == false)
            return;

        if (state == E_SKILL_STATE.NONE)
            state = E_SKILL_STATE.BOOM;
    }

    void Boom()
    {
        time = 0;
        state = E_SKILL_STATE.END;
        PlaySkillAudio(E_SKILL_STATE.BOOM);
        objShoot.SetActive(false);
        objBoom.SetActive(true);
        foreach (var item in listParticle)
            item.Play();
        if (isHit == true)
            return;
        foreach (var item in GetNearestMonsters(size))
        {
            if (item.transform.GetComponent<MonsterBase>() == true)
                item.transform.GetComponent<MonsterBase>().Damage(type, damage, knockBack, color_PaintMonster);
        }
        isHit = true;
    }
}
