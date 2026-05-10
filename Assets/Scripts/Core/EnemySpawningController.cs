using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemySpawningController : MonoBehaviour
{
    public static EnemySpawningController Instance;

    [SerializeField]
    List<BaseEnemy> _enemiesToSpawn;
    [SerializeField]
    List<BaseEnemy> _enemiesInScene;

    [SerializeField]
    float _standardSpawnRate;

    Sequence _spawningSequence;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void Initialize()
    {
        StartSpawningEnemies();
    }

    public void StartSpawningEnemies()
    {
        _spawningSequence = DOTween.Sequence();

        _spawningSequence.AppendInterval(_standardSpawnRate).AppendCallback(() =>
        {
            //Get a random lane from the grid editor
            int randomY = Random.Range(0, GridManager.Instance.Rows);
            GridCell spawningCell = GridManager.Instance.GetCell(0, randomY);

            //choose a random enemy to spawn
            BaseEnemy enemyToSpawn = _enemiesToSpawn[Random.Range(0, _enemiesToSpawn.Count)];
            Vector3 spawnPosition = spawningCell.WorldPosition;

            if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, 10, CastleBuilderController.Instance.HittableLayers))
                spawnPosition.y = hit.point.y;

            //instantiate them (or pull from pool)
            BaseEnemy enemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);

            enemy.Intialize(spawningCell.Row);

            _enemiesInScene.Add(enemy);
        })/*.SetLoops(-1)*/;
    }

    public void StopSpawningEnemies()
    {
        _spawningSequence.Kill(false);
    }
}