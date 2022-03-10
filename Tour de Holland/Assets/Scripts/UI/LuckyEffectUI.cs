using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuckyEffectUI : MonoBehaviour
{
    public enum LuckyEffects { GOTOJAIL, RESPIN, GOTOSTART, GAINONEHUNDRED, GAINTWOHUNDRED, GETPROPERTY}

    [SerializeField] private Image[] luckyEffectBackgrounds;
    [SerializeField] private float slowDownTimer = 3;
    [SerializeField] private float stopTime = 0.5f;

    [SerializeField] private float minIntervalTimeIncrease = 0.02f;
    [SerializeField] private float maxIntervalTimeIncrease = 0.05f;
    [SerializeField] private float minStartingIntervalTime = 0.05f;
    [SerializeField] private float maxStartingIntervalTime = 0.1f;
    [SerializeField] private int currentIndex = 0;
    [SerializeField] private int targetIndex = 0;
    private bool slowdown = false;

    public delegate void LuckyEffectChosen(LuckyEffects luckyEffect);
    public LuckyEffectChosen OnLuckyEffectChosen;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void StartChoosingARandomLuckyEffect()
    {
        currentIndex = Random.Range(0, luckyEffectBackgrounds.Length);
        targetIndex = Random.Range(0, luckyEffectBackgrounds.Length);

        Color color = Color.black;
        color.a = 0.4f;
        luckyEffectBackgrounds[currentIndex].color = color;

        slowdown = false;
        StartCoroutine(nameof(StartSlowingDown));
        StartCoroutine(nameof(ChooseLuckyEffect));
    }

    private IEnumerator ChooseLuckyEffect()
    {
        yield return new WaitForSeconds(1);

        float interval = Random.Range(minStartingIntervalTime, maxStartingIntervalTime);

        while (interval < stopTime || currentIndex != targetIndex)
        {
            yield return new WaitForSeconds(interval);
            Color color = Color.black;
            color.a = 0.2f;
            luckyEffectBackgrounds[currentIndex].color = color;

            currentIndex++;

            if (currentIndex > luckyEffectBackgrounds.Length - 1)
            {
                currentIndex = 0;
            }
            
            color.a = 0.5f;

            luckyEffectBackgrounds[currentIndex].color = color;

            if (slowdown)
            {
                interval += Random.Range(minIntervalTimeIncrease, maxIntervalTimeIncrease);
            }

            AudioManager.instance.Play("WheelTickSFX");
        }

        AudioManager.instance.Play("WheelResultSFX");

        OnLuckyEffectChosen((LuckyEffects)currentIndex);
        anim.SetTrigger("Hide");
    }

    private IEnumerator StartSlowingDown()
    {
        yield return new WaitForSeconds(slowDownTimer + 1);
        slowdown = true;
    }
}
