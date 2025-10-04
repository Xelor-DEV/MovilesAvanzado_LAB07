using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite[] animationFrames;
    [Header("Config")]
    [SerializeField] private float frameRate = 0.1f;
    [SerializeField] private bool playOnStart = true;

    private bool isPlaying = false;
    private Coroutine animationCoroutine;

    private void Start()
    {
        if (playOnStart)
        {
            PlayAnimation();
        }
    }

    public void PlayAnimation()
    {
        if (isPlaying) return;

        isPlaying = true;
        animationCoroutine = StartCoroutine(AnimateSprites());
    }

    public void StopAnimation()
    {
        isPlaying = false;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
    }

    public void StopAndResetAnimation()
    {
        StopAnimation();
        if (targetImage != null && animationFrames.Length > 0)
        {
            targetImage.sprite = animationFrames[0];
        }
    }

    private IEnumerator AnimateSprites()
    {
        int currentFrame = 0;

        while (isPlaying)
        {
            targetImage.sprite = animationFrames[currentFrame];

            yield return new WaitForSeconds(frameRate);

            currentFrame = (currentFrame + 1) % animationFrames.Length;
        }
    }
}