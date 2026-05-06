using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;

    private BlockSO _properties;

    private BlockType _type;

    public BlockType Type => _type;

    public int GroupId;
    public int GridX;
    public int GridY;

    [Header("Thresholds")]

    [SerializeField]
    private int A;

    [SerializeField]
    private int B;

    [SerializeField]
    private int C;


    public void Setup(BlockSO properties, BlockType type)
    {
        this._properties = properties;
        this._type = type;

        _renderer.sprite = _properties.Evolutions[0];
    }

    public void UpdateVisual(int c)
    {
        if (c > C)
        {
            _renderer.sprite = _properties.Evolutions[3];
        }
        else if (c > B)
        {
            _renderer.sprite = _properties.Evolutions[2];
        }
        else if (c > A)
        {
            _renderer.sprite = _properties.Evolutions[1];
        }
        else
        {
            _renderer.sprite = _properties.Evolutions[0];
        }
    }

    public void SetSortingOrder(int order)
    {
        _renderer.sortingOrder = order;
    }

}
public enum BlockType
{
    Blue,
    Green,
    Red,
    Pink,
    Purple,
    Yellow
}
