using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NoMono
{
	public abstract class BaseEntity
	{
		public BaseEntity()
		{
			isDirtyAdd = true;
		}
		public bool isDirtyAdd
		{
			get
			{
				return _isDirtyAdd;
			}
			set
			{
				_isDirtyAdd = value;
				EntityManager.instance.isDirtyAdd = value;
			}
		}
		public abstract EntityType entityType { get; }
		public bool needTick { get; private set; }
		public bool isActive { get; private set; }
		public uint entityId { get; private set; }

		protected abstract string _prefabPath { get; }
		protected GameObject _gameObject { get; private set; }
		protected BaseMonoPlugin _monoPlugin { get; private set; }

		private bool _isDirtyAdd;

		private Dictionary<uint, BaseComponent> _type2compDict = new Dictionary<uint, BaseComponent>(8);
		private LinkedList<BaseComponent> _compLinList = new LinkedList<BaseComponent>();
		private LinkedList<BaseComponent> _tickCompLinList = new LinkedList<BaseComponent>();
		private List<BaseComponent> _toBeAddedCompList = new List<BaseComponent>();

		public void Create(uint entityId, Transform entityRoot)
		{
			this.entityId = entityId;
			SetActive(true);

			if (string.IsNullOrEmpty(_prefabPath) == false)
			{
				GameObject prefab = ResourceManager.instance.Get<GameObject>(_prefabPath);
				if (prefab == null)
					throw new System.Exception("BaseEntity Create.");
				_gameObject = GameObject.Instantiate(prefab);
				_gameObject.transform.SetParent(entityRoot, true);
				_monoPlugin = _gameObject.GetComponent<BaseMonoPlugin>();
				if (_monoPlugin == null)
					Debug.Log("No MonoPlugin. " + entityType.ToString()); //throw new System.Exception("BaseEntity Create No BaseMonoPlugin.");
				else
					_monoPlugin.Init(this);
			}

			Init();
		}

		protected abstract void Init(); // 这里负责设置 isActive _prefabPath 相关的代码; 这里负责new Component(); AddComponent;

		public virtual void Setup() { } // 

		public void AddComponent(BaseComponent comp)
		{
			if (comp == null)
				return;
			var typeId = (uint)comp.compType;
			if (_type2compDict.ContainsKey(typeId))
			{
				Debug.LogError("还没想好要不要携带多个相同组件,目前不允许");
				return;
			}
			isDirtyAdd = true;
			comp.Init(this);
			_toBeAddedCompList.Add(comp);
		}

		public void RemoveComponent(ComponentType compType)
		{
			var typeId = (uint)compType;
			if (_type2compDict.ContainsKey(typeId) == false)
				return;

			var comp = _type2compDict[typeId];
			_type2compDict.Remove(typeId);
			_tickCompLinList.Remove(comp);
			_compLinList.Remove(comp);

			comp.Clear();
			comp.Destroy();
		}

		public void TickAddTo()
		{
			if (isDirtyAdd == true)
			{
				isDirtyAdd = false;
				if (_toBeAddedCompList.Count != 0)
				{
					BaseComponent comp = null;
					for (int i = 0;i < _toBeAddedCompList.Count; i++)
					{
						comp = _toBeAddedCompList[i];
						if (comp != null)
						{
							_compLinList.AddLast(comp);
							_type2compDict.Add((uint)comp.compType, comp);
							if (comp.needTick)
								_tickCompLinList.AddLast(comp);
						}
					}

					for (int i = 0; i < _toBeAddedCompList.Count; i++)
					{
						comp = _toBeAddedCompList[i];
						if (comp !=null)
						{
							comp.Setup();
						}
					}

					_toBeAddedCompList.Clear();
				}
			}
		}

		#region Tick
		public void Tick0(float deltaTime)
		{
			var node = _tickCompLinList.First;
			BaseComponent value = null;
			while (node != null)
			{
				value = node.Value;
				if (value != null && value.needTick && value.isEnable)
				{
					value.Tick0(deltaTime);
				}
				node = node.Next;
			}
		}

		public void Tick1(float deltaTime)
		{
			var node = _tickCompLinList.First;
			BaseComponent value = null;
			while (node != null)
			{
				value = node.Value;
				if (value != null && value.needTick && value.isEnable)
				{
					value.Tick1(deltaTime);
				}
				node = node.Next;
			}
		}

		public void Tick2(float deltaTime)
		{
			var node = _tickCompLinList.First;
			BaseComponent value = null;
			while (node != null)
			{
				value = node.Value;
				if (value != null && value.needTick && value.isEnable)
				{
					value.Tick2(deltaTime);
				}
				node = node.Next;
			}
		}

		public void Tick3(float deltaTime)
		{
			var node = _tickCompLinList.First;
			BaseComponent value = null;
			while (node != null)
			{
				value = node.Value;
				if (value != null && value.needTick && value.isEnable)
				{
					value.Tick3(deltaTime);
				}
				node = node.Next;
			}
		}

		public void Tick4(float deltaTime)
		{
			var node = _tickCompLinList.First;
			BaseComponent value = null;
			while (node != null)
			{
				value = node.Value;
				if (value != null && value.needTick && value.isEnable)
				{
					value.Tick4(deltaTime);
				}
				node = node.Next;
			}
		}

		#endregion

		public virtual void Clear()
		{
			var node = _compLinList.Last;
			BaseComponent value = null;
			while (node != null)
			{
				value = node.Value;
				if (value != null)
				{
					value.Clear();
				}
				node = node.Previous;
			}

			SetActive(false);
		}

		public virtual void Destroy()
		{
			var node = _compLinList.Last;
			BaseComponent value = null;
			while (node != null)
			{
				value = node.Value;
				if (value != null)
				{
					value.Destroy();
				}
				node = node.Previous;
			}

			if (_monoPlugin != null)
			{
				GameObject.Destroy(_monoPlugin.gameObjectPlugin);
			}
		}

		public void SetActive(bool isActive)
		{
			this.isActive = isActive;
		}

		public void SetNeedTick(bool needTick)
		{
			this.needTick = needTick;
		}

		public T GetComponent<T>(ComponentType compType) where T: BaseComponent
		{
			uint typeId = (uint)compType;
			BaseComponent component;
			if (_type2compDict.TryGetValue(typeId, out component) == false)
			{
				return null;
			}
			return component as T;
		}
		public BaseMonoPlugin GetPlugin()
		{
			return _monoPlugin;
		}

		public GameObject GetGameObject()
		{
			return _gameObject;
		}

	}
}