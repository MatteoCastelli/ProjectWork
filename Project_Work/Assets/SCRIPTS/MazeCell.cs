using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField] private GameObject _leftWall;

    [SerializeField] private GameObject _rightWall;

    [SerializeField] private GameObject _frontWall;

    [SerializeField] private GameObject _backWall;

    [SerializeField] private GameObject _UnvisitedBlock;

    [SerializeField] public GameObject ground;


    public bool IsVisited { get; private set; }

    public void Visit()
    {
        IsVisited = true;
        _UnvisitedBlock.SetActive(false);
    }

    public void ColorPath(int i)
    {
        Renderer renderer = ground.GetComponent<Renderer>();

        if (i == 0) renderer.material.color = new Color32(28, 224, 13, 255);
        if (i == 1) renderer.material.color = new Color32(255, 165, 0, 255);
        if (i == 2) renderer.material.color = new Color32(242, 79, 15, 255);
        if (i == 3) renderer.material.color= new Color32(242, 38, 15, 255);
        if (i == 4) renderer.material.color = new Color32(209, 13, 13, 255);
    }
    
    public void ClearLeftWall()
    {
        _leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        _rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        _frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        _backWall.SetActive(false);
    }

   
}
