using UnityEditorInternal;
using UnityEngine;

public class GameLogicController : MonoBehaviour
{
    [SerializeField]
    private GridGenerator _gridGenerator;

    public GridInfo GridInfo;

    private void Start()
    {
        _gridGenerator.GenerateGrid();

        GridInfo = _gridGenerator.GridInfo;

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

        print(selectedBlock.name);

    }
}
