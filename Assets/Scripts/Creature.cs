using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
  public Transform model;
  public float speed = 3f;
  public float baseSpeed = 3f;
  public float fury = 0f;
  public float agility = 0f;
  public float essence = 0f;
  public float health = 100f;
  public float maxHealth = 100f;
  public float baseHealth = 100f;
  // Start is called before the first frame update
  void Start()
  {
    health = maxHealth;
  }

  // Update is called once per frame
  void Update()
  {
    HandleStats();
  }

  void HandleStats()
  {
    speed = baseSpeed + (agility / 10f);
    maxHealth = baseHealth + fury;
    float size = 1f + (.01f * fury);
    this.model.localScale = new Vector3(size, size, size);
  }

  void OnTriggerEnter(Collider other)
  {
    Amoeba amoeba = other.gameObject.GetComponent<Amoeba>();
    if (amoeba == null) return;
    if (amoeba.type == AmoebaType.Agility) agility++;
    else if (amoeba.type == AmoebaType.Essence) essence++;
    else if (amoeba.type == AmoebaType.Fury)
    {
      health = Mathf.Min(health + 1, maxHealth + 1);
      fury++;
    }
    GameObject.Destroy(other.gameObject);
  }
}
