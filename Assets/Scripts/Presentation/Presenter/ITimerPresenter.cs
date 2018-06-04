using System;
using UniRx;
using CAFU.Timer.Domain.UseCase;

namespace CAFU.Timer.Presentation.Presenter
{
    public interface ITimerPresenter : CAFU.Core.Presentation.Presenter.IPresenter
    {
        ITimerUseCase TimerUseCase { get; }
    }

    public static class ITimerPresenterExtension
    {
        public static IObservable<float> GetRemainTimeAsObservable(this ITimerPresenter presenter)
        {
            return presenter.TimerUseCase.RemainTimeAsObservable;
        }

        public static IObservable<float> GetElapsedTimeAsObservable(this ITimerPresenter presenter)
        {
            return presenter.TimerUseCase.ElapsedTimeAsObservable;
        }

        public static IObservable<Unit> GetTimerStartedAsObservable(this ITimerPresenter presenter)
        {
            return presenter.TimerUseCase.StartedAsObservable;
        }

        public static IObservable<Unit> GetTimerFinishedObservable(this ITimerPresenter presenter)
        {
            return presenter.TimerUseCase.FinishedAsObservable;
        }

        public static float GetCurrentFinishTime(this ITimerPresenter presenter)
        {
            return presenter.TimerUseCase.CurrentFinishTime;
        }

        public static bool IsPlayingTimer(this ITimerPresenter presenter)
        {
            return presenter.TimerUseCase.IsPlaying;
        }

        public static void StartTimer(this ITimerPresenter presenter, float finishTime)
        {
            presenter.TimerUseCase.Start(finishTime);
        }

        public static void StopTimer(this ITimerPresenter presenter)
        {
            presenter.TimerUseCase.Stop();
        }

        public static void ResumeTimer(this ITimerPresenter presenter)
        {
            presenter.TimerUseCase.Resume();
        }

        public static void PauseTimer(this ITimerPresenter presenter)
        {
            presenter.TimerUseCase.Pause();
        }
    }
}