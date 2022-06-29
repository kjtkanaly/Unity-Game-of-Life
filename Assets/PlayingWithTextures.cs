using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayingWithTextures : MonoBehaviour
{
    public int imageWidth = 240;
    public int imageHeight = 135;
    int xOffset, yOffset;

    public float frequency = 1;
    public float smoothness = 10f;
    public float spawnLifeCheckBar = 1.3f;
    public float timeBetweenGenerations = 0.1f;
    public float lifeTrailFadeRate = 0.01f;

    public float[,] currentLifeMap;    // This array if fully intialized later for C# rules
    public float[,] nextLifeMap;

    public Image canvas;

    Texture2D texture;

    

    // Start is called before the first frame update
    void Start()
    {
        canvas.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);

        currentLifeMap = new float[imageWidth, imageHeight];
        nextLifeMap = new float[imageWidth, imageHeight];

        initializeBackground();

        beginLife();
    }

    public void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            resetLife();
        }
    }

    void initializeBackground()
    {
        // The background color array
        Color[] backgroundColor = new Color[imageWidth * imageHeight];

        // Crate the texture
        texture = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32,false);

        // Set the Pixels of the Actual Array
        texture.SetPixels(backgroundColor);

        // Apply the pixel map
        texture.Apply();

        // Apply the background texture to the image
        canvas.sprite = Sprite.Create(texture, new Rect(0, 0, imageWidth, imageHeight), new Vector2(0.5f, 0.5f));
    }

    void beginLife()
    {
        // Setting the perlin noise offset randomly
        xOffset = Mathf.RoundToInt(Random.Range(0f, 99999f));
        yOffset = Mathf.RoundToInt(Random.Range(0f, 99999f));

        // Crate the texture
        texture = canvas.sprite.texture;

        // Set the Pixels of the Actual Array
        for (int i = 1; i < imageWidth - 1; i++)
        {
            for(int j = 1; j < imageHeight - 1; j++)
            {
                float spawnLifeCheck = frequency * Mathf.PerlinNoise((i + xOffset) / smoothness, (j + yOffset) / smoothness) / (frequency / 2);

                if (spawnLifeCheck >= spawnLifeCheckBar)
                {
                    currentLifeMap[i, j] = 1f;  // Recording the life

                    texture.SetPixel(i, j, new Color(1f, 1f, 1f));  // Displaying the life
                }
            }
        }

        // Apply the pixel map
        texture.Apply();

        InvokeRepeating("nextGeneration", timeBetweenGenerations, timeBetweenGenerations);
    }

    public void resetLife()
    {
        CancelInvoke();

        initializeBackground();

        beginLife();
    }

    public void nextGeneration()
    {
        // Crate the texture
        texture = canvas.sprite.texture;

        // Initialize the next life record
        nextLifeMap = (new float[imageWidth, imageHeight]);

        // Initialize the sum variable;
        float sum;

        // Set the Pixels of the Actual Array
        for (int i = 1; i < imageWidth - 1; i++)
        {
            for (int j = 1; j < imageHeight -1; j++)
            {
                // Checking if the current pixel is void of life but fading
                if ((texture.GetPixel(i, j).r > 0f) && (texture.GetPixel(i, j).r < 1f))
                {
                    // Recording that this spot is technically void of life 
                    //nextLifeMap[i, j] = 0f;

                    // Applying the cool fading effect
                    texture.SetPixel(i, j, new Color(texture.GetPixel(i, j).r - lifeTrailFadeRate, texture.GetPixel(i, j).g - lifeTrailFadeRate, texture.GetPixel(i, j).b - lifeTrailFadeRate));
                }

                // Checking if the current pixel has life and if it can continue
                if (currentLifeMap[i, j] == 1f)
                {
                    // Solitude Check (If a "living" cell is only surronded by <= 1 "living" neighbor it dies)
                    if ((currentLifeMap[i - 1, j + 1] + currentLifeMap[i, j + 1] + currentLifeMap[i + 1, j + 1] +
                         currentLifeMap[i - 1, j] + currentLifeMap[i, j] + currentLifeMap[i + 1, j] +
                         currentLifeMap[i - 1, j - 1] + currentLifeMap[i, j - 1] + currentLifeMap[i + 1, j - 1])
                         <= 2)
                    {
                        nextLifeMap[i, j] = 0f;

                        // Applying the cool fading effect
                        texture.SetPixel(i, j, new Color(texture.GetPixel(i, j).r - lifeTrailFadeRate, texture.GetPixel(i, j).g - lifeTrailFadeRate, texture.GetPixel(i, j).b - lifeTrailFadeRate));
                    }

                    // Overpopulation Check (If a "living" cell is surroned by >= 4 "living" neighbors it dies)
                    else if ((currentLifeMap[i - 1, j + 1] + currentLifeMap[i, j + 1] + currentLifeMap[i + 1, j + 1] +
                         currentLifeMap[i - 1, j] + currentLifeMap[i, j] + currentLifeMap[i + 1, j] +
                         currentLifeMap[i - 1, j - 1] + currentLifeMap[i, j - 1] + currentLifeMap[i + 1, j - 1])
                         >= 5)
                    {
                        nextLifeMap[i, j] = 0f;
                        
                        // Applying the cool fading effect
                        texture.SetPixel(i, j, new Color(texture.GetPixel(i, j).r - lifeTrailFadeRate, texture.GetPixel(i, j).g - lifeTrailFadeRate, texture.GetPixel(i, j).b - lifeTrailFadeRate));
                    }

                    // Ideal Conditions Check (If a "living" cell is surronded by 2 or 3 "living" neighbors it lives
                    else
                    {
                        nextLifeMap[i, j] = 1f;

                        // Displaying the life
                        texture.SetPixel(i, j, new Color(1f, 1f, 1f));
                    }
                }

                // Checking if the current pixel is void of life but eligable for new life
                else
                {
                    // Ideal Living Check (If a "dead" cell is surronded by 3 "living" neighbors it lives
                    if ((currentLifeMap[i - 1, j + 1] + currentLifeMap[i, j + 1] + currentLifeMap[i + 1, j + 1] +
                         currentLifeMap[i - 1, j] + currentLifeMap[i, j] + currentLifeMap[i + 1, j] +
                         currentLifeMap[i - 1, j - 1] + currentLifeMap[i, j - 1] + currentLifeMap[i + 1, j - 1])
                         == 3)
                    {
                        nextLifeMap[i, j] = 1f;

                        // Displaying the life
                        texture.SetPixel(i, j, new Color(1f, 1f, 1f));  
                    }
                }

                
            }
        }

        for (int i = 1; i < imageWidth - 1; i++)
        {
            for (int j = 1; j < imageHeight - 1; j++)
            {

                sum = 0f;

                for (int offsetX = -1; offsetX <= 1; offsetX++)
                {
                    for (int offsetY = -1; offsetY <= 1; offsetY++)
                    {
                        sum += texture.GetPixel(i + offsetX, j + offsetY).r;
                    }
                }

                sum = sum / 9;

                texture.SetPixel(i, j, new Color(sum, sum, sum));

            }
        }

        currentLifeMap = nextLifeMap;

        // Apply the pixel map
        texture.Apply();
    }
}
