using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Skill_Durian : SkillActive
{
    private Vector2 dirVec;

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        time = 0;
        transform.position = InGameManager.instance.manager_player.transform.position;
        dirVec = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));

        gameObject.SetActive(true);
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        isAlive = true;

        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }
    private void FixedUpdate()
    {
        Move();
        CheckCanvasOut();
        CheckLifetime();
    }

    private void Move()
    {
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    private void CheckCanvasOut()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if ((pos.x < 0f && dirVec.x < 0) || (pos.x > 1f && dirVec.x > 0))
        {
            dirVec = new Vector2(-dirVec.x, dirVec.y);
        }
        if ((pos.y < 0f && dirVec.y < 0) || (pos.y > 1f && dirVec.y > 0))
        {
            dirVec = new Vector2(dirVec.x, -dirVec.y);
        }
        //Vector3 pos = InGameManager.instance.manager_player.transform.position;

        //if ((transform.position.x < pos.x - 3f && dirVec.x < 0) || (transform.position.x > pos.x + 3f && dirVec.x > 0))
        //{
        //    dirVec = new Vector2(-dirVec.x, dirVec.y);
        //}
        //if ((transform.position.y < pos.y - 3f && dirVec.y < 0) || (transform.position.y > pos.y + 3f && dirVec.y > 0))
        //{
        //    dirVec = new Vector2(dirVec.x, -dirVec.y);
        //}
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
