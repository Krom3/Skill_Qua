using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public const int MAX_SKILLCOUNT = 6;
    public const int MAX_GRADE_ACTIVE = 6;
    public const int MAX_GRADE_PASSIVE = 5;

    [System.Serializable]
    public class SkillData
    {
        public GameObject prefab;
        public E_SKILL_TYPE skill;
        public float coolTime;
        public float currCoolTime;
        public bool isReady = true;
        public int grade;
        public int shotCount = 0;
        public float dealAmount;
        public Sprite img;
        public SkillJson data;
    }

    [System.Serializable]
    public class Array2D
    {
        public List<Transform> listNumber;
    }

    public bool showDir { get; set; } = false;
    public GameObject obj_DirPivot;

    //[HideInInspector]
    public List<SkillData> list_skillActive;
    [HideInInspector]
    public List<SkillData> list_skillPassive;

    public GameObject objBarrier;
    public GameObject objBarrierEve;
    public GameObject objDroneA;
    public GameObject objDroneB;
    public GameObject objDroneS;
    public GameObject objTutelar;
    public GameObject objDefender;
    public List<Array2D> listTutelarCount;
    public List<Transform> listTrnMolotov;
    public Transform trnDroneMissileA;
    public Transform trnDroneMissileB;

    [Header("Skill Prefabs")]
    public List<GameObject> list_Prf_Hunter;
    public List<GameObject> list_Prf_Active;
    public List<GameObject> list_Prf_Passive;

    private void Start()
    {
        obj_DirPivot.SetActive(false);

        for (int i = 0; i < list_skillActive.Count; i++)
        {
            var multiple = list_skillActive[i].prefab.GetComponent<SkillActive>().multiple;
            if (multiple > 1)
            {
                InitCoolTime(list_skillActive[i].skill);
                StartCoroutine(CoDelaySkill(multiple, list_skillActive[i]));
            }
            else
            {
                var skill = InGameManager.instance.manager_pool.GetSkill(list_skillActive[i].skill);
                skill.Init(list_skillActive[i]);
            }
        }
    }

    private void FixedUpdate()
    {
        if (list_skillActive.Count <= 0)
            return;

        for (int i = 0; i < list_skillActive.Count; i++)
        {
            list_skillActive[i].currCoolTime += Time.fixedDeltaTime;

            if (i == 0)
                InGameManager.instance.manager_ui.SetCoolTime(list_skillActive[i].currCoolTime);

            if (list_skillActive[i].currCoolTime >= list_skillActive[i].coolTime)
            {
                if (list_skillActive[i].isReady == false)
                    continue;

                var multiple = list_skillActive[i].prefab.GetComponent<SkillActive>().multiple;
                if (multiple > 1)
                {
                    InitCoolTime(list_skillActive[i].skill);
                    StartCoroutine(CoDelaySkill(multiple, list_skillActive[i]));
                }
                else
                {
                    var skill = InGameManager.instance.manager_pool.GetSkill(list_skillActive[i].skill);
                    if (skill == null)
                        continue;

                    skill.Init(list_skillActive[i]);
                }
            }
        }
    }

    IEnumerator CoDelaySkill(int multiple, SkillData _skillData)
    {
        for (int indexSkill = 0; indexSkill < multiple; indexSkill++)
        {
            switch (_skillData.skill)
            {
                case E_SKILL_TYPE.다크디어:
                    break;
                case E_SKILL_TYPE.char4:
                case E_SKILL_TYPE.char5_UP:
                case E_SKILL_TYPE.char7:
                case E_SKILL_TYPE.선지자의창:
                case E_SKILL_TYPE.선지자의가호:
                case E_SKILL_TYPE.웨이브B:
                case E_SKILL_TYPE.얼음표창:
                case E_SKILL_TYPE.저주의비:
                    yield return null;
                    break;
                default:
                    yield return new WaitForSeconds(0.1f);
                    break;
            }
            UseSkill(indexSkill, _skillData, multiple, _skillData.skill);
        }
    }

    private void UseSkill(int index, SkillData _skillData, int multiple, E_SKILL_TYPE type)
    {
        var skill = InGameManager.instance.manager_pool.GetSkill(type);
        if (skill == null)
            return;

        skill.Init(_skillData, multiple, index);
    }

    public void AddSkill(E_SKILL_TYPE _type, SkillJson _skillJson)
    {
        GameObject prefab = InGameManager.instance.manager_pool.prefabs_Skill[(int)_type];
        SkillBase skill = prefab.GetComponent<SkillBase>();

        if (IsAlreadyHaveSkill(_type) == true)
        {
            SkillGradeUp(_type, _skillJson, prefab);
            return;
        }

        var newSkill = new SkillData();
        newSkill.prefab = prefab;
        newSkill.img = skill.img_Skill;
        newSkill.skill = skill.type;
        newSkill.grade = 1;
        newSkill.shotCount = 0;
        newSkill.dealAmount = 0;

        if (_type > E_SKILL_TYPE.돌파조합_________________________________)
        {
            SkillActive skillActive = skill.GetComponent<SkillActive>();
            newSkill.isReady = true;
            newSkill.currCoolTime = 0;
            newSkill.coolTime = skillActive.cooltime;
            list_skillActive.Add(newSkill);
            if (list_skillActive.Count == 1)
                InGameManager.instance.manager_ui.SetCoolTimeMax(skillActive.cooltime);
        }
        else if (_type > E_SKILL_TYPE.패시브스킬_지원품_________________________________)
        {
            list_skillPassive.Add(newSkill);
        }
        else
        {
            SkillActive skillActive = skill.GetComponent<SkillActive>();
            newSkill.isReady = true;
            newSkill.currCoolTime = 0;
            newSkill.coolTime = skillActive.cooltime;
            list_skillActive.Add(newSkill);
            if (list_skillActive.Count == 1)
                InGameManager.instance.manager_ui.SetCoolTimeMax(skillActive.cooltime);
        }
    }

    public void AddHunterSkill(SkillJson _skillData)
    {
        E_SKILL_TYPE type;
        if (_skillData.UnlockRequiredPassive == null)
            type = InGameManager.instance.manager_playerSkill.list_Prf_Hunter[int.Parse(_skillData.SkillId)].GetComponent<SkillBase>().type;
        else
            type = InGameManager.instance.manager_playerSkill.list_Prf_Hunter[int.Parse(_skillData.SkillId) - 1].GetComponent<SkillBase>().type;

        GameObject prefab = InGameManager.instance.manager_playerSkill.list_Prf_Hunter[int.Parse(_skillData.SkillId)];

        var newSkill = ReturnSetSkill(prefab, _skillData, type);

        if (newSkill == null)
            return;

        if (list_skillActive.Count >= MAX_SKILLCOUNT)
            return;

        float skillWeight = 0;
        if (InGameManager.instance.manager_playerSkill.list_skillPassive.Count > 0)
        {
            int findIndex = InGameManager.instance.manager_playerSkill.list_skillPassive.FindIndex(item => item.data.SkillId == "6");
            if (findIndex != -1)
                skillWeight = float.Parse(InGameManager.instance.manager_playerSkill.list_skillPassive[findIndex].data.SkillWeight);
        }

        if (skillWeight > 0)
            newSkill.coolTime = float.Parse(_skillData.AttackDelay) * Mathf.Abs(skillWeight);
        else
            newSkill.coolTime = float.Parse(_skillData.AttackDelay);

        if (InGameManager.instance.GetTotemSkill().Count > 0)
        {
            foreach (TotemJson json in InGameManager.instance.GetTotemSkill())
            {
                if (json.OptionTrigger.Equals("PASSIVE") && json.OptionType.Equals("SKILL_DELAY"))
                {
                    string[] skills = json.SkillIds.Split(",");
                    foreach (string skillId in skills)
                        if (skillId.Trim().Equals(_skillData.SkillId))
                            newSkill.coolTime *= Mathf.Abs(1 - float.Parse(json.OptionPower));
                }
            }
        }

        newSkill.data = _skillData;
        newSkill.isReady = true;
        newSkill.currCoolTime = newSkill.coolTime;

        list_skillActive.Insert(0, newSkill);
        InGameManager.instance.manager_ui.SetCoolTimeMax(newSkill.coolTime);

        if (newSkill != null)
            if (newSkill.prefab.GetComponent<SkillActive>())
                ShowDir(newSkill.prefab.GetComponent<SkillActive>().shootForPlayerLastDir);
    }

    public void AddActiveSkill(SkillJson _skillData)
    {
        bool isEvo = false;

        E_SKILL_TYPE type;
        if (_skillData.UnlockRequiredPassive == null && _skillData.UnlockRequiredSkills == null)
            type = InGameManager.instance.manager_playerSkill.list_Prf_Active[int.Parse(_skillData.SkillId)].GetComponent<SkillBase>().type;
        else
        {
            isEvo = true;

            if (_skillData.SkillId.Equals("27") || _skillData.SkillId.Equals("28"))
                type = E_SKILL_TYPE.마법진;
            else if (_skillData.SkillId.Equals("25"))
                type = E_SKILL_TYPE.쿠라클A;
            else
                type = InGameManager.instance.manager_playerSkill.list_Prf_Active[int.Parse(_skillData.SkillId) - 1].GetComponent<SkillBase>().type;
        }

        GameObject prefab = InGameManager.instance.manager_playerSkill.list_Prf_Active[int.Parse(_skillData.SkillId)];

        var newSkill = ReturnSetSkill(prefab, _skillData, type, isEvo);
        if (newSkill == null)
            return;

        if (list_skillActive.Count >= MAX_SKILLCOUNT)
            return;

        if (_skillData.AttackDelay == null)
            newSkill.coolTime = 10000;
        else
        {
            float skillWeight = 0;
            if (InGameManager.instance.manager_playerSkill.list_skillPassive.Count > 0)
            {
                int findIndex = InGameManager.instance.manager_playerSkill.list_skillPassive.FindIndex(item => item.data.SkillId.Equals("6"));
                if (findIndex != -1)
                    skillWeight = float.Parse(InGameManager.instance.manager_playerSkill.list_skillPassive[findIndex].data.SkillWeight);
            }

            if (skillWeight > 0)
                newSkill.coolTime = float.Parse(_skillData.AttackDelay) * Mathf.Abs(skillWeight);
            else
                newSkill.coolTime = float.Parse(_skillData.AttackDelay);
        }

        newSkill.data = _skillData;
        newSkill.isReady = true;
        newSkill.currCoolTime = newSkill.coolTime;

        list_skillActive.Add(newSkill);
    }

    public void AddPassiveSkill(SkillJson _skillData)
    {
        E_SKILL_TYPE type = InGameManager.instance.manager_playerSkill.list_Prf_Passive[int.Parse(_skillData.SkillId)].GetComponent<SkillBase>().type;
        GameObject prefab = InGameManager.instance.manager_playerSkill.list_Prf_Passive[int.Parse(_skillData.SkillId)];

        var newSkill = ReturnSetSkill(prefab, _skillData, type);

        if (newSkill == null)
            return;

        if (list_skillPassive.Count >= MAX_SKILLCOUNT)
            return;

        newSkill.data = _skillData;

        list_skillPassive.Add(newSkill);
        ApplyPassiveSkill(newSkill.data);
    }

    void ApplyPassiveSkill(SkillJson skillJson)
    {
        float skillWeight = float.Parse(skillJson.SkillWeight);
        switch (skillJson.SkillId)
        {
            // 황금건틀릿 (골드 획득량이 증가합니다)
            case "1":
                // 골드 생성 스크립트에 구현
                break;

            // 화약 (발사체의 속도가 증가합니다)
            case "2":
                // 발사 스크립트에 구현
                break;

            // 피리 (아이템 탐지 범위가 넓어집니다)
            case "3":
                InGameManager.instance.manager_player.magneticCollider.size = Vector2.one * 1.5f * skillWeight;
                break;

            // 성수 (최대 HP가 증가합니다)
            case "4":
                InGameManager.instance.manager_player.MulMaxHp(skillWeight);
                break;

            // 날개 (헌터의 이동 속도가 증가합니다)
            case "5":
                InGameManager.instance.manager_player.MulHunterSpeed(skillWeight);
                break;

            // 에너지 스톤 (스킬 발동 간격이 감소합니다)
            case "6":
                if (list_skillActive.Count == 0)
                    break;

                foreach (SkillData activeData in list_skillActive)
                {
                    activeData.coolTime = float.Parse(activeData.data.AttackDelay) * Mathf.Abs(skillWeight);
                    if (activeData.skill < E_SKILL_TYPE.액티브스킬_________________________________ || activeData.skill >= E_SKILL_TYPE.char1_UP)
                        InGameManager.instance.manager_ui.SetCoolTimeMax(activeData.coolTime);
                }
                break;

            // 성좌의 마력 (스킬 효과 지속시간이 증가합니다)
            case "7":
                // 스킬 스크립트에 구현
                break;

            // 거울 (자동으로 HP 추가 회복)
            case "8":
                InGameManager.instance.manager_player.coolTime_AutoPlusHp = float.Parse(skillJson.AttackDelay);
                InGameManager.instance.manager_player.autoPlusHpWeight = skillWeight;
                InGameManager.instance.manager_player.isAutoPlusHp = true;
                break;

            // 독약 (스킬의 공격범위가 증가합니다)
            case "9":
                // 스킬 스크립트에 구현
                break;

            // 고대유물 (경험치 획득량이 증가합니다)
            case "10":
                // 경험치 아이템 스크립트에 구현
                break;

            // 길드의 문장 (스킬의 공격력이 증가합니다)
            case "11":
                // 스킬 스크립트에 구현
                break;

            // 부적 (헌터가 받는 피해량이 감소합니다)
            case "12":
                // 헌터 스크립트에 구현
                break;
        }
    }

    void SetAdvancedActiveSkill(SkillData skill)
    {
        if (skill.data.AttackDelay == null)
            skill.coolTime = 10000;
        else
        {
            float skillWeight = 0;
            if (InGameManager.instance.manager_playerSkill.list_skillPassive.Count > 0)
            {
                int findIndex = InGameManager.instance.manager_playerSkill.list_skillPassive.FindIndex(item => item.data.SkillId.Equals("6"));
                if (findIndex != -1)
                    skillWeight = float.Parse(InGameManager.instance.manager_playerSkill.list_skillPassive[findIndex].data.SkillWeight);
            }

            if (skillWeight > 0)
                skill.coolTime = float.Parse(skill.data.AttackDelay) * Mathf.Abs(skillWeight);
            else
                skill.coolTime = float.Parse(skill.data.AttackDelay);
        }

        if (InGameManager.instance.GetTotemSkill().Count > 0)
        {
            foreach (TotemJson json in InGameManager.instance.GetTotemSkill())
            {
                if (json.OptionTrigger.Equals("PASSIVE") && json.OptionType.Equals("SKILL_DELAY"))
                {
                    string[] skills = json.SkillIds.Split(",");
                    foreach (string skillId in skills)
                        if (skillId.Trim().Equals(skill.data.SkillId))
                            skill.coolTime *= Mathf.Abs(1 - float.Parse(json.OptionPower));
                }
            }
        }

        SkillBase skillBase = skill.prefab.GetComponent<SkillBase>();
        skill.img = skillBase.img_Skill;
        skill.skill = skillBase.type;
    }

    SkillData ReturnSetSkill(GameObject _prf_Skill, SkillJson _skillData, E_SKILL_TYPE _type, bool isEvo = false)
    {
        SkillBase skill = _prf_Skill.GetComponent<SkillBase>();

        if (IsAlreadyHaveSkill(_type) == true)
        {
            SkillGradeUp(_type, _skillData, _prf_Skill);
            return null;
        }

        var newSkill = new SkillData();
        newSkill.prefab = _prf_Skill;
        if (_skillData.AttackCount != null)
            newSkill.prefab.GetComponent<SkillActive>().multiple = int.Parse(_skillData.AttackCount);
        if (_skillData.RangeWeight != null)
            newSkill.prefab.GetComponent<SkillActive>().range_scan *= float.Parse(_skillData.RangeWeight);
        newSkill.img = skill.img_Skill;
        newSkill.skill = _prf_Skill.GetComponent<SkillBase>().type;
        if (isEvo == true)
            newSkill.grade = 6;
        else
            newSkill.grade = 1;
        newSkill.dealAmount = 0;

        return newSkill;
    }

    public bool IsAlreadyHaveSkill(E_SKILL_TYPE _type)
    {
        foreach (var item in list_skillActive)
        {
            if (item.skill == _type)
                return true;
        }
        foreach (var item in list_skillPassive)
        {
            if (item.skill == _type)
                return true;
        }

        return false;
    }

    public SkillData GetSkill(E_SKILL_TYPE _type)
    {
        SkillData skill = null;
        foreach (var item in list_skillActive)
        {
            if (item.skill == _type)
                skill = item;
        }
        foreach (var item in list_skillPassive)
        {
            if (item.skill == _type)
                skill = item;
        }

        return skill;
    }
    public void SkillGradeUp(E_SKILL_TYPE _type, SkillJson _skillJson, GameObject _prf_SKill)
    {
        if (_prf_SKill != null)
            if (_prf_SKill.GetComponent<SkillActive>() == true)
                ShowDir(_prf_SKill.GetComponent<SkillActive>().shootForPlayerLastDir);

        SkillData skill = null;
        if (_skillJson.SkillId.Equals("25"))
        {
            int hasQuraA = list_skillActive.FindIndex(item => item.data.SkillId.Equals("23"));
            int hasQuraB = list_skillActive.FindIndex(item => item.data.SkillId.Equals("24"));

            if (hasQuraA == -1 || hasQuraB == -1)
                return;

            SkillData skillData = list_skillActive[hasQuraA > hasQuraB ? hasQuraB : hasQuraA];

            skillData.grade = 6;
            skillData.prefab = _prf_SKill;
            skillData.data = _skillJson;
            SetAdvancedActiveSkill(skillData);
            if (_skillJson.AttackCount != null)
                skillData.prefab.GetComponent<SkillActive>().multiple = int.Parse(_skillJson.AttackCount);

            list_skillActive.RemoveAt(hasQuraA > hasQuraB ? hasQuraA : hasQuraB);
        }
        else
        {
            foreach (var item in list_skillActive)
            {
                if (item.skill != _type)
                    continue;

                skill = item;
                if (skill != null)
                {
                    skill.grade += 1;
                    if (skill.grade > MAX_GRADE_ACTIVE)
                        skill.grade = MAX_GRADE_ACTIVE;

                    skill.data = _skillJson;
                    if (skill.grade < MAX_GRADE_ACTIVE)
                    {
                        if (skill.skill < E_SKILL_TYPE.액티브스킬_________________________________ || skill.skill >= E_SKILL_TYPE.char1_UP)
                            skill.data = GameDataManager.GetInstance().GetHunterSkillData(InGameManager.instance.hunterID, int.Parse(_skillJson.SkillId), skill.grade);
                        else
                            skill.data = GameDataManager.GetInstance().GetActiveSkillData(int.Parse(_skillJson.SkillId), skill.grade);
                    }

                    skill.prefab = _prf_SKill;
                    if (skill.grade >= MAX_GRADE_ACTIVE)
                        SetAdvancedActiveSkill(skill);

                    if (item.skill < E_SKILL_TYPE.액티브스킬_________________________________ || item.skill >= E_SKILL_TYPE.char1_UP)
                        InGameManager.instance.manager_ui.SetCoolTimeMax(skill.coolTime);

                    if (_skillJson.AttackCount != null)
                        skill.prefab.GetComponent<SkillActive>().multiple = int.Parse(_skillJson.AttackCount);

                    return;
                }
            }
        }

        skill = null;
        foreach (var item in list_skillPassive)
        {
            if (item.skill != _type)
                continue;

            skill = item;
            if (skill != null)
            {
                skill.grade += 1;
                skill.data = GameDataManager.GetInstance().GetPassiveSkillData(int.Parse(_skillJson.SkillId), skill.grade);
                ApplyPassiveSkill(skill.data);
            }
        }

        if (skill == null)
        {
            Debug.LogError(string.Format("Can't Skill GradeUp - Skill is Null : {0}", _type.ToString()));
            return;
        }
    }

    public float GetSkillCoolTime(E_SKILL_TYPE _typeSkill)
    {
        foreach (var item in list_skillActive)
        {
            if (item.skill == _typeSkill)
            {
                return item.prefab.GetComponent<SkillActive>().cooltime;
            }
        }

        return 1f;
    }

    public void SetReadySkillToFire(E_SKILL_TYPE _typeSkill, bool _isReady = true)
    {
        if (list_skillActive.Count <= 0)
            return;

        foreach (SkillData item in list_skillActive)
        {
            if (item.skill == _typeSkill)
                item.isReady = _isReady;
        }
    }

    public void InitCoolTime(E_SKILL_TYPE _typeSkill)
    {
        if (list_skillActive.Count <= 0)
            return;

        foreach (SkillData item in list_skillActive)
        {
            if (item.skill == _typeSkill)
                item.currCoolTime = 0;
        }
    }

    public List<SkillData> GetSkillDatas()
    {
        List<SkillData> skillDatas = new List<SkillData>();

        if (list_skillActive.Count > 0)
        {
            foreach (SkillData skill in list_skillActive)
            {
                if (skill.grade >= MAX_GRADE_ACTIVE)
                    continue;
                skillDatas.AddRange(list_skillActive);
            }
        }
        if (list_skillPassive.Count > 0)
        {
            foreach (SkillData skill in list_skillPassive)
            {
                if (skill.grade >= MAX_GRADE_PASSIVE)
                    continue;
                skillDatas.AddRange(list_skillPassive);
            }
        }

        return skillDatas;
    }

    public void AddSkillDamageAmount(E_SKILL_TYPE _type, float _damage)
    {
        if (GetSkill(_type) != null)
            GetSkill(_type).dealAmount += _damage;
    }

    public float GetTotalDealAmount()
    {
        float totalDealAmount = 0f;

        foreach (SkillData skillData in list_skillActive)
            totalDealAmount += skillData.dealAmount;

        return totalDealAmount;
    }

    public bool HasRequiredPassive(string _requires)
    {
        List<PlayerSkillManager.SkillData> passsives = InGameManager.instance.manager_playerSkill.list_skillPassive;
        if (passsives.Count == 0)
            return false;

        string[] requirePassives = _requires.Split(",");
        bool[] hasPassives = new bool[requirePassives.Length];
        Array.Fill(hasPassives, false);

        for (int i = 0; i < hasPassives.Length; i++)
            foreach (PlayerSkillManager.SkillData passive in passsives)
                if (requirePassives[i].Trim().Equals(passive.data.SkillId))
                    hasPassives[i] = true;

        foreach (bool hasPassive in hasPassives)
            if (hasPassive == false)
                return false;

        return true;
    }

    public bool HasRequiredActive(string _requires)
    {
        List<SkillData> actives = InGameManager.instance.manager_playerSkill.list_skillActive;
        if (actives.Count == 0)
            return false;

        string[] requireSkills = _requires.Split(",");
        bool[] hasSkills = new bool[requireSkills.Length];
        Array.Fill(hasSkills, false);

        for (int i = 0; i < hasSkills.Length; i++)
            foreach (SkillData active in actives)
                if (requireSkills[i].Trim().Equals(active.data.SkillId) && active.grade >= 5)
                    hasSkills[i] = true;

        foreach (bool hasSkill in hasSkills)
            if (hasSkill == false)
                return false;

        return true;
    }

    public void ShowDir(bool _value)
    {
        showDir = _value;
        obj_DirPivot.SetActive(showDir);
    }

    public void SetDir(Vector3 _value)
    {
        obj_DirPivot.transform.eulerAngles = _value;
    }
}
