using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsMove : MonoBehaviour
{
    public AnimationCurve WallTransform = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 0.5f), new Keyframe(1, 1));
    public Transform newposition;
    private bool CanMove = true;

    public IEnumerator MoveWall()
    {
        if (CanMove == true)
        {
            CanMove = false;
            Vector3 from = transform.position;
            Vector3 to = newposition.transform.position;
            float elapsed = 0f;

            while (elapsed < WallTransform.keys[WallTransform.keys.Length - 1].time)
            {
                transform.position = Vector3.Lerp(from, to, elapsed);

                elapsed += Time.deltaTime;

                yield return null;
            }

            newposition.transform.position = from;
        }
        CanMove = true;
    }
}
