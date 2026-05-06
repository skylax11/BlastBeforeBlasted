using System;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Block[,] Grid;

    [SerializeField]
    private Block _blockPrefab;

    [SerializeField]
    private BlockSO[] _blockProperties;

    [Header("Grid Infos")]
    public GridInfo GridInfo;

    [SerializeField]
    private int _width;

    [SerializeField]
    private int _height;

    [SerializeField]
    private Vector2 _gridAnchor;  


    public void GenerateGrid()
    {
        Grid = new Block[_width, _height];
        float blockSize = _blockPrefab.transform.localScale.x; // same for both x and y

        float startX = _gridAnchor.x - (_width  * blockSize) - blockSize;
        float startY = _gridAnchor.y - (_height * blockSize) - blockSize;

        var enumArray = Enum.GetValues(typeof(BlockType));

        for (int i = 1; i <= _width; i++)
        {
            for(int j = 1; j <= _height; j++)
            {
                int randomType = UnityEngine.Random.Range(0, enumArray.Length);

                Vector3 spawnPoint = new Vector3(startX + i * blockSize * 2,startY + j * blockSize * 2,0);
                
                Block instantiatedBlock;
                if (BlockPool.Instance != null)
                {
                    instantiatedBlock = BlockPool.Instance.GetBlock(spawnPoint, transform);
                }
                else
                {
                    instantiatedBlock = Instantiate(_blockPrefab, spawnPoint, Quaternion.identity, transform);
                }

                instantiatedBlock.Setup(_blockProperties[randomType],(BlockType)randomType);
                instantiatedBlock.GridX = i - 1;
                instantiatedBlock.GridY = j - 1;
                instantiatedBlock.SetSortingOrder((_height - instantiatedBlock.GridY) * 10);
                instantiatedBlock.name = $"{i-1}-{j-1}";
                Grid[i-1, j-1] = instantiatedBlock;
            }
        }

        float endX = _gridAnchor.x + (_width * blockSize) + blockSize;
        float endY = _gridAnchor.y + (_height * blockSize) + blockSize;

        GridInfo = new GridInfo()
        {
            StartX = startX + blockSize,
            StartY = startY + blockSize,
            EndX = endX - blockSize,
            EndY = endY - blockSize,
            BlockSize = blockSize,
        };
    }

    public BlockSO GetRandomBlockProperty(out BlockType type)
    {
        var enumArray = Enum.GetValues(typeof(BlockType));
        int randomType = UnityEngine.Random.Range(0, enumArray.Length);
        type = (BlockType)randomType;
        return _blockProperties[randomType];
    }
}

public struct GridInfo
{
    public float StartX;
    public float StartY;
    public float EndX;
    public float EndY;
    public float BlockSize;

    public GridInfo(float sX, float sY, float eX, float eY, float blockSize)
    {
        StartX = sX;
        StartY = sY;
        EndX = eX;
        EndY = eY;
        BlockSize = blockSize;
    }
}