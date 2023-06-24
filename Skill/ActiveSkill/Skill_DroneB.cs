using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DroneB : SkillActive
{
    
    public List<Skill_DroneMissile> listShoot;
    private GameObject objDrone;
    private Transform trnDroneMissile;
    public float delayShoot = 0.3f;
    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
    }

    public override void Init(PlayerSkillManager.SkillData _skillData, int _multiple = 1, int indexMultiple = 0)
    {
        objDrone = InGameManager.instance.manager_playerSkill.objDroneB;
        trnDroneMissile = InGameManager.instance.manager_playerSkill.trnDroneMissileB;
        transform.SetParent(objDrone.transform.GetChild(0));
        transform.localPosition = Vector3.zero;

        base.Init(_skillData);

        foreach (var item in listShoot)
            item.gameObject.SetActive(false);

        objDrone.SetActive(true);
        gameObject.SetActive(true);
        isAlive = true;

        InGameManager.instance.manager_playerSkill.InitCoolTime(type);
        InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);

        StartCoroutine("Shoot");
    }

    IEnumerator Shoot()
    {
        int cnt = 0;
        while (true)
        {
            yield return new WaitForSeconds(delayShoot);
            cnt++;
            if (cnt % 7 == 0)
                yield return new WaitForSeconds(delayShoot * Random.Range(1f, 2f));
            foreach (var item in listShoot)
            {
                if (item.gameObject.activeSelf == true)
                    continue;
                item.target = trnDroneMissile.position;
                item.gameObject.SetActive(true);
                if (time > lifetime)
                {
                    yield return new WaitUntil(() => item.gameObject.activeSelf == false);
                    isAlive = false;
                    gameObject.SetActive(false);
                    InGameManager.instance.manager_playerSkill.SetReadySkillToFire(type);
                    yield break;
                }
                break;
            }
        }
    }
}
