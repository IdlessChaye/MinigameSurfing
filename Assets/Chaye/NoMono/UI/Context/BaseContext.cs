using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NoMono
{
	public abstract class BaseContext
	{
		public bool needTick { get; protected set; }

		protected abstract string _prefabPath { get; }
		protected GameObject _gameObject { get; private set; }
		protected BaseMonoContext _monoContext { get; private set; }


		private List<UnityEvent> _unityEventList = new List<UnityEvent>(4);

		public void Load(Transform pageContextRoot)
		{
			if (string.IsNullOrEmpty(_prefabPath))
				throw new System.Exception("UIManager ShowPage No PrefabPath.");
			GameObject prefab = ResourceManager.instance.Get<GameObject>(_prefabPath);
			if (prefab == null)
				throw new System.Exception("UIManager ShowPage.");
			_gameObject = GameObject.Instantiate(prefab);
			_gameObject.transform.SetParent(pageContextRoot, false);
			_monoContext = _gameObject.GetComponent<BaseMonoContext>();
			if (_monoContext == null)
				throw new System.Exception("UIManager ShowPage No BaseMonoPageContext.");
			_monoContext.Init(this);

			Init();
			SetupEvents();

			_monoContext.Show();
			FullCoroutineManager.Instance.AddCoroutine(0.5f, () => { // 注意：这0.5s之内Context接收不到Callback，先这样，以后也许会有Bug
				SetupCallbacks();
			});
		}

		public virtual void Setup()
		{

		}

		protected abstract void Init(); // 设置 _prefabPath

		protected virtual void SetupCallbacks()
		{

		}

		protected virtual void SetupEvents()
		{

		}

		protected virtual void ClearEvents()
		{

		}

		public virtual void Tick2(float deltaTime) { }

		public void Close()
		{
			Clear();
			_monoContext.Hide();
            FullCoroutineManager.Instance.AddCoroutine(0.1f, () =>
            {
                Destroy();
            });
        }

		protected virtual void Clear()
		{
			ClearEvents();
			ClearAllCallbacks();
			_unityEventList = null;
			needTick = false;
		}

		protected virtual void Destroy()
		{
			if (_monoContext != null)
			{
				GameObject.Destroy(_monoContext.gameObjectContext);
			}
		}

		private void ClearAllCallbacks()
		{
			if (_unityEventList == null)
				return;
			foreach (var unityEvent in _unityEventList)
			{
				if (unityEvent != null)
					unityEvent.RemoveAllListeners();
			}
			_unityEventList.Clear();
		}

		protected void BindCallback(Button button, UnityAction action)
		{
			if (button == null || button.onClick == null || action == null)
				return;
			_unityEventList.Add(button.onClick);
			button.onClick.AddListener(action);
		}

	}
}