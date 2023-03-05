using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyData
{
    //public string prefabPath;
    public Vector3 initPos;
    public EnemyController.SelectedTraking traking;
    public GameObject prefab;

    public EnemyData(Enemy enemy/*, GameObject enemyPrefab*/)
    {
        //prefabPath = UnityEditor.AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromOriginalSource(enemy));
        prefab = enemy.prefab;
        initPos = enemy.startPosition;
        traking = enemy.GetComponent<EnemyController>().traking;
    }
}
