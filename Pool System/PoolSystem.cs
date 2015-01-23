using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolSystem : MonoBehaviour
{
    private Transform _poolTransform;
    private GameObject _cacheGameObject;
    private Transform _cacheTransform;
    private List<GameObject> _pool;
    private int _size = 0;
    private bool _isNetworkGame;
    private bool _initialized = false;
    public GameObject prefab;
    public string poolName = "PoolSystem";
    public int poolSize = 10;

    public GameObject this[int index]
    {
        get { return _pool[index]; }
    }

    public int Size
    {
        get { return GetSize(); }
    }

    void Awake()
    {
        BaseAwake();
    }

    public void Resize(int size)
    {
        if (size != _size && _initialized)
        {
            int diff = size - _size;

            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                    AddPrefab();
            }
            else if (diff < 0)
                _pool.RemoveRange(_size + diff, diff);

            _size += diff;
        }
    }

    protected void BaseAwake()
    {
        _cacheGameObject = GameObject.Find(poolName);

        if (_cacheGameObject == null)
            _cacheGameObject = new GameObject(poolName);

        _poolTransform = _cacheGameObject.GetComponent<Transform>();
        _pool = new List<GameObject>(poolSize);
        _isNetworkGame = Network.isClient || Network.isServer;

        InitializePool();

        _initialized = true;
    }

    public virtual GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        _cacheGameObject = GetItem();
        _cacheTransform = _cacheGameObject.GetComponent<Transform>();
        _cacheTransform.position = position;
        _cacheTransform.rotation = rotation;
        _cacheGameObject.SetActive(true);
        return _cacheGameObject;
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
            AddPrefab();
    }

    protected virtual GameObject AddPrefab()
    {
        if (_isNetworkGame)
            _cacheGameObject = Network.Instantiate(prefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
        else
            _cacheGameObject = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;

        _cacheGameObject.GetComponent<Transform>().parent = _poolTransform;
        _cacheGameObject.SetActive(false);
        _pool.Add(_cacheGameObject);
        _size++;

        return _cacheGameObject;
    }

    protected virtual GameObject GetItem()
    {
        int index = GetFirstDisabled();

        if (index == -1)
            _cacheGameObject = AddPrefab();
        else
            _cacheGameObject = _pool[index];

        return _cacheGameObject;
    }

    protected virtual int GetFirstDisabled()
    {
        for (int i = 0; i < _size; i++)
        {
            if (!_pool[i].activeSelf)
                return i;
        }

        return -1;
    }

    public virtual int GetSize()
    {
        return _pool.Count;
    }
}

