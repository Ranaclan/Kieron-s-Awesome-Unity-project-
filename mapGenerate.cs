using UnityEngine;

public class mapGenerate : MonoBehaviour
{
    //heightmap
    public int octaves = 4;
    public float heightmapScale = 0.8f;
    public float heightmapAmplitudeMultiplier = 4.5f;
    public float heightmapFrequencyMultiplier = 0.3f;
    public int offset;
    private float max = 0f;
    private float min = 0f;
    //mesh
    Mesh mesh;
    MeshRenderer meshRenderer;
    public int width = 20;
    public int depth = 20;
    Vector3[] vertices;
    int[] triangles;
    //texture
    private Texture2D texture;
    private Vector2[] uvMap;
    private Color[] colourMap;
    //colourmap
    public float colourmapAmplitude = 2f;
    public float colourmapFrequency = 0.1f;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = GetComponent<MeshRenderer>();

        MeshData();
        UpdateMesh();
    }

    void Update()
    {
        MeshData();
        UpdateMesh();
    }

    void MeshData()
    {
        vertices = new Vector3[(width + 1) * (depth + 1)];
        uvMap = new Vector2[vertices.Length];
        colourMap = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                //vertices
                Vertices(x, z, i);

                //texture
                Texture(x, z, i);

                i++;
            }
        }

        //triangles
        SetTriangles();

        //texture
        SetTexture();
    }

    void Vertices(int x, int z, int i)
    {
        //create height map
        //values
        Vector3 noiseValue = Vector3.zero;
        float amplitude = 1;
        float frequency = 1;

        //loop through octaves, adding in less significant and more consistent detail with each
        for (int octave = 0; octave < octaves; octave++)
        {
            //perlin noise
            noiseValue += new Vector3(x, Perlin(x, z, frequency, amplitude, heightmapScale, offset), z);

            frequency *= heightmapFrequencyMultiplier;
            amplitude *= heightmapAmplitudeMultiplier;
        }

        vertices[i] = noiseValue;

        if (noiseValue.y > max)
        {
            max = noiseValue.y;
        }
        else if (noiseValue.y < min)
        {
            min = noiseValue.y;
        }
    }

    float Perlin(int x, int z, float frequency, float amplitude, float scale, int offset)
    {
        float perlinx = (float)x / scale * frequency + offset;
        float perlinz = (float)z / scale * frequency + offset;

        float perlin = Mathf.PerlinNoise(perlinx, perlinz) * amplitude;

        return perlin;
    }

    void Texture(int x, int z, int i)
    {
        //create uvs of mesh
        uvMap[i] = new Vector2(x / (float)width, z / (float)depth);

        //create mesh colour map
        colourMap[z * width + x] = new Color(Mathf.Lerp(0, 255, Perlin(x, z, colourmapFrequency, colourmapAmplitude, heightmapScale, 0)), 0, 0);
        Debug.Log(colourMap[z * width + x].r);
    }

    void SetTriangles()
    {
        //create triangles of mesh
        triangles = new int[width * depth * 6];
        int vertex = 0;
        int triangle = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[triangle] = vertex;
                triangles[triangle + 1] = vertex + width + 1;
                triangles[triangle + 2] = vertex + 1;
                triangles[triangle + 3] = vertex + 1;
                triangles[triangle + 4] = vertex + width + 1;
                triangles[triangle + 5] = vertex + width + 2;

                vertex++;
                triangle += 6;
            }

            vertex++;
        }
    }

    void SetTexture()
    {
        //create texture with colourmap and apply to mesh
        texture = new Texture2D(width, depth);
        texture.SetPixels(colourMap);
        texture.Apply();

        meshRenderer.material.mainTexture = texture;
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.uv = uvMap;
        mesh.colors = colourMap;

        mesh.RecalculateNormals();
    }
}
