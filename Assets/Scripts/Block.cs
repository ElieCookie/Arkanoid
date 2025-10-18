using UnityEngine;
using System.Collections.Generic;
using System;


public class Block : MonoBehaviour
{
    [SerializeField] int hitPoints = 1;
    [SerializeField] public int PointsPerBlock = 10;

    public static event Action<Block> OnBlockDestruction;
    public ParticleSystem DestroyEffect;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        //sr.sprite = BlocksManager.Instance.BlockSprites[hitPoints - 1];
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            hitPoints--;
            if (hitPoints <= 0)
            {
                BlocksManager.Instance.RemainingBlocks.Remove(this);
                OnBlockDestruction?.Invoke(this);
                SpawnDestroyEffect();
                OnBlockDestroyed();
                Destroy(gameObject);
            }
            else
            {
                sr.sprite = BlocksManager.Instance.BlockSprites[hitPoints - 1];
            }
        }
    }
    private void SpawnDestroyEffect()
    {
        Vector3 blockPos = transform.position;
        Vector3 effectPos = new Vector3(blockPos.x, blockPos.y, blockPos.z - 0.2f);
        if (DestroyEffect != null)
        {
            GameObject effect = Instantiate(DestroyEffect.gameObject, effectPos, Quaternion.identity);
            ParticleSystem.MainModule effectMain = effect.GetComponent<ParticleSystem>().main;
            effectMain.startColor = sr.color;
            Destroy(effect, DestroyEffect.main.startLifetime.constant);
        }
    }
    public void OnBlockDestroyed()
    {
        float buffSpawnChance = UnityEngine.Random.Range(0f, 100f);
        float deBuffSpawnChance = UnityEngine.Random.Range(0f, 100f);
        bool alreadySpawned = false;
        if (buffSpawnChance <= CollectablesManager.Instance.BuffChance)
        {
            alreadySpawned = true;
            Collectable newBuff = SpawnCollectable(true);
        }
        if (buffSpawnChance <= CollectablesManager.Instance.BuffChance && !alreadySpawned)
        {
            Collectable newDeBuff = SpawnCollectable(false);
        }
    }

    private Collectable SpawnCollectable(bool isBuff)
    {
        List<Collectable> collection;
        if (isBuff)
        {
            collection = CollectablesManager.Instance.AvailableBuffs;
        }
        else
        {
            collection = CollectablesManager.Instance.AvailableDebuffs;
        }

        int buffIndex = UnityEngine.Random.Range(0, collection.Count);
        Collectable prefab = collection[buffIndex];
        Collectable newCollectable = Instantiate(prefab, transform.position, Quaternion.identity) as Collectable;

        return newCollectable;
    }

    public void Init(Transform containerTransform, Sprite sprite, Color color, int hitPoints)
    {
        transform.parent = containerTransform;
        sr.sprite = sprite;
        sr.color = color;
        this.hitPoints = hitPoints;
    }
}
    