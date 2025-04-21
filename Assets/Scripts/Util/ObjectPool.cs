using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides object pooling for bullets and enemies
/// </summary>
public class ObjectPool : MonoBehaviour, IBulletCreatedInvoker, IEnemyCreatedInvoker
{
    static GameObject prefabBullet;
    static GameObject prefabEnemy;
    static Dictionary<PooledObjectName, List<GameObject>> pools;

    // events invoked by class (for autograding only)
    static BulletCreated bulletCreated = new BulletCreated();
    static EnemyCreated enemyCreated = new EnemyCreated();

    #region Constructor

    // Uncomment the code below after you copy this class into the console
    // app for the automated grader. DON'T uncomment it now; it won't
    // compile in a Unity project

    /// <summary>
    /// Constructor
    /// 
    /// Note: The class in the Unity solution doesn't use a constructor;
    /// this constructor is to support automated grading
    /// </summary>
    /// <param name="gameObject">the game object the script is attached to</param>


    #endregion

    /// <summary>
    /// Initializes the pools
    /// </summary>
    public static void Initialize()
    {
        // load prefabs
        // Caution: Don't change the location of the prefabs in the Resources folder
        prefabBullet = Resources.Load<GameObject>("Bullet");
        prefabEnemy = Resources.Load<GameObject>("Enemy");

        // initialize dictionary
        pools = new Dictionary<PooledObjectName, List<GameObject>>();
        pools.Add(PooledObjectName.Bullet,
            new List<GameObject>(GameConstants.InitialBulletPoolCapacity));
        pools.Add(PooledObjectName.Enemy,
            new List<GameObject>(GameConstants.InitialEnemyPoolCapacity));

        // fill bullet pool
        //Gameconstan capacity i değil pool un al.
      for (int i = 0; i < pools[PooledObjectName.Enemy].Capacity; i++)
        {
            GameObject enemy = GetNewObject(PooledObjectName.Enemy);
            pools[PooledObjectName.Enemy].Add(enemy);
        }

        for (int i = 0; i < pools[PooledObjectName.Bullet].Capacity; i++)
        {
            GameObject bullet = GetNewObject(PooledObjectName.Bullet);
            pools[PooledObjectName.Bullet].Add(bullet);
        }
      

        // fill enemy pool
    }

    /// <summary>
    /// Gets a bullet object from the pool
    /// </summary>
    /// <returns>bullet</returns>
    public static GameObject GetBullet()
    {
        // replace code below with correct code
        /*List<GameObject> bulletPool = pools[PooledObjectName.Bullet];

           if (bulletPool.Count ==0)
           {
              return GetNewObject(PooledObjectName.Bullet);
           }else
           {
               return GetPooledObject(PooledObjectName.Bullet);
           }*/
        return GetPooledObject(PooledObjectName.Bullet);
    }

    /// <summary>
    /// Gets an enemy object from the pool
    /// </summary>
    /// <returns>enemy</returns>
    public static GameObject GetEnemy()
    {
        // replace code below with correct code
        /*    List<GameObject> enemyPool = pools[PooledObjectName.Enemy];

          if (enemyPool.Count == 0)
            {
                return GetNewObject(PooledObjectName.Enemy);
            }
            else if(enemyPool.Count > 0 && enemyPool.Count < enemyPool.Capacity)
            {
                return GetPooledObject(PooledObjectName.Enemy);
            }
            else
            {
                enemyPool.Capacity *= 2;
                return GetPooledObject(PooledObjectName.Enemy);
            }*/
        return GetPooledObject(PooledObjectName.Enemy);
    }


    /// <summary>
    /// Gets a pooled object from the pool
    /// </summary>
    /// <returns>pooled object</returns>
    /// <param name="name">name of the pooled object to get</param>
    static GameObject GetPooledObject(PooledObjectName name)
    {
        // check for available object in pool
        // remove object from pool and return (replace code below)
        List<GameObject> pool = pools[name];

        if (pool.Count > 0)
        {
            GameObject obj = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            return obj;
        }
        else
        {
            //Sıkıntılı kısım?
            pool.Capacity++; //yeni
            if (PooledObjectName.Bullet == name)
            {
                return GetNewObject(PooledObjectName.Bullet);
            }
            else
            {
                return GetNewObject(PooledObjectName.Enemy);
            }
        }
    }




    /// <summary>
    /// Returns a bullet object to the pool
    /// </summary>
    /// <param name="bullet">bullet</param>
    public static void ReturnBullet(GameObject bullet)
    {
        // add your code here
        ReturnPooledObject(PooledObjectName.Bullet, bullet);
    }


    /// <summary>
    /// Returns an enemy object to the pool
    /// </summary>
    /// <param name="enemy">enemy</param>
    public static void ReturnEnemy(GameObject enemy)
    {
        // add your code here

        ReturnPooledObject(PooledObjectName.Enemy, enemy);
    }

    /// <summary>
    /// Returns a pooled object to the pool
    /// </summary>
    /// <param name="name">name of pooled object</param>
    /// <param name="obj">object to return to pool</param>
    public static void ReturnPooledObject(PooledObjectName name,
        GameObject obj)
    {
        // add your code here
        obj.SetActive(false);
        //Stop moving i unutmuşum
         if (name == PooledObjectName.Bullet)
          {
              obj.GetComponent<Bullet>().StopMoving();
          }
          else
          {
            obj.GetComponent<Enemy>().Deactivate();
          }
        pools[name].Add(obj);
    }

    /// <summary>
    /// Gets a new object
    /// </summary>
    /// <returns>new object</returns>
    static GameObject GetNewObject(PooledObjectName name)
    {
        GameObject obj;
        if (name == PooledObjectName.Bullet)
        {
            obj = GameObject.Instantiate(prefabBullet);
            bulletCreated.Invoke(obj);
            obj.GetComponent<Bullet>().Initialize();
        }
        else
        {
            obj = GameObject.Instantiate(prefabEnemy);
            enemyCreated.Invoke(obj);
            obj.GetComponent<Enemy>().Initialize();
        }

        obj.SetActive(false);
        GameObject.DontDestroyOnLoad(obj);
        return obj;
    }

    /// <summary>
    /// Removes all the pooled objects from the object pools
    /// </summary>
   public static void EmptyPools()
    {
        // add your code here
        pools[PooledObjectName.Bullet].Clear();
        pools[PooledObjectName.Enemy].Clear();
    }
     

    #region Methods to support autograder

    /// <summary>
    /// Adds the given listener for the BulletCreated event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddBulletCreatedListener(UnityAction<GameObject> listener)
    {
        bulletCreated.AddListener(listener);
    }

    /// <summary>
    /// Adds the given listener for the EnemyCreated event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddEnemyCreatedListener(UnityAction<GameObject> listener)
    {
        enemyCreated.AddListener(listener);
    }

    /// <summary>
    /// Gets the current pool count for the given pooled object
    /// </summary>
    /// <param name="name">pooled object name</param>
    /// <returns>current pool count</returns>
    public int GetPoolCount(PooledObjectName name)
    {
        if (pools.ContainsKey(name))
        {
            return pools[name].Count;
        }
        else
        {
            // should never get here
            return -1;
        }
    }

    /// <summary>
    /// Gets the current pool capacity for the given pooled object
    /// </summary>
    /// <param name="name">pooled object name</param>
    /// <returns>current pool capacity</returns>
    public int GetPoolCapacity(PooledObjectName name)
    {
       //if (pools[name].Count > 0)
       if(pools.ContainsKey(name))
        {
            return pools[name].Capacity;
        }
        else
        {
            // should never get here
            return -1;
        }
    }

    #endregion
}