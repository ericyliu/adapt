using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
  public float speed = 3f;
  public float baseSpeed = 3f;
  public float fury = 0f;
  public float agility = 0f;
  public float essence = 0f;
  public float health = 100f;
  public float maxHealth = 100f;
  public float baseHealth = 100f;
  public Camera playerCamera;
  public Terrain terrain;
  public Transform model;
  public Text score;

  public ClientControlState clientControlState;

  // Use this for initialization
  void Start()
  {
    health = maxHealth;
  }

  // Update is called once per frame
  void Update()
  {
    HandleStats();
    HandleMovement();
    HandleUI();
  }

  void HandleStats()
  {
    speed = baseSpeed + (agility / 10f);
    maxHealth = baseHealth + fury;
    float size = 1f + (.01f * fury);
    model.localScale = new Vector3(size, size, size);
  }

  void HandleMovement()
  {
    Vector3 forward = Vector3.ProjectOnPlane(playerCamera.transform.up, terrain.transform.up).normalized * speed * Time.deltaTime;
    Vector3 direction = Vector3.zero;
    clientControlState = new ClientControlState();
    if (Input.GetKey("w"))
    {
      direction += forward;
      clientControlState.moveUp = true;
    }
    if (Input.GetKey("a"))
    {
      direction += new Vector3(-forward.z, forward.y, forward.x);
      clientControlState.moveLeft = true;
    }
    if (Input.GetKey("s"))
    {
      direction += new Vector3(-forward.x, forward.y, -forward.z);
      clientControlState.moveDown = true;
    }
    if (Input.GetKey("d"))
    {
      direction += new Vector3(forward.z, forward.y, -forward.x);
      clientControlState.moveRight = true;
    }
    transform.Translate(direction);
    if (!direction.Equals(Vector3.zero)) model.LookAt(model.position + direction, Vector3.up);
  }

  void HandleUI()
  {
    score.text = "Health: " + health + "/" + maxHealth + "\nAgility: " + agility + "\nEssence: " + essence + "\nFury: " + fury;
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
