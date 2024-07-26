using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grassMove : MonoBehaviour
{
    [SerializeField] private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>(); //animation
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("grassAnim", true);
        anim.SetBool("grassAnim2", true);
        anim.SetBool("grassAnim3", true);
    }
}
