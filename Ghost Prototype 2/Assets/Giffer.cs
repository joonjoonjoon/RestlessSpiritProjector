using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Giffer : MonoBehaviour {

    private SpriteRenderer sprite;
    public float fps = 3;
    [ReadOnly]
    public Sprite[] sprites;
    private float timer;
    private int frame;

	void Start ()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void LoadFromString(string spritesheet)
    {
        sprites = Resources.LoadAll<Sprite>(spritesheet);
    }

	void Update () {
		if(timer<= 0 && sprites!= null && sprites.Length>0)
        {
            timer = 1 / fps;
            frame++;
            if (frame >= sprites.Length)
                frame = 0;

            sprite.sprite = sprites[frame];
        }
        timer -= Time.deltaTime;
	}
}
