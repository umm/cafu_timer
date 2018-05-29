using CAFU.Core.Presentation.View;
using CAFU.Timer.Presentation.Presenter;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityModule;

namespace CAFU.Timer.Presentation.View
{
    [RequireComponent(typeof(Image))]
    public class TimerGauge : UIBehaviour, IView
    {
        protected override void Start()
        {
            this.RegisterEventActivationHandler();

            var presenter = this.GetPresenter<ITimerPresenter>();

            presenter.GetRemainTimeAsObservable()
                .Subscribe(time => this.Render(time, presenter.GetCurrentFinishTime()))
                .AddTo(this);
        }

        private void Render(float time, float finishTime)
        {
            var ratio = finishTime > 0 ? time / finishTime : 1f;
            this.GetComponent<Image>().fillAmount = ratio;
        }
    }
}