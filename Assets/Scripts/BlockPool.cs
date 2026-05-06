using System.Collections.Generic;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
    public static BlockPool Instance;

    [SerializeField]
    private Block _blockPrefab;

    private Queue<Block> _pool = new Queue<Block>();

    private void Awake()
    {
        Instance = this;
    }

    public Block GetBlock(Vector3 position, Transform parent)
    {
        Block block;
        if (_pool.Count > 0)
        {
            block = _pool.Dequeue();
            block.gameObject.SetActive(true);
            block.transform.position = position;
            block.transform.SetParent(parent);
        }
        else
        {
            block = Instantiate(_blockPrefab, position, Quaternion.identity, parent);
        }
        return block;
    }

    public void ReturnBlock(Block block)
    {
        block.gameObject.SetActive(false);
        _pool.Enqueue(block);
    }
}
