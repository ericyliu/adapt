using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
  public Camera playerCamera;
  public Text score;
  public Transform model;
  public Creature creature;

  public ClientControlState clientControlState;

  public void UpdateFromServer(ServerCharacter character)
  {
    Vector3 newPosition = new Vector3(
      character.position.x,
      this.transform.position.y,
      character.position.y
    );
    Vector3 direction = newPosition - this.transform.position;
    if (Vector3.Angle(direction, model.forward) > 20f) model.LookAt(model.position + direction, Vector3.up);
    this.transform.position = newPosition;
  }

  // Update is called once per frame
  void Update()
  {
    HandleMovement();
    HandleUI();
  }

  void HandleMovement()
  {
    clientControlState = new ClientControlState();
    if (Input.GetKey("w")) clientControlState.moveUp = true;
    if (Input.GetKey("a")) clientControlState.moveLeft = true;
    if (Input.GetKey("s")) clientControlState.moveDown = true;
    if (Input.GetKey("d")) clientControlState.moveRight = true;
    // if (!direction.Equals(Vector3.zero)) model.LookAt(model.position + direction, Vector3.up);
  }

  void HandleUI()
  {
    score.text = "Health: " + creature.health + "/" + creature.maxHealth + "\nAgility: " + creature.agility + "\nEssence: " + creature.essence + "\nFury: " + creature.fury;
  }
}
