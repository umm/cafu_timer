using CAFU.Core.Domain.UseCase;
using UnityModule;
using ExtraUniRx;
using NUnit.Framework;
using UniRx;

namespace CAFU.Timer.Domain.UseCase {

    public class TimerUseCaseTest {

        // ライブラリ使いたいが、めんどいので自作Spyする
        class TimerUseCaseSpy : TimerUseCase {

            public class SpyFactory : DefaultUseCaseFactory<TimerUseCaseSpy> {

                private ISubject<float> frameDiffTime;

                protected override void Initialize(TimerUseCaseSpy instance) {
                    base.Initialize(instance);

                    instance.Initialize(this.frameDiffTime);
                }

                public TimerUseCaseSpy Create(ISubject<float> frameDiffTime) {
                    this.frameDiffTime = frameDiffTime;
                    return base.Create();
                }

            }

            public ISubject<float> SpyFrameDiffTimeSubject;

            protected void Initialize(ISubject<float> frameDiffTime) {
                base.Initialize();
                this.SpyFrameDiffTimeSubject = frameDiffTime;
                this.StopWatch = new StopWatch(frameDiffTime);
            }

        }

        private TimerUseCaseSpy usecase;

        [SetUp]
        public void SetUp() {
            this.usecase = new TimerUseCaseSpy.SpyFactory().Create(new Subject<float>());
        }

        [Test]
        public void StartedAsObservableTest() {
            var observer = new TestObserver<Unit>();

            this.usecase.StartedAsObservable.Subscribe(observer);
            Assert.AreEqual(0, observer.OnNextCount);

            this.usecase.Start(1);
            this.usecase.SpyFrameDiffTimeSubject.OnNext(1f);
            Assert.AreEqual(1, observer.OnNextCount);
        }

        [Test]
        public void FinishedAsObservableTest() {
            {
                var observer = new TestObserver<Unit>();
                this.usecase.FinishedAsObservable.Subscribe(observer);
                this.usecase.Start(1f);
                this.usecase.SpyFrameDiffTimeSubject.OnNext(1f);
                Assert.AreEqual(1, observer.OnNextCount);
            }

            {
                var observer = new TestObserver<Unit>();
                this.usecase.FinishedAsObservable.Subscribe(observer);

                Assert.AreEqual(0, observer.OnNextCount);

                this.usecase.Start(2f);
                Assert.AreEqual(0, observer.OnNextCount);
                this.usecase.SpyFrameDiffTimeSubject.OnNext(1f);
                Assert.AreEqual(0, observer.OnNextCount);

                this.usecase.SpyFrameDiffTimeSubject.OnNext(1f);
                Assert.AreEqual(1, observer.OnNextCount);
            }
        }

