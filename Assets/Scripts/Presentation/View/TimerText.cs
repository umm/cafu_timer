using CAFU.Core.Presentation.View;
using CAFU.Timer.Presentation.Presenter;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityModule;

namespace CAFU.Timer.Presentation.View {

    [RequireComponent(typeof(Text))]
    public class TimerText : UIBehaviour, IView {

        public string Format = "のこりじかん: {0} びょう";

        protected override void Start() {
            this.RegisterEventActivationHandler();

            this.GetPresenter<ITimerPresenter>().GetRemainTimeAsObservable()
                .Select(it => Mathf.CeilToInt(it))
                .Subscribe(this.Render)
                .AddTo(this);
        }

        private void Render(int time) {
            this.GetComponent<Text>().text = string.Format(this.Format, time);
        }

    }

}