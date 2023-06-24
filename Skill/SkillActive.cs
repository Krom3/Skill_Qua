using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActive : SkillBase
{
    public enum E_SKILL_STATE
    {
        NONE,
        SHOOT,
        BOOM,
        END
    }

    public E_SKILL_STATE state;
    public int multiple = 1;
    public float lifetime = 10;
    public float initLifeTime;
    public float cooltime = 12;
    public float size = 0.1f;
    public float initSize = 0;
    public float speed = 3;
    public float initSpeed = 0;
    public float damage = 10;
    public float range_scan = 10;
    public LayerMask layerTarget;
    protected float time;
    protected Rigidbody2D rigid;

    public bool shootForPlayerLastDir = false;
    public bool isAlive { get; set; } = false;

    [Header("KnockBack")]
    public float knockBack = 0.25f;
    public Color color_PaintMonster = Color.white;

    public virtual void Init(PlayerSkillManager.SkillData _skillData, int _multple = 1, int indexMultiple = 0)
    {
        if (rigid == null)
            rigid = GetComponent<Rigidbody2D>();

        gameObject.SetActive(false);
        time = 0;

        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type, false);

        SkillJson skillJson = _skillData.data;

        float powerWeight = float.Parse(skillJson.PowerWeight);
        float _damage = powerWeight * InGameManager.instance.manager_player.damage;
        if (InGameManager.instance.manager_playerSkill.list_skillPassive.Count > 0)
        {
            int findIndex = InGameManager.instance.manager_playerSkill.list_skillPassive.FindIndex(item => item.data.SkillId == "11");
            if (findIndex != -1)
                _damage *= float.Parse(InGameManager.instance.manager_playerSkill.list_skillPassive[findIndex].data.SkillWeight);
        }
        damage = _damage;

        size = initSize;
        if (InGameManager.instance.manager_playerSkill.list_skillPassive.Count > 0)
        {
            int findIndex = InGameManager.instance.manager_playerSkill.list_skillPassive.FindIndex(item => item.data.SkillId == "9");
            if (findIndex != -1)
                size = initSize * float.Parse(InGameManager.instance.manager_playerSkill.list_skillPassive[findIndex].data.SkillWeight);
        }
        //if (_skillJson.RangeWeight != null)
        //    size *= float.Parse(_skillJson.RangeWeight);

        transform.localScale = Vector3.one * size;

        lifetime = initLifeTime;
        if (InGameManager.instance.manager_playerSkill.list_skillPassive.Count > 0)
        {
            int findIndex = InGameManager.instance.manager_playerSkill.list_skillPassive.FindIndex(item => item.data.SkillId == "7");
            if (findIndex != -1)
                lifetime = initLifeTime * float.Parse(InGameManager.instance.manager_playerSkill.list_skillPassive[findIndex].data.SkillWeight);
        }

        speed = initSpeed;
        if (InGameManager.instance.manager_playerSkill.list_skillPassive.Count > 0)
        {
            int findIndex = InGameManager.instance.manager_playerSkill.list_skillPassive.FindIndex(item => item.data.SkillId == "2");
            if (findIndex != -1)
                speed = initSpeed * float.Parse(InGameManager.instance.manager_playerSkill.list_skillPassive[findIndex].data.SkillWeight);
        }

        // 토템 적용
        if (InGameManager.instance.GetTotemSkill().Count > 0)
        {
            foreach (TotemJson json in InGameManager.instance.GetTotemSkill())
            {
                if (json.OptionTrigger.Equals("PASSIVE") && json.OptionType.Equals("ATTACK"))
                {
                    string[] skills = json.SkillIds.Split(",");
                    foreach (string skillId in skills)
                        if (skillId.Trim().Equals(skillJson.SkillId))
                            _damage += _damage * Mathf.Abs(float.Parse(json.OptionPower));
                }
            }
        }

        _skillData.shotCount++;
        if (InGameManager.instance.GetTotemSkill().Count > 0)
        {
            foreach (TotemJson json in InGameManager.instance.GetTotemSkill())
            {
                if (json.OptionTrigger.Equals("SKILL_COUNT") && json.OptionType.Equals("ATTACK"))
                {
                    string[] skills = json.SkillIds.Split(",");
                    foreach (string skillId in skills)
                    {
                        if (skillId.Trim().Equals(skillJson.SkillId))
                            if (json.OptionTriggerConditionValue != null)
                                if (_skillData.shotCount % int.Parse(json.OptionTriggerConditionValue) == 0)
                                    if (json.OptionPower != null)
                                        _damage *= float.Parse(json.OptionPower);
                    }
                }
            }
        }
    }

    public Rigidbody2D GetNearestMonster()
    {
        Rigidbody2D target_nearest = null;

        var arrayMon = Physics2D.CircleCastAll(InGameManager.instance.manager_player.transform.position, range_scan, Vector2.zero, 0, layerTarget);
        if (arrayMon == null)
            return null;

        float diff = 100f;

        foreach (RaycastHit2D target in arrayMon)
        {
            if (target.transform.gameObject.activeInHierarchy == false)
                continue;

            Vector3 pos_player = InGameManager.instance.manager_player.transform.position;
            Vector3 pos_targer = target.transform.position;

            float curDiff = Vector3.Distance(pos_player, pos_targer);

            if (curDiff < diff)
            {
                diff = curDiff;
                target_nearest = target.transform.GetComponent<Rigidbody2D>();
            }
        }

        return target_nearest;
    }

    public Rigidbody2D GetRandomMonster()
    {
        Rigidbody2D target = null;

        var arrayMon = Physics2D.CircleCastAll(InGameManager.instance.manager_player.transform.position, range_scan, Vector2.zero, 0, layerTarget);
        if (arrayMon == null || arrayMon.Length == 0)
            return null;

        int indexRanMon = Random.Range(0, arrayMon.Length);
        target = arrayMon[indexRanMon].transform.GetComponent<Rigidbody2D>();

        return target;
    }
    public RaycastHit2D[] GetNearestMonsters(float _range)
    {
        var arrayMon = Physics2D.CircleCastAll(transform.position, _range, Vector2.zero, 0, layerTarget);
        if (arrayMon == null)
            return null;

        return arrayMon;
    }

    protected virtual void PlaySkillAudio(E_SKILL_STATE _state)
    {
        InGameManager.instance.PlaySkillSound(type, _state);
    }
}
