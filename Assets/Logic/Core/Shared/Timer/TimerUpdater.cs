using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Shared.Timer
{
    public class TimerUpdater : MonoBehaviour
    {
        private event Action OnUpdate;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneUnloaded += ClearListeners;
        }
        public void AddListener(Action update)
        {
            OnUpdate += update;
        }
        public void RemoveListener(Action update)
        {
            OnUpdate -= update;
        }
        private void Update()
        {
            OnUpdate?.Invoke();
        }
        private void ClearListeners(Scene scene)
        {
            OnUpdate = null;
        }
        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= ClearListeners;
        }
    }
}
