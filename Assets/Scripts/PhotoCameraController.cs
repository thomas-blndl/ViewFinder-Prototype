using EzySlice;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Plane = UnityEngine.Plane;
using DynamicShadowProjector;

public class PhotoCameraController : MonoBehaviour
{
    private StarterAssetsInputs _input;
    public Camera camera;
    Plane[] cameraFrustum;
    GameObject[] allObjects;
    List<GameObject> objectsInSight = new List<GameObject>();
    public bool hasPicture = false;
    public GameObject photo;
    public GameObject HoldingPosPicture;
    public GameObject TakingPosPicture;
    public GameObject prefabPicture;
    public RenderTexture cameraView;
    [HideInInspector]
    public Vector3[] currNearCorners;
    [HideInInspector]
    public Vector3[] currFarCorners;
    public GameObject viewFinder;
    private bool isCameraActivated = false;
    public GameObject lePlaneChiant;
    public GameObject planeGO;

    public GameObject[] cameraPlanes;

    public GameObject shadowProjector;

    void Start()
    {
        camera.transform.position = Camera.main.transform.position;
        camera.transform.rotation = Camera.main.transform.rotation;
        DrawFrustum(camera);
        _input = transform.root.GetComponent<StarterAssetsInputs>();
        photo.SetActive(false);
        allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        ActivateCamera(false);

        InitializePlanes();
    }

    private void InitializePlanes()
    {
        Vector3[] VerteicesArray = new Vector3[3];
        int[] trianglesArray = new int[3];

        trianglesArray[0] = 0;
        trianglesArray[1] = 1;
        trianglesArray[2] = 2;

        Mesh m = new Mesh();
        GameObject currPlane;
        cameraPlanes = Array.Empty<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            m = new Mesh();

            if (i == 3)
            {
                VerteicesArray[0] = transform.position;
                VerteicesArray[1] = currFarCorners[i];
                VerteicesArray[2] = currFarCorners[0];
            }
            else
            {
                VerteicesArray[0] = transform.position;
                VerteicesArray[1] = currFarCorners[i];
                VerteicesArray[2] = currFarCorners[i+1];
            }
            

            //add these two triangles to the mesh
            m.vertices = VerteicesArray;
            m.triangles = trianglesArray;

            currPlane = Instantiate(planeGO, Vector3.zero, Quaternion.identity);
            currPlane.GetComponent<MeshFilter>().mesh = m;
            currPlane.GetComponent<MeshCollider>().sharedMesh = m;
            cameraPlanes = cameraPlanes.Append(currPlane).ToArray();
        }

        VerteicesArray = new Vector3[4];
        trianglesArray = new int[6];

        trianglesArray[0] = 0;
        trianglesArray[1] = 1;
        trianglesArray[2] = 2;
        trianglesArray[3] = 0;
        trianglesArray[4] = 2;
        trianglesArray[5] = 3;
        m = new Mesh();

        VerteicesArray[0] = currFarCorners[0];
        VerteicesArray[1] = currFarCorners[1];
        VerteicesArray[2] = currFarCorners[2];
        VerteicesArray[3] = currFarCorners[3];        

        m.vertices = VerteicesArray;
        m.triangles = trianglesArray;

        currPlane = Instantiate(planeGO, Vector3.zero, Quaternion.identity);
        currPlane.GetComponent<MeshFilter>().mesh = m;
        cameraPlanes = cameraPlanes.Append(currPlane).ToArray();
        
