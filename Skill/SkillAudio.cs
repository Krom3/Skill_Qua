using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillAudio", menuName = "Scriptable Object Asset/SkillAudio")]
public class SkillAudio : ScriptableObject
{
    [System.Serializable]
    public class SkillAudioData
    {
        public bool useShootSound;
        public AudioData audioShoot;
        public bool useBoomSound;
        public AudioData audioBoom;
    }
    [System.Serializable]
    public class AudioData
    {
        public AudioClip audioSource;
        public float delay;
        public float volume = 0.5f;
    }

    public SkillAudioData[] arrSkillAudioData = new SkillAudioData[(int)E_SKILL_TYPE.END];

}
