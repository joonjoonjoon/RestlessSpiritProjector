using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class WorldParticles : MonoSingleton<WorldParticles> {
    private ParticleSystem ps;
    public AnimationCurve fucjkl;
    public float startsize;

    void Awake()
    {
        instance = this;
        ps = GetComponent<ParticleSystem>();
        startsize = ps.main.startSize.constant;

    }
    void Start () {
        StartCoroutine(DoStarT());
	}

    IEnumerator DoStarT()
    {
        GameManager.instance.freezeAllDetection = true;
        yield return new WaitForSeconds(0.5f);
        DOTween.To(() => startsize, x => { startsize = x; }, 0, 5f).SetEase(Ease.OutCirc);
        yield return new WaitForSeconds(7);
        SoundManager.instance.PlayRandom(SoundManager.instance.voiceIsReady);
        yield return null;
        GameManager.instance.freezeAllDetection = false;
        yield return new WaitForSeconds(15);
        gameObject.SetActive(false);
    }

	public void KillmeAndRestart()
    {
        gameObject.SetActive(true);
        StartCoroutine(DoKillme());
    }

    IEnumerator DoKillme()
    {
        GameManager.instance.freezeAllDetection = true;
        DOTween.To(() => startsize, x => { startsize = x; }, 0.5f, 5f).SetEase(Ease.OutCirc);
        yield return null;

        yield return new WaitForSeconds(5);
        AutoStoredValueManager.ForceStore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    void Update()
    {
        var main = ps.main;
        main.startSize = new ParticleSystem.MinMaxCurve(startsize);
    }
}
