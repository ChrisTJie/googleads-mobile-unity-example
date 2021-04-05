using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CTJ
{
    public class Coroutine : MonoBehaviour
    {
        internal static WaitForEndOfFrame EndOfFrame;
        internal static WaitForSeconds GetWaitForSeconds(float _seconds) { return new WaitForSeconds(_seconds); }
    }
}