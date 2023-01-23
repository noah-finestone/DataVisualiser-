using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using System.Linq;
using System.IO; 
using System.Text; 
using System.Threading.Tasks;
using Newtonsoft.Json;


 
public class MyParsedJSON
{
    public string name {get;set;}
    public List<float> Values {get;set;}
    public List<Point> Points {get;set;}
} 
public class Point
{
    public float x{get;set;}
    public float y {get;set;} 
    public float z{get;set;}
}

//always a mesh filter on script // 
// [RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles; 

    public int xSize = 20;
    public int zSize = 20;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(GetData());
        GetData();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //StartCoroutine(CreateShape());
        CreateModel();
         //tell mesh to use data 
        UpdateModel();
        
    }

    private List<Point> GetData()
    {      
        List<Point> myPoints = new List<Point>();
            // StreamReader sr1 = new StreamReader(@"C:/Users/Noah.Finestone/DataVisualiser/Assets/fake.json");
            string jsonString = File.ReadAllText(@"C:/Users/Noah.Finestone/DataVisualiser/Assets/testme2.json");
            MyParsedJSON myImportData =   JsonConvert.DeserializeObject<MyParsedJSON>(jsonString);
            for(int i = 0; i < myImportData.Points.Count; ++i){
                //Debug.Log("X value =  " + i + " " + myImportData.Points[i].x);
                myPoints.Add( myImportData.Points[i]);
 
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Plane);
                Vector3 myVec = new Vector3( myImportData.Points[i].x,  myImportData.Points[i].z +  (myImportData.Values[i]),  myImportData.Points[i].y);
                cube.transform.position = myVec;
                cube.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
               // cube.GetComponent<MeshRenderer>().material.color = new Color(0.5f,0.5f,0.5f);

                Debug.Log(myImportData.Values[i]);

                Color c = RemapValuesForColors(myImportData.Values[i], 1f, 65f, 0.7f, 0.0f);
                
                cube.GetComponent<MeshRenderer>().material.color = c; 
            }
            return myPoints;
    }


      // remap a number to a given domain
    public Color RemapValuesForColors(float valueA, float low1, float high1, float low2, float high2)
    {
        float mappedPeak = low2 + (high2 - low2) * (valueA - low1) / (high1 - low1);
        return HSVtoRGB(new Vector3(mappedPeak, 1, 1));
    }
    // convert a Vector3 hue, saturation, brightness to red, green blue values 	
    private Color HSVtoRGB(Vector3 HSV)
    {
        Vector3 H = Hue(HSV.x);
        H = new Vector3(H.x - 1, H.y - 1, H.z - 1) * HSV.y;
        H = new Vector3(H.x + 1, H.y + 1, H.z + 1) * HSV.z;
        return new Color(H.x, H.y, H.z, 1);
    }
    // algorithm to convert hue value for RGB [used in HSVtoRGB function]
    private Vector3 Hue(float H)
    {
        float R = Mathf.Abs(H * 6 - 3) - 1;
        float G = 2 - Mathf.Abs(H * 6 - 2);
        float B = 2 - Mathf.Abs(H * 6 - 4);
        return new Vector3(Mathf.Clamp01(R), Mathf.Clamp01(G), Mathf.Clamp01(B));
    }
  
    void CreateModel()
    {
        //if we had three squares we need four vertices 
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        //loop over each vertices and assigne them a position on the grid
        //from left to right  
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            //loop over all squares on x 
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z *.3f) * 2f;
                //0 on y for flat plane 
                vertices[i] = new Vector3(x, y, z);
                i++;
            }    
        } 

        //6 points for each square (two triangles)
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0; 

        for (int z = 0; z < zSize; z++)
        {
            //loop that iterates through all the squares on the x
            for (int x = 0; x < xSize; x++)
            {
            triangles[tris + 0] = vert + 0; 
            triangles[tris + 1] = vert + xSize + 1;
            triangles[tris + 2] = vert + 1; 
            triangles[tris + 3] = vert + 1;
            triangles[tris + 4] = vert + xSize + 1; 
            triangles[tris + 5] = vert + xSize + 2;

            // shift all triangles one to the right
            vert++;
            tris += 6; 
            }
           vert++;
        }

          
    }
 
    void UpdateModel()
    {
        //clear mesh from any previous data
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles; 

        mesh.RecalculateNormals(); 
    } 
   
    
    void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        //if we have no vertices 
        if (vertices == null)
            return; 

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
            
    }
    
}

