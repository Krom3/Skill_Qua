using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_BlueSlash : SkillActive
{
    
    public float timeEnd;
    public List<Rigidbody2D> listSkill;
    public List<Transform> listTransfromStart;
    public Transform trnRotate;
    public List<TrailRenderer> listTrail;

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        switch (state)
        {
            case E_SKILL_STATE.SHOOT:
                if (time > timeEnd)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        listSkill[i].transform.SetParent(trnRotate);
                    }
                    state = E_SKILL_STATE.BOOM;
                    PlaySkillAudio(E_SKILL_STATE.BOOM);
                }
                break;
            case E_SKILL_STATE.BOOM:
                Move();
                break;
        }
        if (time > lifetime)
        {
            isAlive = false;
            gameObject.SetActive(false);
        }
    }

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        transform.position = InGameManager.instance.manager_player.transform.position;
        for (int i = 0; i < 8; i++)
        {
            listSkill[i].transform.SetParent(listTransfromStart[i]);
            listSkill[i].transform.localPosition = Vector3.zero;
            listSkill[i].transform.SetParent(transform);
            listTrail[i].Clear();
        }
        state = E_SKILL_STATE.SHOOT;
        gameObject.SetActive(true);
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
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
        collision.GetComponent<MonsterBase>().Damage(type, (int)damage, knockBack, color_PaintMonster);
    }
    void Move()
    {
        for (int i = 0; i < listSkill.Count; i++)
        {
            listSkill[i].transform.position += (transform.position - listSkill[i].transform.position).normalized * speed * Time.fixedDeltaTime; ;
        }
    }
}
