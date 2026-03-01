using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private int poolSize = 20;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();


    [Header("To Initialize")]
    [SerializeField] private GameObject weaponPickup;
    [SerializeField] private GameObject ammoPickup;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (weaponPickup != null)
            InitializeNewPool(weaponPickup);

        if (ammoPickup != null)
            InitializeNewPool(ammoPickup);
    }

    public GameObject GetObject(GameObject prefab, Transform target)
    {
        if (prefab == null) return null;

        // Tối ưu: TryGetValue thay vì ContainsKey + index
        if (!poolDictionary.TryGetValue(prefab, out Queue<GameObject> poolQueue))
        {
            InitializeNewPool(prefab);
            poolQueue = poolDictionary[prefab];
        }

        if (poolQueue.Count == 0)
        {
            CreateNewObject(prefab);
        }

        GameObject objectToGet = poolQueue.Dequeue();

        objectToGet.transform.SetParent(null);
        objectToGet.transform.position = target.position;
        objectToGet.SetActive(true);

        return objectToGet;
    }

    public void ReturnObject(GameObject objectToReturn, float delay = .001f)
    {
        if (objectToReturn == null) return;
        StartCoroutine(DelayReturn(delay, objectToReturn));
    }

    private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(objectToReturn);
    }

    private void ReturnToPool(GameObject objectToReturn)
    {
        if (objectToReturn == null) return;

        var pooledObj = objectToReturn.GetComponent<PooledObject>();
        if (pooledObj == null || pooledObj.originalPrefab == null)
        {
            Debug.LogWarning("Object returned to pool without PooledObject component.");
            Destroy(objectToReturn);
            return;
        }

        GameObject originalPrefab = pooledObj.originalPrefab;

        //
        if (!poolDictionary.TryGetValue(originalPrefab, out Queue<GameObject> poolQueue))
        {
            InitializeNewPool(originalPrefab);
            poolQueue = poolDictionary[originalPrefab];
        }

        objectToReturn.SetActive(false);
        objectToReturn.transform.SetParent(transform);

        poolQueue.Enqueue(objectToReturn);
    }

    private void InitializeNewPool(GameObject prefab)
    {
        if (prefab == null) return;

        // 
        if (poolDictionary.ContainsKey(prefab)) return;

        Queue<GameObject> newQueue = new Queue<GameObject>(poolSize);
        poolDictionary[prefab] = newQueue;

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }

    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform);
        var pooledObj = newObject.GetComponent<PooledObject>();

        if (pooledObj == null)
        {
            pooledObj = newObject.AddComponent<PooledObject>();
        }

        pooledObj.originalPrefab = prefab;
        newObject.SetActive(false);

        poolDictionary[prefab].Enqueue(newObject);
    }

    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    //private void Start()
    //{
    //    InitializeNewPool(weaponPickup);
    //    InitializeNewPool(ammoPickup);
    //}


    //public GameObject GetObject(GameObject prefab, Transform target)
    //{
    //    if (poolDictionary.ContainsKey(prefab) == false)
    //    {
    //        InitializeNewPool(prefab);
    //    }

    //    if (poolDictionary[prefab].Count == 0)
    //    {
    //        CreateNewObject(prefab);
    //    }
    //    GameObject objectToGet = poolDictionary[prefab].Dequeue();

    //    objectToGet.transform.position = target.position;
    //    objectToGet.transform.parent = null;
    //    objectToGet.SetActive(true);

    //    return objectToGet;
    //}

    //public void ReturnObject(GameObject objectToReturn, float delay = .001f)
    //{
    //    StartCoroutine(DelayReturn(delay, objectToReturn));
    //}

    //private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    //{
    //    yield return new WaitForSeconds(delay);
    //    ReturnToPool(objectToReturn);
    //}

    //private void ReturnToPool(GameObject objectToReturn)
    //{
    //    objectToReturn.SetActive(false);

    //    GameObject originalPrefab = objectToReturn.GetComponent<PooledObject>().originalPrefab;
    //    objectToReturn.transform.parent = transform;

    //    poolDictionary[originalPrefab].Enqueue(objectToReturn);
    //}

    //private void InitializeNewPool(GameObject prefab)
    //{
    //    poolDictionary[prefab] = new Queue<GameObject>();
    //    for (int i = 0; i < poolSize; i++)
    //    {
    //        CreateNewObject(prefab);
    //    }
    //}

    //private void CreateNewObject(GameObject prefab)
    //{
    //    GameObject newObject = Instantiate(prefab);
    //    newObject.AddComponent<PooledObject>().originalPrefab = prefab;
    //    newObject.SetActive(false);
    //    newObject.transform.parent = transform;

    //    poolDictionary[prefab].Enqueue(newObject);
    //}
}

