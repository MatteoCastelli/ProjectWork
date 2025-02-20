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

    public void ColorMain()
    {
        Renderer renderer = ground.GetComponent<Renderer>();
        Material nuovoMateriale = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        nuovoMateriale.color = Color.green;
        renderer.material = nuovoMateriale;
        
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
