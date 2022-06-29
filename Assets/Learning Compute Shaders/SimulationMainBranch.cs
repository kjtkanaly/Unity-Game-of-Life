using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Cell
{
    public Vector2 Position;
    public float CurrentStatus;
    public float PreviousStatus;
    public float FilterValue;
}

public class SimulationMainBranch : MonoBehaviour
{
    private Cell[] cellMap;
    private Cell[] tempMap;

    //public ComputeShader lifeShader;
    public ComputeShader lifeGeneration;
    //public ComputeShader updatePreviousGeneration;
    public ComputeShader lifeMapFilter;

    public ComputeBuffer cellsBuffer;
    //public ComputeBuffer recordPopulationBuffer;

    public RenderTexture renderTexture;

    public Vector2Int resolution; // (x,y)

    public float lifeTrailFadeRate = 0.5f;
    public float frequency = 1;
    public float smoothness = 10f;
    public float spawnLifeCheckBar = 1.3f;

    public uint countLimit = 50;
    private uint count = 0;

    public bool applyFilter = false;

    public void Start()
    {
        // Checks if the renderTexture has been made/active
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(resolution.x, resolution.y, 0);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }

        cellMap = new Cell[resolution.x * resolution.y];
        tempMap = new Cell[resolution.x * resolution.y];

        cellsBuffer = new ComputeBuffer(cellMap.Length, sizeof(float) + 2 * sizeof(float) + sizeof(float) + sizeof(float));
        //recordPopulationBuffer = new ComputeBuffer(cellMap.Length, sizeof(float) + 2 * sizeof(float) + sizeof(float) + sizeof(float));


        lifeGeneration.SetBuffer(0, "cellMap", cellsBuffer);
        //lifeGeneration.SetTexture(0, "renderTexture", renderTexture);
        lifeGeneration.SetInt("width", resolution.x);
        lifeGeneration.SetInt("height", resolution.y);
        lifeGeneration.SetFloat("lifeTrailFadeRate", lifeTrailFadeRate);

        /*
        updatePreviousGeneration.SetBuffer(0, "cellMap", recordPopulationBuffer);
        updatePreviousGeneration.SetInt("width", resolution.x);
        updatePreviousGeneration.SetInt("height", resolution.y);
        */

        lifeMapFilter.SetBuffer(0, "cellMap", cellsBuffer);
        lifeMapFilter.SetTexture(0, "renderTexture", renderTexture);
        lifeMapFilter.SetFloat("lifeTrailFadeRate", lifeTrailFadeRate);
        lifeMapFilter.SetInt("width", resolution.x);
        lifeMapFilter.SetInt("height", resolution.y);


        // Begin Life Shader
        //lifeShader.SetTexture(0, "renderTexture", renderTexture);
        //lifeShader.SetInt("width", resolution.x);
        //lifeShader.SetInt("height", resolution.y);

        //beginLife();
        beginLifeBranch();

        // Draw Life
        imageFilter();
    }

    // Renders/Applys changes to the renderTexture
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(renderTexture, destination);
    }

    // Fixed update fx. That is called every 1/50 seconds
    public void FixedUpdate()
    {

        if (renderTexture != null)
        {
            if (count >= countLimit)
            {
                stepGeneration();
                
                imageFilter();
                count = 0;
            }

            count += 1;
        }
    }

    void beginLifeBranch()
    {
        // Setting the perlin noise offset randomly
        int xOffset = Mathf.RoundToInt(Random.Range(0f, 99999f));
        int yOffset = Mathf.RoundToInt(Random.Range(0f, 99999f));

        // Set the Pixels of the Actual Array
        for (int i = 0; i < resolution.x * resolution.y; i++)
        {
            int x = i % resolution.x;
            int y = i / resolution.x;

            float spawnLifeCheck = frequency * Mathf.PerlinNoise((x + xOffset) / smoothness, (y + yOffset) / smoothness) / (frequency / 2);

            if (spawnLifeCheck >= spawnLifeCheckBar)
            {
                cellMap[i].CurrentStatus = 1f;
            }
        }

        /*
        cellsBuffer.SetData(cellMap);

        lifeShader.SetBuffer(0, "cellMap", cellsBuffer);
        lifeShader.Dispatch(0, resolution.x / 16 + 1, resolution.y / 16, 1);

        cellsBuffer.GetData(cellMap);

        //cellsBuffer.Dispose();
        */
    }

    void stepGeneration()
    {

        /*
        tempMap = cellMap;
        recordPopulationBuffer.SetData(tempMap);

        updatePreviousGeneration.SetBuffer(0, "cellMap", recordPopulationBuffer);
        //updatePreviousGeneration.SetInt("width", resolution.x);
        //updatePreviousGeneration.SetInt("height", resolution.y);
        updatePreviousGeneration.Dispatch(0, (resolution.x * resolution.y) / 16 + 1, 1, 1);

        recordPopulationBuffer.GetData(cellMap);

        //recordPopulationBuffer.Dispose();
        */

        // Record the new generation map in the "CurrentValue" slot and Update the texture
        tempMap = cellMap;
        cellsBuffer.SetData(tempMap);

        lifeGeneration.SetBuffer(0, "cellMap", cellsBuffer);
        //lifeGeneration.SetTexture(0, "renderTexture", renderTexture);
        //lifeGeneration.SetInt("width", resolution.x);
        //lifeGeneration.SetInt("height", resolution.y);
        //lifeGeneration.SetFloat("lifeTrailFadeRate", lifeTrailFadeRate);
        lifeGeneration.Dispatch(0, (resolution.x * resolution.y) / 32 + 1, 1, 1);

        cellsBuffer.GetData(cellMap);

        //cellsBuffer.Dispose();

    }

    void imageFilter()
    {

        tempMap = cellMap;
        cellsBuffer.SetData(tempMap);

        lifeMapFilter.SetBuffer(0, "cellMap", cellsBuffer);
        lifeMapFilter.SetBool("applyFilter",applyFilter);
        lifeMapFilter.Dispatch(0, resolution.x / 32 + 1, resolution.y / 32 +1, 1);

        cellsBuffer.GetData(cellMap);

        

    }

    public void OnApplicationQuit()
    {
        cellsBuffer.Dispose();
        //recordPopulationBuffer.Dispose();
    }

}
