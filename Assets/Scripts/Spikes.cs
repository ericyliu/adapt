using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
  public int damage = 3;
  // Start is called before the first frame update
  void Start()
  {
    Transform model = this.transform.Find("Model");
    GameObject prefab = (GameObject.Find("Prefabs").GetComponent<Prefabs>() as Prefabs).spikes;
    Instantiate(prefab, model);
  }

  void OnTriggerEnter(Collider other)
  {
    Creature enemy = other.gameObject.GetComponent<Creature>();
    if (enemy == null) return;
    enemy.TakeDamage(this.damage);
  }
}
