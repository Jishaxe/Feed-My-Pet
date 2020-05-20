using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
     public static float Clamp0360(float eulerAngles)
     {
         float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
         if (result < 0)
         {
             result += 360f;
         }
         return result;
     }
}
