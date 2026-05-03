using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;

    private BlockSO _properties;

    private BlockType _type;

    public BlockType Type => _type;

    public int BlockId;

    public void Setup(BlockSO properties, BlockType type)
    {
        this._properties = properties;
        this._type = type;

        _renderer.sprite = _properties.Evolutions[0];
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