        [Test]
        public void RemainTimeElapsedTimeAsObservableTest() {
            // start
            {
                var remainTimeObserver = new TestObserver<float>();
                var elapsedTimeObserver = new TestObserver<float>();
                this.usecase.RemainTimeAsObservable.Subscribe(remainTimeObserver);
                this.usecase.ElapsedTimeAsObservable.Subscribe(elapsedTimeObserver);
                Assert.AreEqual(0, remainTimeObserver.OnNextCount);
                Assert.AreEqual(0, elapsedTimeObserver.OnNextCount);

                this.usecase.Start(2f);
                Assert.AreEqual(1, remainTimeObserver.OnNextCount);
                Assert.AreEqual(2f, remainTimeObserver.OnNextValues[0]);
                Assert.AreEqual(1, elapsedTimeObserver.OnNextCount);
                Assert.AreEqual(0f, elapsedTimeObserver.OnNextValues[0]);

                this.usecase.SpyFrameDiffTimeSubject.OnNext(1f);
                Assert.AreEqual(2, remainTimeObserver.OnNextCount);
                Assert.AreEqual(1f, remainTimeObserver.OnNextValues[1]);
                Assert.AreEqual(2, elapsedTimeObserver.OnNextCount);
                Assert.AreEqual(1f, elapsedTimeObserver.OnNextValues[1]);

                this.usecase.SpyFrameDiffTimeSubject.OnNext(1f);
                Assert.AreEqual(3, remainTimeObserver.OnNextCount);
                Assert.AreEqual(0f, remainTimeObserver.OnNextValues[2]);
                Assert.AreEqual(3, elapsedTimeObserver.OnNextCount);
                Assert.AreEqual(2f, elapsedTimeObserver.OnNextValues[2]);
            }

            // restart
            {
                var remainTimeObserver = new TestObserver<float>();
                var elapsedTimeObserver = new TestObserver<float>();
                this.usecase.RemainTimeAsObservable.Subscribe(remainTimeObserver);
                this.usecase.ElapsedTimeAsObservable.Subscribe(elapsedTimeObserver);

                Assert.AreEqual(0, remainTimeObserver.OnNextCount);
                this.usecase.Start(1f);

                Assert.AreEqual(1, remainTimeObserver.OnNextCount);
                Assert.AreEqual(1f, remainTimeObserver.OnNextValues[0]);
                Assert.AreEqual(1, elapsedTimeObserver.OnNextCount);
                Assert.AreEqual(0f, elapsedTimeObserver.OnNextValues[0]);

                this.usecase.SpyFrameDiffTimeSubject.OnNext(1f);
                Assert.AreEqual(2, remainTimeObserver.OnNextCount);
                Assert.AreEqual(0f, remainTimeObserver.OnNextValues[1]);
                Assert.AreEqual(2, elapsedTimeObserver.OnNextCount);
                Assert.AreEqual(1f, elapsedTimeObserver.OnNextValues[1]);
            }
        }

        [Test]
        public void StartStopTest() {
            // start
            var elapsedTimeObserver = new TestObserver<float>();
            this.usecase.ElapsedTimeAsObservable.Subscribe(elapsedTimeObserver);
            var startedObserver = new TestObserver<Unit>();
            this.usecase.StartedAsObservable.Subscribe(startedObserver);
            var finishedObserver = new TestObserver<Unit>();
            this.usecase.FinishedAsObservable.Subscribe(finishedObserver);

            Assert.AreEqual(0, elapsedTimeObserver.OnNextCount);
            Assert.AreEqual(0, startedObserver.OnNextCount);
            Assert.AreEqual(0, finishedObserver.OnNextCount);

            this.usecase.Start(2f);
            Assert.AreEqual(1, elapsedTimeObserver.OnNextCount);
            Assert.AreEqual(0, elapsedTimeObserver.OnNextValues[0]);
            Assert.AreEqual(1, startedObserver.OnNextCount);
            Assert.AreEqual(0, finishedObserver.OnNextCount);

            this.usecase.SpyFrameDiffTimeSubject.OnNext(1f);
            Assert.AreEqual(2, elapsedTimeObserver.OnNextCount);
            Assert.AreEqual(1f, elapsedTimeObserver.OnNextValues[1]);
            Assert.AreEqual(1, startedObserver.OnNextCount);
            Assert.AreEqual(0, finishedObserver.OnNextCount);

            this.usecase.Stop();

            // dont increased
            this.usecase.SpyFrameDiffTimeSubject.OnNext(1f);
            Assert.AreEqual(2, elapsedTimeObserver.OnNextCount);
            Assert.AreEqual(1f, elapsedTimeObserver.OnNextValues[1]);
            Assert.AreEqual(1, startedObserver.OnNextCount);
            Assert.AreEqual(0, finishedObserver.OnNextCount);

            this.usecase.Start(2f);
            Assert.AreEqual(3, elapsedTimeObserver.OnNextCount);
            Assert.AreEqual(0f, elapsedTimeObserver.OnNextValues[2]);
            Assert.AreEqual(2, startedObserver.OnNextCount);
            Assert.AreEqual(0, finishedObserver.OnNextCount);

            this.usecase.SpyFrameDiffTimeSubject.OnNext(1f);
            Assert.AreEqual(4, elapsedTimeObserver.OnNextCount);
            Assert.AreEqual(1f, elapsedTimeObserver.OnNextValues[3]);
            Assert.AreEqual(2, startedObserver.OnNextCount);
            Assert.AreEqual(0, finishedObserver.OnNextCount);

            this.usecase.SpyFrameDiffTimeSubject.OnNext(1f);
            Assert.AreEqual(5, elapsedTimeObserver.OnNextCount);
            Assert.AreEqual(2f, elapsedTimeObserver.OnNextValues[4]);
            Assert.AreEqual(2, startedObserver.OnNextCount);
            Assert.AreEqual(1, finishedObserver.OnNextCount);
        }

