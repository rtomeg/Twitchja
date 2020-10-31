using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource baseLoop;
    [SerializeField] private AudioSource ritualLoop;
    [SerializeField] private AudioSource ouijaTimeLoop;
    [SerializeField] private AudioSource sfx;

    [SerializeField] private AudioClip hit1;
    [SerializeField] private AudioClip hit2;

    private Tween baseLoopTween;
    private Tween ritualLoopTween;
    private Tween ouijaLoopTween;

    private void Awake()
    {
        EventsManager.onStartReadingTwitchResponses += StartRitualLoop;
        EventsManager.onEndReadingTwitchResponses += StopRitualLoop;
        EventsManager.onEndReadingTwitchResponses += StopBaseLoop;

        EventsManager.onOuijaResponseStarted += StartOuijaTimeLoop;
        EventsManager.onOuijaResponseEnded += StartBaseLoop;
        EventsManager.onOuijaResponseEnded += StopOuijaTimeLoop;


        EventsManager.onEndReadingTwitchResponses += PlayHit1Sound;
        EventsManager.onOuijaResponseEnded += PlayHit2Sound;
    }

    private void OnDestroy()
    {
        EventsManager.onStartReadingTwitchResponses -= StartRitualLoop;
        EventsManager.onEndReadingTwitchResponses -= StopRitualLoop;
        EventsManager.onEndReadingTwitchResponses -= StopBaseLoop;
        EventsManager.onOuijaResponseStarted -= StartOuijaTimeLoop;
        EventsManager.onEndReadingTwitchResponses -= PlayHit1Sound;

        EventsManager.onOuijaResponseEnded -= PlayHit2Sound;
        EventsManager.onOuijaResponseEnded -= StartBaseLoop;
        EventsManager.onOuijaResponseEnded -= StopOuijaTimeLoop;
    }

    private void PlayHit1Sound()
    {
        sfx.PlayOneShot(hit1);
    }

    private void PlayHit2Sound(string s)
    {
        sfx.PlayOneShot(hit2);
    }

    private void StartBaseLoop(string s)
    {
        if (baseLoopTween != null && baseLoopTween.IsPlaying())
        {
            baseLoopTween.Pause();
        }

        baseLoopTween = baseLoop.DOFade(1, 1f);
    }

    private void StopBaseLoop()
    {
        if (baseLoopTween != null && baseLoopTween.IsPlaying())
        {
            baseLoopTween.Pause();
        }

        baseLoopTween = baseLoop.DOFade(0, 0.1f);
    }

    private void StartRitualLoop()
    {
        if (ritualLoopTween != null && ritualLoopTween.IsPlaying())
        {
            ritualLoopTween.Pause();
        }

        ritualLoopTween = ritualLoop.DOFade(1, 1f);
    }

    private void StopRitualLoop()
    {
        if (ritualLoopTween != null && ritualLoopTween.IsPlaying())
        {
            ritualLoopTween.Pause();
        }

        ritualLoopTween = ritualLoop.DOFade(0, 0.1f);
    }

    private void StartOuijaTimeLoop()
    {
        if (ouijaLoopTween != null && ouijaLoopTween.IsPlaying())
        {
            ouijaLoopTween.Pause();
        }

        ouijaLoopTween = ouijaTimeLoop.DOFade(1, 1f);
    }

    private void StopOuijaTimeLoop(string s)
    {
        if (ouijaLoopTween != null && ouijaLoopTween.IsPlaying())
        {
            ouijaLoopTween.Pause();
        }

        ouijaLoopTween = ouijaTimeLoop.DOFade(0, 0.1f);
    }
}