using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public class MeshGenerator : MonoBehaviour
{
    #region --- helper ---
    private enum enumQuad
    {
        ABDC_back = 0,      //each quad has 2 triangles, 6 verts
        EFHG_front = 6,
        EFBA_top = 12,
        GHCD_bottom = 18,
        EAGC_left = 24,
        FBDH_right = 30,
    }
    private class Point
    {
        public static Vector3 A = new Vector3(-0.5f,      OFFSET, 0);
        public static Vector3 B = new Vector3(0.5f, OFFSET, 0);
        public static Vector3 C = new Vector3(-0.5f,      0,      0);
        public static Vector3 D = new Vector3(0.5f, 0,      0);
        public static Vector3 E = new Vector3(0,      OFFSET, OFFSET);
        public static Vector3 F = new Vector3(OFFSET, OFFSET, OFFSET);
        public static Vector3 G = new Vector3(0,      0,      OFFSET);
        public static Vector3 H = new Vector3(OFFSET, 0,      OFFSET);
    }
    #endregion

    /*
        E ---------- F
        |    A --------- B
        |    |       |   |
        |    |       |   |
        G ---|------ H   |
             C --------- D

        C is the origin point for the mesh
    */

    private const float OFFSET = 1f;

    public Material material = null;
    private Vector3[] vertices = new Vector3[36]; //triangle 3, quad 6, cube 36
    private int[] triangles = new int[36];
    private Vector2[] uv = new Vector2[36];
    private Vector3[] nearCorners;
    private Vector3[] farCorners;
    private PhotoCameraController photoCameraController;
    private GameObject meshGo;

    private static MeshGenerator INSTANCE;


    private void Start()
    {
        INSTANCE = this;

        photoCameraController = GetComponent<PhotoCameraController>();
        farCorners = GetComponent<PhotoCameraController>().currFarCorners;
        nearCorners = GetComponent<PhotoCameraController>().currNearCorners;
    }

    private void Update() {
        farCorners = GetComponent<PhotoCameraController>().currFarCorners;
        nearCorners = GetComponent<PhotoCameraController>().currNearCorners;
        if (nearCorners.Length > 0 && farCorners.Length > 0)
        {
            Point.A = Vector3.zero;
            Point.B = Vector3.zero;
            Point.C = Vector3.zero;
            Point.D = Vector3.zero;
            Point.E += farCorners[0];
            Point.F += farCorners[3];
            Point.G += farCorners[1];
            Point.H += farCorners[2];
            DefineMeshData();
            Mesh mesh = GetMesh();
            if(mesh == null) return;
            SetMesh(mesh);
        }
    }

    private void DefineMeshData()
    {
        foreach (enumQuad code in System.Enum.GetValues(typeof(enumQuad)))
        {
            DefineQuad(code);
        }
    }
    private Mesh GetMesh()
    {
        Mesh m = null;
        // if (!this.gameObject.TryGetComponent<MeshFilter>(out MeshFilter mf))
        // {
        //     mf = this.gameObject.AddComponent<MeshFilter>();
        // }

        // if (!this.gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
        // {
        //     mr = this.gameObject.AddComponent<MeshRenderer>();
        // }

        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();
        if(mr == null || mf == null) return m;
        mr.material = material;
        if (Application.isEditor == true)
        {
            m = mf.sharedMesh;
            if (m == null)
            {
                mf.sharedMesh = new Mesh();
                m = mf.sharedMesh;
            }
        }
        else
        {
            m = mf.mesh;
            if (m == null)
            {
                mf.mesh = new Mesh();
                m = mf.mesh;
            }
        }

        return m;
    }
    private void SetMesh(Mesh m)
    {
        m.Clear();

        m.vertices = vertices;
        m.triangles = triangles;
        m.uv = uv;

        m.RecalculateNormals();
        m.RecalculateBounds();
        m.RecalculateTangents();
    }
    private void DefineQuad(enumQuad code)
    { 
        switch (code)
        {
            case enumQuad.ABDC_back:
                vertices[0] = Point.A; //ADC
                vertices[1] = Point.D;
                vertices[2] = Point.C;
                vertices[3] = Point.A; //ABD
                vertices[4] = Point.B;
                vertices[5] = Point.D;
                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 3;
                triangles[4] = 4;
                triangles[5] = 5;
                uv[0] = new Vector2(0, 1);
                uv[1] = new Vector2(1, 0);
                uv[2] = new Vector2(0, 0);
                uv[3] = new Vector2(0, 1);
                uv[4] = new Vector2(1, 1);
                uv[5] = new Vector2(1, 0);
                break;
            case enumQuad.EFHG_front:
                vertices[6] = Point.F; //FGH
                vertices[7] = Point.G;
                vertices[8] = Point.H;
                vertices[9] = Point.F; //FEG
                vertices[10] = Point.E;
                vertices[11] = Point.G;
                triangles[6] = 6;
                triangles[7] = 7;
                triangles[8] = 8;
                triangles[9] = 9;
                triangles[10] = 10;
                triangles[11] = 11;
                uv[6] = new Vector2(0, 1);
                uv[7] = new Vector2(1, 0);
                uv[8] = new Vector2(0, 0);
                uv[9] = new Vector2(0, 1);
                uv[10] = new Vector2(1, 1);
                uv[11] = new Vector2(1, 0);
                break;
            case enumQuad.EFBA_top:
                vertices[12] = Point.E; //EBA
                vertices[13] = Point.B;
                vertices[14] = Point.A;
                vertices[15] = Point.E; //EFB
                vertices[16] = Point.F;
                vertices[17] = Point.B;
                triangles[12] = 12;
                triangles[13] = 13;
                triangles[14] = 14;
                triangles[15] = 15;
                triangles[16] = 16;
                triangles[17] = 17;
                uv[12] = new Vector2(0, 1);
                uv[13] = new Vector2(1, 0);
                uv[14] = new Vector2(0, 0);
                uv[15] = new Vector2(0, 1);
                uv[16] = new Vector2(1, 1);
                uv[17] = new Vector2(1, 0);
                break;
            case enumQuad.GHCD_bottom:
                vertices[18] = Point.C; //CHG
                vertices[19] = Point.H;
                vertices[20] = Point.G;
                vertices[21] = Point.C; //CDH
                vertices[22] = Point.D;
                vertices[23] = Point.H;
                triangles[18] = 18;
                triangles[19] = 19;
                triangles[20] = 20;
                triangles[21] = 21;
                triangles[22] = 22;
                triangles[23] = 23;
                uv[18] = new Vector2(0, 1);
                uv[19] = new Vector2(1, 0);
                uv[20] = new Vector2(0, 0);
                uv[21] = new Vector2(0, 1);
                uv[22] = new Vector2(1, 1);
                uv[23] = new Vector2(1, 0);
                break;
            case enumQuad.EAGC_left:
                vertices[24] = Point.E; //ECG
                vertices[25] = Point.C;
                vertices[26] = Point.G;
                vertices[27] = Point.E; //EAC
                vertices[28] = Point.A;
                vertices[29] = Point.C;
                triangles[24] = 24;
                triangles[25] = 25;
                triangles[26] = 26;
                triangles[27] = 27;
                triangles[28] = 28;
                triangles[29] = 29;
                uv[24] = new Vector2(0, 1);
                uv[25] = new Vector2(1, 0);
                uv[26] = new Vector2(0, 0);
                uv[27] = new Vector2(0, 1);
                uv[28] = new Vector2(1, 1);
                uv[29] = new Vector2(1, 0);
                break;
            case enumQuad.FBDH_right:
                vertices[30] = Point.B; //BHD
                vertices[31] = Point.H;
                vertices[32] = Point.D;
                vertices[33] = Point.B; //BFH
                vertices[34] = Point.F;
                vertices[35] = Point.H;
                triangles[30] = 30;
                triangles[31] = 31;
                triangles[32] = 32;
                triangles[33] = 33;
                triangles[34] = 34;
                triangles[35] = 35;
                uv[30] = new Vector2(0, 1);
                uv[31] = new Vector2(1, 0);
                uv[32] = new Vector2(0, 0);
                uv[33] = new Vector2(0, 1);
                uv[34] = new Vector2(1, 1);
                uv[35] = new Vector2(1, 0);
                break;
        }
    }
    public static void SaveMesh (Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh) {
		string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
		if (string.IsNullOrEmpty(path)) return;
        
		path = FileUtil.GetProjectRelativePath(path);

		Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;
		
		if (optimizeMesh)
            UnityEditor.MeshUtility.Optimize(meshToSave);
        
		AssetDatabase.CreateAsset(meshToSave, path);
		AssetDatabase.SaveAssets();
	}
}
