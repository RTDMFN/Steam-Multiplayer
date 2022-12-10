using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 0.1f;
    public GameObject PlayerModel;

    private void Start(){
        PlayerModel.SetActive(false);
    }

    private void Update(){

        
        if(SceneManager.GetActiveScene().name == "TestGame"){
            if(PlayerModel.activeSelf == false && hasAuthority){
                //Everything in here occurs for all players
                SetPosition();
                PlayerModel.SetActive(true);
            }

            if(hasAuthority){
                //Everything in here occurs for each client seperately to their own
                Movement();
            }
        }
    }

    public void SetPosition(){
        Debug.Log(this.gameObject.name + ": " + this.gameObject.transform.position);
        transform.position = new Vector3(Random.Range(-5,5),0.8f,Random.Range(-15,7));
        Debug.Log(this.gameObject.name + ": " + this.gameObject.transform.position);
    }

    public void Movement(){
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(xDirection,0f,zDirection);
        transform.position += moveDirection * Speed * Time.deltaTime;
    }
}
