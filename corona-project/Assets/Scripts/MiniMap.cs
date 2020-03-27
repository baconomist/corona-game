using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    private RawImage _rawImage;
    private Texture2D _texture;
    private Vector3 _mapSize;
    private Car _car;

    void Start()
    {
        _rawImage = GetComponent<RawImage>();
        _texture = new Texture2D(100, 100);
        _mapSize = GameManager.Instance.ground.GetComponent<MeshRenderer>().bounds.size;
        _car = FindObjectOfType<Car>();
    }
    
    void Update()
    {
        // Clear the map
        for(int y = 0; y < 100; y++)
        {
            for (int x = 0; x < 100; x++)
            {
                _texture.SetPixel(x, y, Color.gray);
            }
        }

        // Draw plebs on the map
        foreach (Transform infectedPleb in GameManager.Instance.infectedPlebs)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 2; x++)
                {
                    Vector2 pos = WorldToMiniMap(infectedPleb.position);
                    _texture.SetPixel((int) pos.x + x, (int) pos.y + y, Color.red);
                }
            }
        }

        // Give the player a square 4x4 on the minimap
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                Vector2 pos = WorldToMiniMap(GameManager.Instance.player.transform.position);
                _texture.SetPixel((int) pos.x + x, (int) pos.y + y, Color.cyan);
            }
        }
        
        // Give the car a square 4x4 on the minimap
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                Vector2 pos = WorldToMiniMap(_car.transform.position);
                _texture.SetPixel((int) pos.x + x, (int) pos.y + y, Color.green);
            }
        }

        // Apply the texture to the image
        _texture.Apply();
        _rawImage.texture = _texture;
    }
    
    private Vector2 WorldToMiniMap(Vector3 pos)
    {
        float x = _texture.width - Map(pos.z + _mapSize.z / 2, 0, _mapSize.z, 0, _texture.height);
        float y = Map(pos.x + _mapSize.x / 2, 0, _mapSize.x, 0, _texture.width);
        return new Vector2(x, y);
    }

    private float Map(float val, float old_min, float old_max, float new_min, float new_max)
    {
        return ((val - old_min) / (old_max - old_min)) * (new_max - new_min) + new_min;
    }
}
