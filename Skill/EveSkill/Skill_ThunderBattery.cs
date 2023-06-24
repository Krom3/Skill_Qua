using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_ThunderBattery : SkillActive
{
    
    public GameObject objBoom;
    public Vector2 posTarget;
    public List<ParticleSystem> listParticle;
    public GameObject objSpriteAnim;
    public float timeEnd;
    public Rigidbody2D target;
    private bool isHit;

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

        target = null;
        target = GetRandomMonster();
        if (target == null)
        {
            posTarget = InGameManager.instance.manager_player.transform.position;
            posTarget += new Vector2(Random.Range(-2, 3), Random.Range(-6, 6));
        }
        else
            posTarget = target.position;

        posTarget += new Vector2(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f));
        isHit = false;
        Vector3 startPos = posTarget + new Vector2(5, 5);
        transform.position = startPos;

        state = E_SKILL_STATE.SHOOT;
        GetComponent<SpriteRenderer>().enabled = true;
        objSpriteAnim.SetActive(false);
        objBoom.SetActive(false);
        gameObject.SetActive(true);
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        isAlive = true;

        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }

    void Move()
    {
        if ((posTarget - rigid.position).magnitude < .5f)
        {
            rigid.position = posTarget;
            state = E_SKILL_STATE.BOOM;
            return;
        }
        Vector2 nextVec = (posTarget - rigid.position).normalized * speed * Time.fixedDeltaTime;
        transform.position = new Vector2(transform.position.x + nextVec.x, transform.position.y + nextVec.y);
    }

    void Boom()
    {
        time = 0;
        GetComponent<SpriteRenderer>().enabled = false;
        PlaySkillAudio(E_SKILL_STATE.BOOM);
        state = E_SKILL_STATE.END;
        objSpriteAnim.SetActive(true);
        objBoom.SetActive(true);
        if (isHit == true)
            return;
        foreach (var item in GetNearestMonsters(size))
        {
            if (item.transform.GetComponent<MonsterBase>() == true)
                item.transform.GetComponent<MonsterBase>().Damage(type, damage, knockBack, color_PaintMonster);
        }
        foreach (var item in listParticle)
        {
            item.Play();
        }
    }
}