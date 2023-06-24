using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Defender : SkillActive
{
    public static int number;
    public bool loopSound;
    public float delaySound = .5f;
    public int index;

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        switch (state)
        {
            case E_SKILL_STATE.SHOOT:
                OnScale();
                break;
        }
        Rotate();
        if (time > lifetime)
        {
            isAlive = false;
            gameObject.SetActive(false);
            InGameManager.instance.manager_playerSkill.InitCoolTime(type);
            InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
        }
    }
    private void OnEnable()
    {
        if (index != 0)
            return;

        StartCoroutine(SoundLoop());
    }
    public IEnumerator SoundLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(delaySound);
            PlaySkillAudio(E_SKILL_STATE.SHOOT);
        }
    }
    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        transform.SetParent(InGameManager.instance.manager_playerSkill.objDefender.transform.GetChild(indexMultiple));
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.zero;
        state = E_SKILL_STATE.SHOOT;
        index = indexMultiple;

        gameObject.SetActive(true);
        isAlive = true;

        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }

    public void Rotate()
    {
        transform.eulerAngles = Vector3.zero;
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
        PlaySkillAudio(E_SKILL_STATE.BOOM);
        collision.GetComponent<MonsterBase>().Damage(type, (int)damage, knockBack, color_PaintMonster);
    }
}