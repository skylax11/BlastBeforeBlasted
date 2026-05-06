using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockGroup
{
    public int GroupId;
    public List<Block> Blocks;

    public BlockGroup(int blockId, List<Block> blocks)
    {
        GroupId = blockId;
        Blocks = blocks;
    }
}


public class GameLogicController : MonoBehaviour
{
    [SerializeField]
    private GridGenerator _gridGenerator;

    private Dictionary<int, BlockGroup> _groups = new Dictionary<int, BlockGroup>();

    public GridInfo GridInfo;

    private bool _isProcessing = false;

    private void Start()
    {
        _gridGenerator.GenerateGrid();
        GridInfo = _gridGenerator.GridInfo;
        CalculateGroups();
    }

    private void CalculateGroups()
    {
        _groups.Clear();
        Block[,] grid = _gridGenerator.Grid;
        bool[,] visited = new bool[grid.GetLength(0), grid.GetLength(1)];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != null && !visited[i, j])
                {
                    Block block = grid[i, j];

                    BlockGroup newGroup = new BlockGroup()
                    {
                        GroupId = _groups.Count + 1,
                        Blocks = new List<Block>()
                    };

                    _groups.Add(newGroup.GroupId, newGroup);

                    GroupBlocks(grid, newGroup, block.Type, visited, i, j);
                }
            }
        }

        foreach (var group in _groups.Values)
        {
            foreach (var block in group.Blocks)
            {
                block.UpdateVisual(group.Blocks.Count);
            }
        }
    }


    public void GroupBlocks(Block[,] matrix, BlockGroup group, BlockType targetType, bool[,] visited, int x, int y)
    {
        if (x < 0 || y < 0 || x >= matrix.GetLength(0) || y >= matrix.GetLength(1) || visited[x, y] || matrix[x, y] == null) return;

        Block block = matrix[x, y];

        if (block.Type == targetType)
        {
            visited[x, y] = true;

            group.Blocks.Add(block);
            block.GroupId = group.GroupId;
            block.name = group.GroupId.ToString();


            GroupBlocks(matrix, group, targetType, visited, x, y + 1);
            GroupBlocks(matrix, group, targetType, visited, x + 1, y);
            GroupBlocks(matrix, group, targetType, visited, x, y - 1);
            GroupBlocks(matrix, group, targetType, visited, x - 1, y);
        }
    }

    public void OnClickedScreen(Vector3 position)
    {
        if (_isProcessing) return;

        float clickedX = position.x;
        float clickedY = position.y;

        if (clickedX < GridInfo.StartX || clickedX > GridInfo.EndX || clickedY < GridInfo.StartY || clickedY > GridInfo.EndY)
        {
            return;
        }

        int xIndex = (int)Mathf.Floor((clickedX - GridInfo.StartX) / (GridInfo.BlockSize * 2));
        int yIndex = (int)Mathf.Floor((clickedY - GridInfo.StartY) / (GridInfo.BlockSize * 2));

        Block[,] grid = _gridGenerator.Grid;

        if (xIndex < 0 || xIndex >= grid.GetLength(0) || yIndex < 0 || yIndex >= grid.GetLength(1))
        {
            return;
        }

        Block selectedBlock = grid[xIndex, yIndex];

        if (selectedBlock == null) return;

        BlastGroup(_groups[selectedBlock.GroupId]);
    }

    private void BlastGroup(BlockGroup group)
    {
        if (group.Blocks.Count < 2)
        {
            return;
        }

        Block[,] grid = _gridGenerator.Grid;

        for (int i = 0; i < group.Blocks.Count; i++)
        {
            Block block = group.Blocks[i];
            grid[block.GridX, block.GridY] = null;
            BlockPool.Instance.ReturnBlock(block);
        }

        StartCoroutine(FallAndCalculateRoutine());
    }

    private IEnumerator FallAndCalculateRoutine()
    {
        _isProcessing = true;
        
        yield return StartCoroutine(FallBlocksCoroutine());
        yield return StartCoroutine(RefillGridCoroutine());
        
        CalculateGroups();
        
        _isProcessing = false;
    }

    private IEnumerator FallBlocksCoroutine()
    {
        Block[,] grid = _gridGenerator.Grid;
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        List<(Block block, Vector3 targetPos)> movements = new List<(Block, Vector3)>();

        for (int x = 0; x < width; x++)
        {
            int emptySpotY = -1;

            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    if (emptySpotY == -1)
                    {
                        emptySpotY = y;
                    }
                }
                else
                {
                    if (emptySpotY != -1)
                    {
                        // Move block down to empty spot
                        Block block = grid[x, y];
                        grid[x, emptySpotY] = block;
                        grid[x, y] = null;

                        // Update block's grid position
                        block.GridY = emptySpotY;
                        block.SetSortingOrder((height - block.GridY) * 10);

                        // Calculate target world position
                        float gridOriginY = GridInfo.StartY - GridInfo.BlockSize;
                        Vector3 targetPos = block.transform.position;
                        targetPos.y = gridOriginY + (block.GridY + 1) * GridInfo.BlockSize * 2;
                        
                        movements.Add((block, targetPos));

                        // The new empty spot is the one above the previous one
                        emptySpotY++;
                    }
                }
            }
        }

        if (movements.Count > 0)
        {
            float duration = 0.2f;
            float elapsed = 0;
            
            Dictionary<Block, Vector3> startPositions = new Dictionary<Block, Vector3>();
            foreach(var m in movements) startPositions[m.block] = m.block.transform.position;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                t = t * t; 

                foreach (var m in movements)
                {
                    if (m.block != null)
                        m.block.transform.position = Vector3.Lerp(startPositions[m.block], m.targetPos, t);
                }
                yield return null;
            }

            foreach (var m in movements)
            {
                if (m.block != null)
                    m.block.transform.position = m.targetPos;
            }
        }
    }

    private IEnumerator RefillGridCoroutine()
    {
        Block[,] grid = _gridGenerator.Grid;
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        List<(Block block, Vector3 targetPos)> movements = new List<(Block, Vector3)>();

        float startX = GridInfo.StartX - GridInfo.BlockSize;
        float startY = GridInfo.StartY - GridInfo.BlockSize;

        for (int x = 0; x < width; x++)
        {
            int missingCount = 0;
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    missingCount++;
                    
                    BlockType type;
                    BlockSO props = _gridGenerator.GetRandomBlockProperty(out type);

                    Vector3 targetPos = new Vector3(startX + (x + 1) * GridInfo.BlockSize * 2, startY + (y + 1) * GridInfo.BlockSize * 2, 0);
                    // Spawn above the highest point
                    Vector3 spawnPos = new Vector3(targetPos.x, startY + (height + missingCount + 1) * GridInfo.BlockSize * 2, 0);

                    Block newBlock = BlockPool.Instance.GetBlock(spawnPos, _gridGenerator.transform);
                    newBlock.Setup(props, type);
                    newBlock.GridX = x;
                    newBlock.GridY = y;
                    newBlock.SetSortingOrder((height - newBlock.GridY) * 10);
                    newBlock.name = $"{x}-{y}";

                    grid[x, y] = newBlock;
                    movements.Add((newBlock, targetPos));
                }
            }
        }

        if (movements.Count > 0)
        {
            float duration = 0.3f;
            float elapsed = 0;
            Dictionary<Block, Vector3> startPositions = new Dictionary<Block, Vector3>();
            foreach (var m in movements) startPositions[m.block] = m.block.transform.position;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                t = t * t;

                foreach (var m in movements)
                {
                    if (m.block != null)
                        m.block.transform.position = Vector3.Lerp(startPositions[m.block], m.targetPos, t);
                }
                yield return null;
            }

            foreach (var m in movements)
            {
                if (m.block != null)
                    m.block.transform.position = m.targetPos;
            }
        }
    }
}