        [Test]
        public void ResumePauseTest() {
            // start
            var elapsedTimeObserver = new TestObserver<float>();
            this.usecase.ElapsedTimeAsObservable.Subscribe(elapsedTimeObserver);
            var startedObserver = new TestObserver<Unit>();
            this.usecase.StartedAsObservable.Subscribe(startedObserver);
            var finishedObserver = new TestObserver<Unit>();
            this.usecase.FinishedAsObservable.Subscribe(finishedObserver);

            Assert.AreEqual(0, elapsedTimeObserver.OnNextCount);
            Assert.AreEqual(0, startedObserver.OnNextCount);
            Assert.AreEqual(0, finishedObserver.OnNextCount);

            this.usecase.Start(3f);
            Assert.AreEqual(1, elapsedTimeObserver.OnNextCount);
            Assert.AreEqual(0f, elapsedTimeObserver.OnNextValues[0]);
            Assert.AreEqual(1, startedObserver.OnNextCount);
            Assert.AreEqual(0, finishedObserver.OnNextCount);

            this.usecase.SpyFrameDiffTimeSubject.OnNext(2f);
            Assert.AreEqual(2, elapsedTimeObserver.OnNextCount);
            Assert.AreEqual(2f, elapsedTimeObserver.OnNextValues[1]);
            Assert.AreEqual(1, startedObserver.OnNextCount);
            Assert.AreEqual(0, finishedObserver.OnNextCount);

            this.usecase.Pause();
            this.usecase.SpyFrameDiffTimeSubject.OnNext(2f);
            Assert.AreEqual(2, elapsedTimeObserver.OnNextCount);
            Assert.AreEqual(2f, elapsedTimeObserver.OnNextValues[1]);
            Assert.AreEqual(1, startedObserver.OnNextCount);
            Assert.AreEqual(0, finishedObserver.OnNextCount);

            this.usecase.Resume();
            this.usecase.SpyFrameDiffTimeSubject.OnNext(2f);
            Assert.AreEqual(3, elapsedTimeObserver.OnNextCount);
            Assert.AreEqual(3f, elapsedTimeObserver.OnNextValues[2]);
            Assert.AreEqual(1, startedObserver.OnNextCount);
            Assert.AreEqual(1, finishedObserver.OnNextCount);
        }

        [Test]
        public void IsPlayingTest() {
            var observer = new TestObserver<bool>();
            this.usecase.IsPlayingAsObservable.Subscribe(observer);
            
            this.usecase.Start(1f);
            Assert.IsTrue(this.usecase.IsPlaying);
            Assert.AreEqual(1, observer.OnNextCount);
            Assert.IsTrue(observer.OnNextValues[0]);

            this.usecase.SpyFrameDiffTimeSubject.OnNext(1f);
            Assert.IsFalse(this.usecase.IsPlaying);
            Assert.AreEqual(2, observer.OnNextCount);
            Assert.IsFalse(observer.OnNextValues[1]);
        }

    }

}