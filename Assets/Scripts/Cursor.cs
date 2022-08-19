using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// this class represents a cursor to showcase the maze generation algorithms. each frame the alpha is reduced
    /// until it reaches zero and the object is destroys. this gives the effect of leaving behind a trail
    /// </summary>
    public class Cursor : MonoBehaviour
    {
        [SerializeField] private Color cursorColor;
        [SerializeField] private float fadeSpeed;
        [SerializeField] private SpriteRenderer renderer;
        private float alpha;

        private void Start()
        {
            alpha = 1;
            renderer.color = cursorColor;
            StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            while (alpha >= 0)
            {
                alpha -= fadeSpeed * Time.deltaTime;
                renderer.color = new Color(cursorColor.r, cursorColor.g, cursorColor.b, alpha);
                yield return new WaitForEndOfFrame(); 
            }
            Destroy(gameObject);
        }
    }
}