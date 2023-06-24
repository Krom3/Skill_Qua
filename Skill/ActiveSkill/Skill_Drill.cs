using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Drill : SkillActive
{
    public TrailRenderer trail;
    private Vector2 dirVec;

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        time = 0;
        transform.position = InGameManager.instance.manager_player.transform.position;
        gameObject.SetActive(true);
        trail.Clear();
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        isAlive = true;

        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }
    public void OnEnable()
    {
        dirVec = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
        CheckCanvasOut();
        CheckLifetime();
    }

    private void Move()
    {
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }
    private void Rotate()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.up, dirVec);
        transform.eulerAngles += new Vector3(0, 0, -150);
    }
    private void CheckLifetime()
    {
        time += Time.fixedDeltaTime;
        if (time > lifetime)
        {
            isAlive = false;
            gameObject.SetActive(false);
            InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
        }
    }

    private void CheckCanvasOut()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if ((pos.x < 0f && dirVec.x < 0) || (pos.x > 1f && dirVec.x > 0))
        {
            dirVec = new Vector2(-dirVec.x, dirVec.y);
            PlaySkillAudio(E_SKILL_STATE.SHOOT);
        }
        if ((pos.y < 0f && dirVec.y < 0) || (pos.y > 1f && dirVec.y > 0))
        {
            dirVec = new Vector2(dirVec.x, -dirVec.y);
            PlaySkillAudio(E_SKILL_STATE.SHOOT);
        }
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