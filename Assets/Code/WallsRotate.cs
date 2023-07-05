using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsRotate : MonoBehaviour
{
    public AnimationCurve WallRotation = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 0.5f), new Keyframe(1, 1));
   
    public IEnumerator RotateWall(Vector3 axis, float angle)
    {
        Quaternion from = transform.rotation;
        Quaternion to = transform.rotation;
        to *= Quaternion.Euler(axis * angle);
        float elapsed = 0f;

        while (elapsed < WallRotation.keys[WallRotation.keys.Length - 1].time)
        {
            transform.rotation = Quaternion.Lerp(from, to, WallRotation.Evaluate(elapsed));

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.rotation = to;
    }
}
