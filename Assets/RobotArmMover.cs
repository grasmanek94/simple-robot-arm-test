using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArmMover : MonoBehaviour {

    public GameObject base_rotation;
    public GameObject arm_1;
    public GameObject arm_2;
    public GameObject end;

    public GameObject move_to;

    bool CheckNull(GameObject check)
    {
        return 
            check == null || 
            check.transform == null || 
            check.transform.position == null ||
            check.transform.rotation == null;
    }

	void Update ()
    {
        if( CheckNull(base_rotation) ||
            CheckNull(arm_1) ||
            CheckNull(arm_2) ||
            CheckNull(end) ||
            CheckNull(move_to))
        {
            return;
        }

        MoveTo(move_to);
	}

    public void MoveTo(GameObject game_object)
    {
        MoveTo(game_object.transform.position);
    }

    public void MoveTo(Vector3 position)
    {
        // see documentation for explanation of formulas and used variable names
        // rotate base
        float r1 = 0.0f;

        if (position.z != 0.0f || position.x != 0.0f)
        {
            r1 = -Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;
        }

        base_rotation.transform.localRotation = Quaternion.Euler(0.0f, r1, 0.0f);

        // calculate distances
        float len1 = (arm_2.transform.position - arm_1.transform.position).magnitude;
        float len2 = (end.transform.position - arm_2.transform.position).magnitude;
        float dist = (position - base_rotation.transform.position).magnitude;

        // rotate arm 1
        float d1 = 0.0f;
        if (position.y != 0.0f || position.x != 0.0f)
        {
            // the sqrt(pow + pow) is done to create a "2d plane" view projection
            d1 = Mathf.Atan2(position.y, Mathf.Sqrt(Mathf.Pow(position.x, 2.0f) + Mathf.Pow(position.z, 2.0f))) 
                * Mathf.Rad2Deg;
        }

        float d2 = 0.0f;

        if (len1 != 0.0f && dist != 0.0f)
        {
            d2 = Mathf.Acos(
                (Mathf.Pow(len1, 2.0f) + Mathf.Pow(dist, 2.0f) - Mathf.Pow(len2, 2.0f)) 
                / 
                (2.0f * len1 * dist)
            ) * Mathf.Rad2Deg;
        }

        float a1 = d1 + d2;

        if (!float.IsNaN(a1))
        {
            // need to shift +1/2 pi in game engine
            arm_1.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -Mathf.PI/2.0f*Mathf.Rad2Deg + a1); 
        }
        
        // rotate arm 2
        float a2 = 0.0f;

        if (len1 != 0.0f && len2 != 0.0f)
        {
            a2 = -Mathf.Acos(
                (Mathf.Pow(len1, 2.0f) + Mathf.Pow(len2, 2.0f) - Mathf.Pow(dist, 2.0f))
                / 
                (2.0f * len1 * len2)
            ) * Mathf.Rad2Deg;
        }

        if (!float.IsNaN(a2))
        {
            arm_2.transform.localRotation = Quaternion.Euler(180.0f, 0.0f, a2);
        }
    }
}
