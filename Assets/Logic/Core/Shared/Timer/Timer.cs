using System;
using UnityEngine;

namespace Core.Shared.Timer
{
    public class Timer
    {
        private static TimerUpdater _timerUpdater;
        private float _interval;
        private float _elapsedTime;
        private Action _onComplete;

        public float Interval => _interval;
        public float ElapsedTime => _elapsedTime;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateUpdater()
        {
            GameObject updater = new GameObject("Timer");
            _timerUpdater = updater.AddComponent<TimerUpdater>();
        }
        public static Timer Start(float time)
        {
            Timer timer = new Timer(time);
            _timerUpdater.AddListener(timer.Update);
            return timer;
        }
        private Timer(float time)
        {
            _interval = time;
        }
        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            if(_elapsedTime >= _interval)
            {
                Complete();
            }
        }
        public void Pause()
        {
            _timerUpdater.RemoveListener(Update);
        }
        public void Resume()
        {
            _timerUpdater.AddListener(Update);
        }
        public Timer OnComplete(Action onComplete)
        {
            _onComplete = null;
            _onComplete = onComplete;
            return this;
        }
        public void Complete()
        {
            _onComplete?.Invoke();
            Stop();
        }
        public void Stop()
        {
            _onComplete = null;
            _timerUpdater.RemoveListener(Update);
        }
    }
}