        Mesh newMesh = CombineAndDestroyMeshes(cameraPlanes);
        planeGO.GetComponent<MeshFilter>().mesh = newMesh;
        planeGO.GetComponent<MeshCollider>().sharedMesh = newMesh;
        planeGO.GetComponent<MeshCollider>().convex = true;
        planeGO.GetComponent<MeshCollider>().isTrigger = true;
    }

    private Mesh CombineAndDestroyMeshes(GameObject[] sourcesGO)
    {
        MeshFilter[] sourceMeshFilters = Array.Empty<MeshFilter>();
        foreach (GameObject go in sourcesGO)
        {
            sourceMeshFilters = sourceMeshFilters.Append(go.GetComponent<MeshFilter>()).ToArray();
            Destroy(go);
        }

        var combine = new CombineInstance[sourceMeshFilters.Length];

        for (int i = 0; i < sourceMeshFilters.Length; i++)
        {
            combine[i].mesh = sourceMeshFilters[i].sharedMesh;
            combine[i].transform = sourceMeshFilters[i].transform.localToWorldMatrix;
        }

        var mesh = new Mesh();
        mesh.CombineMeshes(combine);
        return mesh;
    }

    private void Update()
    {
        camera.transform.position = Camera.main.transform.position;
        camera.transform.rotation = Camera.main.transform.rotation;
        DrawFrustum(camera);
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(Camera.main);



        // objectsInSight = Array.Empty<GameObject>();
        // foreach (GameObject go in allObjects)
        // {
        //     if (go == null) continue;
        //     if (go.tag == "IgnorePicture" || go.tag == "Player" || go.tag == "MainCamera")
        //         continue;

        //     if (go.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
        //     {
        //         if (go.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
        //         {
        //             Vector3[] vertices = lePlaneChiant.GetComponent<MeshFilter>().mesh.vertices;

        //             for (int i = 0; i < 3; i++)
        //             {
        //                 vertices[i] = lePlaneChiant.transform.TransformPoint(vertices[i]);
        //             }

        //             var bounds = mr.bounds;
        //             Vector3 position = transform.InverseTransformPoint(go.transform.position);
        //             if (GeometryUtility.TestPlanesAABB(cameraFrustum, bounds)
        //             //&& position.z > 0 && position.y < 0
        //             //&& position.x > -5f && position.x < 5f 
        //             //&& (position.x > -2 && position.x < 2)
        //             //&& (position.y > -2 && position.y < 2)
        //             // && Vector3.Dot(transform.position - go.transform.position, go.transform.forward) > 0    
        //             )
        //             {
        //                 //Debug.Log(position);
        //                 go.GetComponent<MeshRenderer>().material.color = Color.green;
        //                 objectsInSight = objectsInSight.Append(go).ToArray();

        //             }
        //             else
        //                 go.GetComponent<MeshRenderer>().material.color = Color.red;
        //         }
        //     }
        // }
        #region inputs
        if (hasPicture)
        {
            if (_input.lookPicture)
            {
                LookPicture();
                ActivateCamera(false);
                _input.picture = false;

            }
            else
            {
                ActivateCamera(true);
                HoldPicture();
            }
        }
        if (_input.usePicture)
        {
            UsePicture();
            _input.usePicture = false;
        }
        if (_input.picture)
        {
            if (isCameraActivated)
                TakePicture();
            _input.picture = false;
        }
        if (_input.activateCamera)
        {
            isCameraActivated = !isCameraActivated;
            ActivateCamera(isCameraActivated);
            _input.activateCamera = false;
        }
        #endregion
    }

    private void TakePicture()
    {
        prefabPicture = new GameObject("PrefabPicture");
        prefabPicture.transform.parent = Camera.main.gameObject.transform;

        //copy CameraView
        RenderTexture copytexture;
        copytexture = new RenderTexture(4096, 4096, 0);
        copytexture.enableRandomWrite = true;
        RenderTexture.active = copytexture;
        Graphics.Blit(cameraView, copytexture);

        photo.SetActive(true);
        photo.GetComponent<Image>().material.mainTexture = copytexture;

        if (prefabPicture == null) return;

        objectsInSight = CollisionHandler.collidingObjects;
        foreach (GameObject go in objectsInSight)
        {
            if (go == null) continue;
            Debug.Log(go.GetComponent<Renderer>().lightmapIndex);
            // go.Slice(planeGO.transform.position, planeGO.transform.up);

            // continue;
            GameObject newGo;
            GameObject sliced = Slice(go, true);

            if (sliced == null)
            {
                newGo = GameObject.Instantiate(go, go.transform.position, go.transform.rotation, prefabPicture.transform);
            }
            else
            {
                newGo = sliced;
                Destroy(sliced);
                newGo = GameObject.Instantiate(newGo, newGo.transform.position, newGo.transform.rotation, prefabPicture.transform);
            }
            newGo.transform.position = prefabPicture.transform.TransformPoint(go.transform.position);
            newGo.SetActive(false);
            newGo.tag = "IgnorePicture";
            newGo.GetComponent<MeshFilter>().mesh.uv = go.GetComponent<MeshFilter>().mesh.uv;
        }
        PrefabUtility.SaveAsPrefabAsset(prefabPicture, "Assets/Prefabs/picturePrefab.prefab", out bool success);

        hasPicture = true;

        StartCoroutine("MovePictureTowardPosition", HoldingPosPicture.GetComponent<RectTransform>().position);
        StartCoroutine("ShrinkOrGrowPicture", true);
    }

    private IEnumerator MovePictureTowardPosition(Vector3 position)
    {
        Transform photo = this.photo.transform.parent;
        float speed = 1000f;
        Vector3 picturePos = photo.position;
        while (Vector3.Distance(picturePos, position) > 0f)
        {
            picturePos = photo.position;
            photo.position = Vector3.MoveTowards(picturePos, new Vector3(position.x, position.y, picturePos.z), Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    private IEnumerator ShrinkOrGrowPicture(bool shrink)
    {
        Vector3 scaleChange = new Vector3(0.1f, 0.1f, 0f);
        float multiplier = 0.05f;
        Vector3 picturePos = photo.transform.position;
        if (shrink)
        {
            while (photo.transform.localScale.x > 0.5f)
            {
                photo.transform.localScale -= scaleChange * multiplier;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (photo.transform.localScale.x < 1f)
            {
                photo.transform.localScale += scaleChange * multiplier;
                yield return new WaitForEndOfFrame();
            }
        }

        yield return null;
    }

    public void UsePicture()
    {
        if (!hasPicture) return;
        //deletes and cut object in sight
        foreach (GameObject go in objectsInSight)
        {
            if (go == null) continue;

            GameObject sliced = Slice(go, false);
            Destroy(go);
        }

        //Instantiate new objects
        if (prefabPicture != null && prefabPicture.transform.childCount > 0)
        {
            GameObject instanciated = Instantiate(prefabPicture, prefabPicture.transform.position, prefabPicture.transform.rotation);
            if (instanciated == null) return;
            for (int i = 0; i < instanciated.transform.childCount; i++)
            {
                GameObject child = instanciated.transform.GetChild(i).gameObject;
                child.tag = "Untagged";
                child.SetActive(true);
                child.AddComponent<MeshCollider>();
                child.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                GameObject currShadowProj = Instantiate(shadowProjector,child.transform.position, Quaternion.identity, child.transform);
                DrawTargetObject drawTargetObject = currShadowProj.GetComponent<DrawTargetObject>();
                drawTargetObject.target = child.transform;
                drawTargetObject.targetDirection = RenderSettings.sun.transform;
            }
            // instanciated.transform.DetachChildren();
            // Destroy(instanciated);
        }
        Destroy(prefabPicture);
        photo.SetActive(false);
        hasPicture = false;
        allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        return;
    }

    public void LookPicture()
    {
        StopCoroutine("MovePictureTowardPosition");
        StopCoroutine("ShrinkOrGrowPicture");
        StartCoroutine("MovePictureTowardPosition", TakingPosPicture.GetComponent<RectTransform>().position);
        StartCoroutine("ShrinkOrGrowPicture", false);
    }

    public void HoldPicture()
    {
        StopCoroutine("MovePictureTowardPosition");
        StopCoroutine("ShrinkOrGrowPicture");
        StartCoroutine("MovePictureTowardPosition", HoldingPosPicture.GetComponent<RectTransform>().position);
        StartCoroutine("ShrinkOrGrowPicture", true);
    }

    private void ActivateCamera(bool activate)
    {
        viewFinder.SetActive(activate);
    }

    void DrawFrustum(Camera cam)
    {
        Vector3[] nearCorners = new Vector3[4]; //Approx'd nearplane corners
        Vector3[] farCorners = new Vector3[4]; //Approx'd farplane corners
        Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(cam); //get planes from matrix

        Plane temp = camPlanes[1]; camPlanes[1] = camPlanes[2]; camPlanes[2] = temp; //swap [1] and [2] so the order is better for the loop
        for (int i = 0; i < 4; i++)
        {
            nearCorners[i] = Plane3Intersect(camPlanes[4], camPlanes[i], camPlanes[(i + 1) % 4]); //near corners on the created projection matrix
            farCorners[i] = Plane3Intersect(camPlanes[5], camPlanes[i], camPlanes[(i + 1) % 4]); //far corners on the created projection matrix
        }

        for (int i = 0; i < 4; i++)
        {
            //Debug.DrawLine( nearCorners[i], nearCorners[( i + 1 ) % 4], Color.red, Time.deltaTime, true ); //near corners on the created projection matrix
            //Debug.DrawLine( farCorners[i], farCorners[( i + 1 ) % 4], Color.blue, Time.deltaTime, true ); //far corners on the created projection matrix
            //Debug.DrawLine( nearCorners[i], farCorners[i], Color.green, Time.deltaTime, true ); //sides of the created projection matrix
        }

        currFarCorners = farCorners;
        currNearCorners = nearCorners;
    }



    Vector3 Plane3Intersect(Plane p1, Plane p2, Plane p3)
    { //get the intersection point of 3 planes
        return ((-p1.distance * Vector3.Cross(p2.normal, p3.normal)) +
                (-p2.distance * Vector3.Cross(p3.normal, p1.normal)) +
                (-p3.distance * Vector3.Cross(p1.normal, p2.normal))) /
        (Vector3.Dot(p1.normal, Vector3.Cross(p2.normal, p3.normal)));
    }

    public GameObject Slice(GameObject target, bool getUpper)
    {
        bool hasSliced = false;
        GameObject currTarget = null;

        GameObject upperHull = null;
        GameObject lowerHull = null;
        for (int i = 0; i < currNearCorners.Length; i++)
        {
            if (hasSliced)
            {
                target = currTarget;
                //if(getUpper)
                Destroy(currTarget);
            }

            Material sliceMat = target.GetComponent<Renderer>().material;
            Vector3 norm = i + 1 >= currNearCorners.Length ?
                (currFarCorners[i] + currFarCorners[0]) / 2.0f : (currFarCorners[i] + currFarCorners[i + 1]) / 2.0f;
            Quaternion direction = Quaternion.LookRotation(norm);

            if (i == 1 || i == 3)
                direction *= Quaternion.Euler(0, 0, 90);
            if (i == 3 || i == 2)
                direction *= Quaternion.Euler(0, 0, 180);
            SlicedHull hull = target.Slice(transform.position, direction * Vector3.up);

            if (hull != null)
            {
                if (getUpper)
                {
                    upperHull = hull.CreateUpperHull(target, sliceMat);
                    currTarget = upperHull;
                }
                else
                {
                    //upperHull = hull.CreateUpperHull(target, sliceMat);
                    lowerHull = hull.CreateLowerHull(target, sliceMat);
                    currTarget = target;
                    lowerHull.AddComponent<MeshCollider>();
                    //Destroy(upperHull);
                }
                hasSliced = true;
            }
            else
            {
                Debug.Log("Hull en null !");
            }
        }
        return currTarget;
    }
}
