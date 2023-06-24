using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DancingCube : SkillActive
{
    public List<Transform> listTrnChild;
    public Transform trnRootChild1;
    public Transform trnRootChild2;
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
    private void OnEnable()
    {
        StartCoroutine(ChangeChildMoveDir());
    }
    IEnumerator ChangeChildMoveDir()
    {
        int dir = 2;
        while (true)
        {
            yield return new WaitForSeconds(1f);
            listTrnChild[0].localPosition -= new Vector3(0, 0, dir);
            listTrnChild[1].localPosition += new Vector3(0, 0, dir);
            listTrnChild[0].localPosition -= new Vector3(0, 0, dir);
            dir *= -1;
        }
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

        if ((pos.x < 0.2f && dirVec.x < 0) || (pos.x > 0.8f && dirVec.x > 0))
        {
            dirVec = new Vector2(-dirVec.x, dirVec.y);
        }
        if ((pos.y < 0.2f && dirVec.y < 0) || (pos.y > 0.8f && dirVec.y > 0))
        {
            dirVec = new Vector2(dirVec.x, -dirVec.y);
        }
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