using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketHits : MonoBehaviour
{

    public GameObject explosion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            explosion.transform.position = gameObject.transform.position;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            explosion.SetActive(true);
            StartCoroutine(later());
        }
    }

    IEnumerator later()
    {
        yield return new WaitForSeconds(0.25f);
        explosion.SetActive(false);
        gameObject.SetActive(false);
    }
}
