using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Skill_Ball : SkillActive
{
    private Vector2 dirVec;
    private int cntCurrent;
    public int cntMax = 4;

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        cntCurrent = 0;
        time = 0;
        transform.position = InGameManager.instance.manager_player.transform.position;

        gameObject.SetActive(true);
        PlaySkillAudio(E_SKILL_STATE.SHOOT);
        isAlive = true;
        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }

    public void OnEnable()
    {
        dirVec = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));

        DOTween.To(() => transform.localEulerAngles
        , x => transform.localEulerAngles = x, new Vector3(0, 0, 360), 1f)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);

        DOTween.To(() => transform.localScale
        , x => transform.localScale = x, new Vector3(size,size,size)*1.2f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InCubic);
    }

    private void FixedUpdate()
    {
        Move();
        CheckCanvasOut();
        CheckHitMaxCount();
        CheckLifetime();
    }

    private void Move()
    {
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }
    private void CheckHitMaxCount()
    {
        if (cntCurrent < cntMax)
            return;

        isAlive = false;
        gameObject.SetActive(false);
    }
    private void CheckLifetime()
    {
        time += Time.fixedDeltaTime;
        if (time > lifetime)
        {
            isAlive = false;
            gameObject.SetActive(false);
        }
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
    }
    //private void CheckCanvasOut()
    //{
    //    Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

    //    if ((pos.x < 0.05f && dirVec.x < 0)||(pos.x > 0.95f && dirVec.x > 0))
    //    {
    //        dirVec = new Vector2(-dirVec.x, Random.Range(-100, 100));
    //    }
    //    if ((pos.y < 0.05f && dirVec.y < 0) || (pos.y > 0.95f && dirVec.y > 0))
    //    {
    //        dirVec = new Vector2(Random.Range(-100, 100), -dirVec.y);
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Monster")
            return;
        if (collision.GetComponent<MonsterBase>() == false)
            return;
        collision.GetComponent<MonsterBase>().Damage(type, (int)damage, knockBack, color_PaintMonster);

        if (collision.tag == "Monster")
            PlaySkillAudio(E_SKILL_STATE.BOOM);
        if (collision.tag == "Monster"
            || collision.tag == "벽") //TODO.
        {
            cntCurrent++;
            if (dirVec.x>dirVec.y)
                dirVec = new Vector2(-dirVec.x, dirVec.y);
            else
            {
                dirVec = new Vector2(dirVec.x, -dirVec.y);
            }
            // Debug.LogError(-collision.ClosestPoint(transform.position));
            // dirVec = caclulateReflect(rigid.velocity, collision.ClosestPoint(transform.position).normalized);
        }
    }
}
