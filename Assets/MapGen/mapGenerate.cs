using UnityEngine;

public class mapGenerate : MonoBehaviour
{
    //mesh
    Mesh mesh;
    MeshRenderer meshRenderer;
    Vector3[] vertices;
    int[] triangles;
    public int width = 20;
    public int depth = 20;
    //heightmap
    public int heightmapOctaves = 5;
    public float heightmapScale = 0.8f;
    public float heightmapAmplitude = 1;
    public float heightmapFrequency = 1;
    public float heightmapAmplitudeMultiplier = 4.5f;
    public float heightmapFrequencyMultiplier = 0.3f;
    public float heightmapOffset;
    private float maxHeight = 0;
    private float minHeight = 0;
    //colours
    private Color baseColour;
    public float baseRed;
    public float baseGreen;
    //perlin colours
    public float baseBlue;
    public float perlinRed;
    public float perlinGreen;
    public float perlinBlue;
    public float perlinRedMultiplier;
    public float perlinGreenMultiplier;
    public float perlinBlueMultiplier;
    //height colours
    private Color heightColour;
    public float colourHeightFraction = 0.9f;
    private float minColourHeight;
    private float colourHeightRange;
    //colourmap generation
    private Color[] colourMap;
    public int colourmapOctaves = 5;
    public float colourmapScale = 0.8f;
    public float colourmapAmplitude = 0.4f;
    public float colourmapFrequency = 0.05f;
    public float colourmapAmplitudeMultiplier = 4.5f;
    public float colourmapFrequencyMultiplier = 0.3f;
    public float colourmapOffset;
    //texture
    private Texture2D texture;
    private Vector2[] uvMap;
    //material
    private Material mat;
    public float metallic;
    public float smoothness;

    private bool update = false;

    void Start()
    {
        //mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = GetComponent<MeshRenderer>();

        //colours
        Randomise();

        //material
        mat = meshRenderer.material;

        MeshData();
        UpdateMesh();
    }

    void Update()
    {
        if(Input.GetKey("space"))
        {
            Randomise();
            MeshData();
            UpdateMesh();
        }

        if(Input.GetKey("a"))
        {
            update = !update;
        }

        if(Input.GetKey("d"))
        {
            MeshData();
            UpdateMesh();
        }

        if(update)
        {
            MeshData();
            UpdateMesh();
        }
    }

    void Randomise()
    {
        //heightmap
        heightmapScale = RandomValue(0.45f, 1.2f);
        heightmapFrequency = RandomValue(0.8f, 1.5f);
        heightmapAmplitudeMultiplier = RandomValue(4, 5);
        heightmapFrequencyMultiplier = RandomValue(0.26f, 0.33f);
        heightmapOffset = RandomValue(-10000, 10000);

        //colourmap
        colourmapAmplitude = RandomValue(0.45f, 0.8f);
        colourmapFrequency = RandomValue(0.01f, 0.04f);
        colourmapOffset = RandomValue(-10000, 10000);

        //colours
        baseRed = RandomValue(-1, 1);
        baseGreen = RandomValue(-1, 1);
        baseBlue = RandomValue(-1, 1);
        perlinRedMultiplier = RandomValue(0, 3);
        perlinGreenMultiplier = RandomValue(0, 3);
        perlinBlueMultiplier = RandomValue(0, 3);

        //material properties
        metallic = RandomValue(0, 0.6f);
        smoothness = RandomValue(0, 0.6f);
    }

    void MeshData()
    {
        //vertices
        vertices = new Vector3[(width + 1) * (depth + 1)];
        maxHeight = 0;
        minHeight = 0;

        //texture
        uvMap = new Vector2[vertices.Length];
        colourMap = new Color[vertices.Length];
        baseColour = new Color(baseRed, baseGreen, baseBlue);
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Glossiness", smoothness);

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
        float amplitude = heightmapAmplitude;
        float frequency = heightmapFrequency;
        float perlin = 0;

        //loop through octaves, adding in less significant and more consistent detail with each
        for (int octave = 0; octave < heightmapOctaves; octave++)
        {
            //perlin noise
            perlin = Perlin(x, z, frequency, amplitude, heightmapScale, heightmapOffset);
            noiseValue += new Vector3(x, perlin, z);

            frequency *= heightmapFrequencyMultiplier;
            amplitude *= heightmapAmplitudeMultiplier;
        }

        vertices[i] = noiseValue;

        if (perlin > maxHeight)
        {
            maxHeight = perlin;
        }
        if (perlin < minHeight)
        {
            minHeight = perlin;
        }
    }

    float Perlin(int x, int z, float frequency, float amplitude, float scale, float offset)
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

        PerlinColourMap(x, z);
        //HeightColourMap(x, z, i);
    }

    void PerlinColourMap(int x, int z)
    {
        Color[] colourValue = new Color[vertices.Length];
        for (int octave = 0; octave < colourmapOctaves; octave++)
        {
            //perlin noise
            perlinRed = Perlin(x, z, colourmapFrequency, colourmapAmplitude, colourmapScale, 0) * perlinRedMultiplier;
            perlinGreen = Perlin(x, z, colourmapFrequency, colourmapAmplitude, colourmapScale, 0) * perlinGreenMultiplier;
            perlinBlue = Perlin(x, z, colourmapFrequency, colourmapAmplitude, colourmapScale, 0) * perlinBlueMultiplier;

            colourValue[z * width + x] += baseColour + new Color(perlinRed, perlinGreen, perlinBlue);

            colourmapFrequency *= colourmapFrequencyMultiplier;
            colourmapAmplitude *= colourmapAmplitudeMultiplier;
        }

        colourMap[z * width + x] = colourValue[z * width + x];
        /*
        perlinRed = Perlin(x, z, colourmapFrequency, colourmapAmplitude, colourmapScale, 0) * perlinRedMultiplier;
        perlinGreen = Perlin(x, z, colourmapFrequency, colourmapAmplitude, colourmapScale, 0) * perlinGreenMultiplier;
        perlinBlue = Perlin(x, z, colourmapFrequency, colourmapAmplitude, colourmapScale, 0) * perlinBlueMultiplier;
        colourMap[z * width + x] = baseColour + new Color(perlinRed, perlinGreen, perlinBlue);
        */
    }

    void HeightColourMap(int x, int z, int i)
    {
        /*
        minColourHeight = minHeight + colourHeightFraction * (maxHeight - minHeight);
        colourHeightRange = heightMap[x, z] - minColourHeight * 0.01f;
        heightColour = new Color(1, 0, 0) * colourHeightRange;

        if (heightMap[x, z] > minColourHeight)
        {
            colourMap[z * width + x] = heightColour;
        }
        */
    }

    float RandomValue(float min = 0, float max = 1)
    {
        return Random.Range(min, max);
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
        texture.wrapMode = TextureWrapMode.Clamp;
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

        mesh.RecalculateNormals();
    }
}
