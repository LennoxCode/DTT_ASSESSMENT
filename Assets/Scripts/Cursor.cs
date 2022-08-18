using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class Cursor : MonoBehaviour
    {
        [SerializeField] private float fadeSpeed;
        [SerializeField] private SpriteRenderer renderer;
        private float alpha;

        private void Start()
        {
            alpha = 1;
            Color test = Color.magenta;
            renderer.color = test;
            StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            while (alpha >= 0)
            {
                alpha -= fadeSpeed * Time.deltaTime;
                renderer.color = new Color(255, 0, 255, alpha);
                yield return new WaitForEndOfFrame(); 
            }
            Destroy(gameObject);
        }
    }
}