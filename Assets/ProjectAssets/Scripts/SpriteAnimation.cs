using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;

public class SpriteAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite[] animationFrames;

    [Header("Config")]
    [SerializeField] private float frameRate = 0.1f;
    [SerializeField] private float minAnimationDuration = 0.5f; // Tiempo mínimo que debe reproducirse
    [SerializeField] private bool playOnStart = true;

    private bool isPlaying = false;
    private Coroutine animationCoroutine;
    private float lastPressTime = 0f;
    private bool animationRequested = false;

    private void Start()
    {
        if (playOnStart)
        {
            PlayAnimation();
        }
    }

    public void PlayAnimation()
    {
        if (isPlaying)
        {
            lastPressTime = Time.time;
            animationRequested = true;
            return;
        }

        StartAnimation();
    }

    private void StartAnimation()
    {
        if (isPlaying) return;

        isPlaying = true;
        animationRequested = true;
        lastPressTime = Time.time;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateWithCooldown());
    }

    private IEnumerator AnimateWithCooldown()
    {
        int currentFrame = 0;
        float animationStartTime = Time.time;

        while (isPlaying && (Time.time - animationStartTime) < minAnimationDuration)
        {
            if (targetImage != null && animationFrames.Length > 0)
            {
                targetImage.sprite = animationFrames[currentFrame];
            }

            yield return new WaitForSeconds(frameRate);

            currentFrame = (currentFrame + 1) % animationFrames.Length;
        }

        while (isPlaying && animationRequested)
        {
            if (targetImage != null && animationFrames.Length > 0)
            {
                targetImage.sprite = animationFrames[currentFrame];
            }

            yield return new WaitForSeconds(frameRate);

            currentFrame = (currentFrame + 1) % animationFrames.Length;

            if (Time.time - lastPressTime > frameRate * 2)
            {
                animationRequested = false;
            }
        }

        StopAnimation();
    }

    public void StopAnimation()
    {
        isPlaying = false;
        animationRequested = false;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        if (targetImage != null && animationFrames.Length > 0)
        {
            targetImage.sprite = animationFrames[0];
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

    public void ForceStopAnimation()
    {
        isPlaying = false;
        animationRequested = false;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        if (targetImage != null && animationFrames.Length > 0)
        {
            targetImage.sprite = animationFrames[0];
        }
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }
}