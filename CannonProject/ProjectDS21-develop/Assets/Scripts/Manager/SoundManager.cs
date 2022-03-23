using System.Collections.Generic;
using UnityEngine;

public enum SoundEffectType
{
    CARD_OVERTURN,
    CARD_TAKEOUT
}

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioClip[] effectClips; //사운드 타입의 순서대로 배열에 집어넣는다
    private GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;
    }

    public void PlaySound(SoundEffectType set)
    {
        if (gm.savedData.option.soundEffectSize <= 0) return;

        PoolManager.GetItem<SoundPrefab>().PlaySound(effectClips[(int)set],gm.savedData.option.soundEffectSize);
    }
}
