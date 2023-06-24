using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_QuantumBall : SkillActive
{
    public static Vector2 vecStart;
    private Vector2 dirVec;
    private int cntCurrent;
    public int cntMax = 15;
    public List<ParticleSystem> listParticle;

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        base.Init(_skillData);

        cntCurrent = 0;
        time = 0;
        transform.position = InGameManager.instance.manager_player.transform.position;

        if(indexMultiple == 0)
        {
            vecStart = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
        }
        gameObject.SetActive(true);
        if (indexMultiple == 0)
            PlaySkillAudio(E_SKILL_STATE.SHOOT);
        foreach (var item in listParticle)
        {
            item.Play();
        }
        isAlive = true;
        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
    }

    public void OnEnable()
    {
        dirVec = vecStart;
        transform.localEulerAngles = Vector3.zero;

        DOTween.To(() => transform.localEulerAngles
        , x => transform.localEulerAngles = x, new Vector3(0, 0, 360), .3f)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);

        DOTween.To(() => transform.localScale
        , x => transform.localScale = x, new Vector3(size, size, size) * 1.2f, 1f)
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
    public void Hit()
    {
        if (cntCurrent == 0)
        {
            cntCurrent++;
            dirVec = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
        }
    }

    private void CheckCanvasOut()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if ((pos.x < 0f && dirVec.x < 0) || (pos.x > 1f && dirVec.x > 0))
        {
            dirVec = new Vector2(-dirVec.x, dirVec.y);
            Hit();
        }
        if ((pos.y < 0f && dirVec.y < 0) || (pos.y > 1f && dirVec.y > 0))
        {
            dirVec = new Vector2(dirVec.x, -dirVec.y);
            Hit();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Monster")
            return;
        if (collision.GetComponent<MonsterBase>() == false)
            return;
        collision.GetComponent<MonsterBase>().Damage(type, (int)damage, knockBack, color_PaintMonster);

        PlaySkillAudio(E_SKILL_STATE.BOOM);
        Hit();
        cntCurrent++;
        if (dirVec.x > dirVec.y)
            dirVec = new Vector2(-dirVec.x, dirVec.y);
        else
        {
            dirVec = new Vector2(dirVec.x, -dirVec.y);
        }
    }
}
