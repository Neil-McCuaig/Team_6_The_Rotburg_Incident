using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;

public class FadeToBlack : MonoBehaviour
{
    // Start is called before the first frame update

    //Need to find a command to set the referenced object of fadeToBlack at the start of the game's runtime,
    //otherwise it will be a pain of going back and forth checking that every trap has it added to it.
    //public Material fadeToBlack;

    //[Range(0f, 1f)] public float alpha = 0f;

    //public SpriteRenderer fadeRender;

    public bool FadeIn, FadeOut;

    public float fadeSpeed;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (FadeIn)
        {
            Color objectColor = this.GetComponent<Renderer>().material.color;
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            this.GetComponent<Renderer>().material.color = objectColor;

            if (objectColor.a >= fadeAmount)
            {
                FadeIn = false;
                FadeOut = true;
            }
        }

        if (FadeOut) 
        {
            Color objectColor = this.GetComponent<Renderer>().material.color;
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            this.GetComponent<Renderer>().material.color = objectColor;

            if (objectColor.a <= fadeAmount) 
            {
                FadeOut = false;
            }
        }
    }

    //IEnumerator FadeIn()
    //{
        //float elapsed = 0f;
        //Color originalColor = fadeRender.color;

        //while (elapsed < 0.75f)
        //{
            //float alpha = Mathf.Lerp(1f, 0f, elapsed / 1f);
            //fadeRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            //elapsed += Time.deltaTime;
            //yield return null;
        //}
        
        //fadeRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

    //}

    //IEnumerator FadeOut()
    //{
        //float elapsed = 0f;
        //Color originalColor = fadeRender.color;

        //while (elapsed < 0.75f)
        //{
            //float alpha = Mathf.Lerp(1f, 0f, elapsed / 1f);
            //fadeRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            //elapsed += Time.deltaTime;
            //yield return null;
        //}

        //fadeRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

    //}

    public void FadeInObject()
    {
        FadeIn = true;
    }

    public void FadeOutObject()
    {
        FadeOut = true;
    }
}
