using UnityEngine;

namespace StatSystem.Example
{
public class Example : MonoBehaviour
{
    void Start()
    {
        Debug.Log(System.Runtime.InteropServices.Marshal.SizeOf(typeof(Modifier)));
    }
}
}
