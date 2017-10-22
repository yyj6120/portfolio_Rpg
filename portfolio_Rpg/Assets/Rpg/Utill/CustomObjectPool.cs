using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomObjectPool
{
    public Transform parent = null;
    protected GameObject baseObject = null;
    protected List<GameObject> reUsePool = new List<GameObject>();
    protected Dictionary<int, GameObject> usePool = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> usePoolObject
    {
        get
        {
            return usePool;
        }
    }
    public CustomObjectPool()
    {

    }
    public CustomObjectPool(GameObject original = null, Transform parent = null)
    {
        Register(original, parent);
    }

    public int UsePoolCount
    {
        get
        {
            return usePool.Count;
        }
    }

    public void Register(GameObject go, Transform parent)
    {
        baseObject = go;
        this.parent = parent;
    }

    public void Destroy(bool bUseObject)
    {
        baseObject = null;
        if (bUseObject)
        {
            foreach (var it in usePool)
            {
                GameObject.Destroy(it.Value);
            }
            usePool.Clear();
        }
        else
        {
            int count = reUsePool.Count;
            for (int i = 0; i < count; ++i)
            {
                GameObject.Destroy(reUsePool[i]);
            }
            reUsePool.Clear();
        }
    }
    protected virtual GameObject SpawnObject()
    {
        if (baseObject == null)
        {
            Debug.LogError("No registered objects");
            return new GameObject("go");
        }
        return Instantiate(baseObject) as GameObject;
    }

    public virtual GameObject GetInstance()
    {
#if UNITY_EDITOR
        if (baseObject == null)
        {
            Debug.LogError("No registered objects");
            return null;
        }
#endif
        GameObject go;
        if (reUsePool.Count == 0)
        {
            go = SpawnObject();
            go.transform.parent = parent;
        }
        else
        {
            int instIndex = reUsePool.Count - 1;
            go = reUsePool[instIndex];
            go.SetActive(true);
            reUsePool.RemoveAt(instIndex);
        }
        usePool.Add(go.GetInstanceID(), go);
        return go;
    }

    public void UnUseInsert(GameObject go)
    {
        if (go == null)
            return;
        int ObjectHeshKey = go.GetInstanceID();
        if (usePool.ContainsKey(ObjectHeshKey) == false)
            return;

        go.SetActive(false);
        usePool.Remove(ObjectHeshKey);
        reUsePool.Add(go);
    }

    public IEnumerator UnUseInsert(GameObject go, float time)
    { 
        yield return new WaitForSeconds(time);
        UnUseInsert(go);
    }

    public Object Instantiate(Object original)
    {
        if (original != null)
            return Object.Instantiate(original);
        else
            return null;
    }
}