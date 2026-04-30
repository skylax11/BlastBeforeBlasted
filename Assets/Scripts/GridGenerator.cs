using System;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    private Block _blockPrefab;

    [SerializeField]
    private BlockSO[] _blockProperties;

    [Header("Grid Infos")]

    [SerializeField]
    private int _width;

    [SerializeField]
    private int _height;

    [SerializeField]
    private Vector2 _gridAnchor;  

    private float _blockSize;


    private void Start()
    {
        _blockSize = _blockPrefab.transform.localScale.x; // same for both x and y
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        float startX = _gridAnchor.x - (_width  * _blockSize) - _blockSize;
        float startY = _gridAnchor.y - (_height * _blockSize);

        var enumArray = Enum.GetValues(typeof(BlockType));


        for (int i = 1; i <= _width; i++)
        {
            for(int j = 1; j <= _height; j++)
            {
                int randomType = UnityEngine.Random.Range(0, enumArray.Length);

                Vector3 spawnPoint = new Vector3(startX + i * _blockSize * 2,startY + j * _blockSize * 2,0);
                var instantiatedBlock = Instantiate(_blockPrefab);
                instantiatedBlock.transform.position = spawnPoint;

                instantiatedBlock.Setup(_blockProperties[randomType],(BlockType)randomType);
            }
        }
    }
    
}
