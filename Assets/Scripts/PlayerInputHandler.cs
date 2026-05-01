using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField]
    private GameLogicController _gameLogicController;

    void Update()
    {
        ListenClick();
    }
    private void ListenClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(mousePos);

            _gameLogicController.OnClickedScreen(clickPosition);
        }
    }
}
