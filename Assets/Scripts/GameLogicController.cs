using NUnit.Framework;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public struct BlockGroup
{
    public int GroupId;
    public List<Block> Blocks;

    public BlockGroup(int blockId,List<Block> blocks)
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

    private void Start()
    {
        _gridGenerator.GenerateGrid();

        Block[,] grid = _gridGenerator.Grid;

        bool[,] visited = new bool[grid.GetLength(0), grid.GetLength(1)];


        for(int i = 0; i <  grid.GetLength(0); i++)
        {
            for (int j = 0;  j < grid.GetLength(1); j++)
            {
                if(!visited[i, j])
                {
                    Block block = grid[i, j];

                    BlockGroup newGroup = new BlockGroup()
                    {
                        GroupId = _groups.Count+1,
                        Blocks = new List<Block>()
                    };

                    _groups.Add(newGroup.GroupId, newGroup);

                    GroupBlocks(grid, newGroup, block.Type,visited,i,j);
                }
            }
        }

        GridInfo = _gridGenerator.GridInfo;
    }

    public void GroupBlocks(Block[,] matrix,BlockGroup group ,BlockType targetType ,bool[,] visited ,int x, int y)
    {
        if(x < 0  || y < 0 || x >= matrix.GetLength(0) || y >= matrix.GetLength(1) || visited[x,y]) return;
        
        Block block = matrix[x,y];

        if (block.Type == targetType)
        {
            visited[x, y] = true;

            group.Blocks.Add(block);
            block.BlockId = group.GroupId;
            block.name = group.GroupId.ToString();


            GroupBlocks(matrix, group, targetType, visited, x, y + 1);
            GroupBlocks(matrix, group, targetType, visited, x + 1, y);
            GroupBlocks(matrix, group, targetType, visited, x , y - 1);
            GroupBlocks(matrix, group, targetType, visited, x - 1, y);
        }
    }

    public void OnClickedScreen(Vector3 position)
    {
        float clickedX = position.x;
        float clickedY = position.y;

        if(clickedX < GridInfo.StartX || clickedX > GridInfo.EndX || clickedY < GridInfo.StartY || clickedY > GridInfo.EndY)
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

    }
}
