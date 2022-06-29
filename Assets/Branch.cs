using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Branch : MonoBehaviour
{
    public int imageWidth = 240;
    public int imageHeight = 135;
    int xOffset, yOffset;

    public float frequency = 1;
    public float smoothness = 10f;
    public float spawnLifeCheckBar = 1.3f;
    public float timeBetweenGenerations = 0.1f;
    public float lifeTrailFadeRate = 0.01f;

    public Color[] backgroundColor;

    //public float[,].r currentLifeMap;    // This array if fully intialized later for C# rules
    //public float[,].r nextLifeMap;

    public Image canvas;

    Texture2D previousTexture, newTexture;



    // Start is called before the first frame update
    void Start()
    {
        // The background color array
        backgroundColor = new Color[imageWidth * imageHeight];

        canvas.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);

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
        // Crate the texture
        previousTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false);
        newTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false);

        // Set the Pixels of the Actual Array
        newTexture.SetPixels(backgroundColor);

        // Apply the pixel map
        newTexture.Apply();

        // Apply the background texture to the image
        canvas.sprite = Sprite.Create(newTexture, new Rect(0, 0, imageWidth, imageHeight), new Vector2(0.5f, 0.5f));
    }

    void beginLife()
    {
        // Setting the perlin noise offset randomly
        xOffset = Mathf.RoundToInt(Random.Range(0f, 99999f));
        yOffset = Mathf.RoundToInt(Random.Range(0f, 99999f));

        // Crate the texture
        newTexture = canvas.sprite.texture;

        // Set the Pixels of the Actual Array
        for (int i = 1; i < imageWidth - 1; i++)
        {
            for (int j = 1; j < imageHeight - 1; j++)
            {
                float spawnLifeCheck = frequency * Mathf.PerlinNoise((i + xOffset) / smoothness, (j + yOffset) / smoothness) / (frequency / 2);

                if (spawnLifeCheck >= spawnLifeCheckBar)
                {
                    newTexture.SetPixel(i, j, new Color(1f, 1f, 1f));  // Displaying the life
                }
            }
        }

        // Apply the pixel map
        newTexture.Apply();

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
        Color[] colorData = new Color[imageWidth * imageHeight];
        //Debug.Log(canvas.sprite.texture.GetPixelData<Color32>(colorData));

        //// Record the previous texture
        //previousTexture = canvas.sprite.texture;

        //// Initialize the new texture
        //newTexture.SetPixels(backgroundColor);

        //// Initialize the sum variable;
        //float sum;

        //// Set the Pixels of the Actual Array
        //for (int i = 1; i < imageWidth - 1; i++)
        //{
        //    for (int j = 1; j < imageHeight - 1; j++)
        //    {
        //        //newTexture.SetPixel(i, j, new Color(0f, 0f, 0f));

        //        // Checking if the current pixel is void of life but fading
        //        if ((previousTexture.GetPixel(i, j).r > 0f) && (previousTexture.GetPixel(i, j).r < 1f))
        //        {
        //            // Applying the cool fading effect
        //            newTexture.SetPixel(i, j, new Color(previousTexture.GetPixel(i, j).r - lifeTrailFadeRate, previousTexture.GetPixel(i, j).g - lifeTrailFadeRate, previousTexture.GetPixel(i, j).b - lifeTrailFadeRate));
        //        }

        //        int lifeCheck = 0;

        //        // Checking if the current pixel has life and if it can continue
        //        if (previousTexture.GetPixel(i, j).r == 1f)
        //        {
        //            for (int bufferX = -1; bufferX <= 1; bufferX++)
        //            {
        //                for (int bufferY = -1; bufferY <= 1; bufferY++)
        //                {
        //                    if (previousTexture.GetPixel(i + bufferX, j + bufferY).r == 1f)
        //                    {
        //                        lifeCheck += 1;
        //                    }
        //                }
        //            }



        //            // Solitude Check (If a "living" cell is only surronded by <= 1 "living" neighbor it dies)
        //            if (lifeCheck <= 2)
        //            {
        //                newTexture.SetPixel(i, j, new Color(previousTexture.GetPixel(i, j).r - lifeTrailFadeRate, previousTexture.GetPixel(i, j).g - lifeTrailFadeRate, previousTexture.GetPixel(i, j).b - lifeTrailFadeRate));
        //            }

        //            // Overpopulation Check (If a "living" cell is surroned by >= 4 "living" neighbors it dies)
        //            else if (lifeCheck >= 5)
        //            {
        //                newTexture.SetPixel(i, j, new Color(previousTexture.GetPixel(i, j).r - lifeTrailFadeRate, previousTexture.GetPixel(i, j).g - lifeTrailFadeRate, previousTexture.GetPixel(i, j).b - lifeTrailFadeRate));
        //            }

        //            // Ideal Conditions Check (If a "living" cell is surronded by 2 or 3 "living" neighbors it lives
        //            else
        //            {
        //                newTexture.SetPixel(i, j, new Color(1f, 1f, 1f));
        //            }
        //        }

        //        else
        //        {
        //            for (int bufferX = -1; bufferX <= 1; bufferX++)
        //            {
        //                for (int bufferY = -1; bufferY <= 1; bufferY++)
        //                {
        //                    if (previousTexture.GetPixel(i + bufferX, j + bufferY).r == 1)
        //                    {
        //                        lifeCheck += 1;
        //                    }
        //                }
        //            }

        //            // Ideal Living Check (If a "dead" cell is surronded by 3 "living" neighbors it lives
        //            if (lifeCheck == 3)
        //            {
        //                newTexture.SetPixel(i, j, new Color(1f, 1f, 1f));
        //            }
        //        }
        //    }
        //}

        ////for (int i = 1; i < imageWidth - 1; i++)
        ////{
        ////    for (int j = 1; j < imageHeight - 1; j++)
        ////    {

        ////        sum = 0f;

        ////        for (int offsetX = -1; offsetX <= 1; offsetX++)
        ////        {
        ////            for (int offsetY = -1; offsetY <= 1; offsetY++)
        ////            {
        ////                sum += texture.GetPixel(i + offsetX, j + offsetY).r;
        ////            }
        ////        }

        ////        sum = sum / 9;

        ////        texture.SetPixel(i, j, new Color(sum, sum, sum));

        ////    }
        ////}

        //// Apply the pixel map
        //newTexture.Apply();
    }
}
