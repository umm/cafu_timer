using CAFU.Core.Domain.UseCase;
using CAFU.Timer.Domain.Model;
using ExtraTime;
using UniRx;
using UnityEditor;

namespace CAFU.Timer.Domain.UseCase {

    public interface ITimerUseCase : IUseCase {

        /// <summary>
        /// Start Timer
        /// </summary>
        /// <param name="time">Timer's finish time. unit is second.</param>
        void Start(float timeSeconds);

        /// <summary>
        /// Stop Timer
        /// </summary>
        void Stop();

        /// <summary>
        /// Pause Timer
        /// </summary>
        void Pause();

        /// <summary>
        /// Resume Timer
        /// </summary>
        void Resume();

        /// <summary>
        /// Unit Observable fires when the timer will be started.
        /// </summary>
        IObservable<Unit> StartedAsObservable { get; }

        /// <summary>
        /// Unit Observable fires when the timer will be finished.
        /// </summary>
        IObservable<Unit> FinishedAsObservable { get; }

        /// <summary>
        /// Remain time seconds. It's changing on everyframe.
        /// </summary>
        IObservable<float> RemainTimeAsObservable { get; }

        /// <summary>
        /// Elapsed time seconds. It's changing on everyframe.
        /// </summary>
        IObservable<float> ElapsedTimeAsObservable { get; }

        /// <summary>
        /// Check if it's playing.
        /// </summary>
        IObservable<bool> IsPlayingAsObservable { get; }

        /// <summary>
        /// Return finish time of current timer.
        /// </summary>
        float CurrentFinishTime { get; }

        /// <summary>
        /// Timer is Playing or not
        /// </summary>
        bool IsPlaying { get; }

    }

    public class TimerUseCase : ITimerUseCase {

        public class Factory : DefaultUseCaseFactory<TimerUseCase> {

            protected override void Initialize(TimerUseCase instance) {
                base.Initialize(instance);
                instance.Initialize();
            }

        }

        public IObservable<Unit> StartedAsObservable => this.StartedSubject;

        public IObservable<Unit> FinishedAsObservable => this.GetFinishedAsObservable();

        public IObservable<float> RemainTimeAsObservable => this.GetRemainTimeAsObservable();

        public IObservable<float> ElapsedTimeAsObservable => this.GetElapsedTimeAsObservable();

        public IObservable<bool> IsPlayingAsObservable =>
            this.StopWatch.TimeAsObservable
                .Select(time => this.StopWatch.IsPlaying && time < this.Model.FinishTime)
                .DistinctUntilChanged();

        public bool IsPlaying =>
            this.StopWatch.IsPlaying && this.StopWatch.Time < this.Model.FinishTime;

        public float CurrentFinishTime => this.Model.FinishTime;

        protected IStopWatch StopWatch { get; set; }

        protected TimerModel Model { get; set; }

        private ISubject<Unit> StartedSubject { get; set; }

        public void Start(float timeSeconds) {
            this.Stop();
            this.Model.FinishTime = timeSeconds;

            this.StopWatch.Start();
            this.StartedSubject.OnNext(Unit.Default);
        }

        public void Stop() {
            this.StopWatch.Stop();
        }

        public void Resume() {
            this.StopWatch.Resume();
        }

        public void Pause() {
            this.StopWatch.Pause();
        }

        private IObservable<Unit> GetFinishedAsObservable() {
            return this.GetElapsedTimeAsObservable()
                .Where(time => time >= this.Model.FinishTime)
                .AsUnitObservable()
                .First();
        }

        private IObservable<float> GetRemainTimeAsObservable() {
            return this.StopWatch.TimeAsObservable
                .Select(time => this.Model.FinishTime - time)
                .Select(time => time < 0 ? 0 : time);
        }

        private IObservable<float> GetElapsedTimeAsObservable() {
            return this.StopWatch.TimeAsObservable
                .Select(time => time > this.Model.FinishTime ? this.Model.FinishTime : time);
        }

        protected virtual void Initialize() {
            this.Model = new TimerModel();
            this.StartedSubject = new Subject<Unit>();
            this.StopWatch = new StopWatch();
        }

    }

}