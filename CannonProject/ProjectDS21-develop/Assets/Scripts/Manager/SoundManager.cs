using System.Collections.Generic;
using UnityEngine;

public enum SoundEffectType
{
    CARD_OVERTURN,
    CARD_TAKEOUT
}

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioClip[] effectClips; //���� Ÿ���� ������� �迭�� ����ִ´�
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
