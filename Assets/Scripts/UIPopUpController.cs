using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIPopUpController : MonoBehaviour
{
    public float speed;
    public float timeToDestroy = 2;
    public enum direction { Up, Down, Left, Right };
    public direction translationDirection;

    private void Start()
    {
        GetComponent<Text>().DOFade(0, timeToDestroy).SetEase(Ease.InExpo).OnComplete(()=>Destroy(gameObject));
    }

    private void Update()
    {
        switch (translationDirection)
        {
            case direction.Up:
                transform.Translate(Vector3.up * speed * Time.deltaTime);
                break;
            case direction.Down:
                transform.Translate(Vector3.down * speed * Time.deltaTime);
                break;
            case direction.Left:
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                break;
            case direction.Right:
                transform.Translate(Vector3.right * speed * Time.deltaTime);
                break;
        }
    }
}
