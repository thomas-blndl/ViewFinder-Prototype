using UnityEngine;

public class testAABB : MonoBehaviour
{
    MeshRenderer mr;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (GeometryUtility.TestPlanesAABB(planes, mr.bounds))
        {
            mr.material.color = Color.green;
        }
        else
        {
            mr.material.color = Color.red;
        }
    }
}
