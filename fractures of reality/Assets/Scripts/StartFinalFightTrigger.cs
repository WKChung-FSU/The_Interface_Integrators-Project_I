using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFinalFightTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]DragonScript dragon;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dragon.StartFinalBattle();
            Destroy(this.gameObject);
        }
    }
}
