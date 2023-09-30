using UnityEngine;
using TMPro;

public class Player : MonoBehaviour {

    public static Player client;
    public Camera cam;
    public DebugScreen debugscreen;
    public TextMeshProUGUI selectedBlockText;
    public Highlight highlight;
    public Hotbar hotbar;
    public Inventory inventory;
    public CharacterController controller;
    public InventoryManager inventoryM;
  
    public GameObject crosshair;

    int layermask;

    public bool inUI = false;
    public int dir4;
    public int dir6;

    public float sensitivity;
    public float reach;
    public float walkSpeed;
    public float sprintSpeed;
    public float flySpeed;
    public float jumpForce;
    public float gravity;

    float horizontal;
    float vertical;
    float mouseX;
    float mouseY;
    float jump;
    bool sprint;
    bool fly;
    bool grounded;

    float speedEffect = 1;
    float time;
    Vector3 velocity;

    public Vector3Int Position {
        get { return Vector3Int.RoundToInt(transform.position); }
        set { transform.position = value; }
    }
    public Vector3 Facing {
        get { return cam.transform.forward; }
    }

    void Start() { 
        client = this;
        layermask = ~LayerMask.GetMask("Player");
        cam.farClipPlane = (Main.s.lodrenderDistance + 1) * VD.LODWidth;
       
        inventoryM = new(this);
        inventory.Init(); //Unity buggt rum und ruft startet Inventory nicht
	
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        grounded = Physics.CheckSphere(transform.position - new Vector3(0f, transform.localScale.y, 0f), 0.4f, layermask);
        GetPlayerInput();
        CalcDirection();
        CalcMovement();
        highlight.PlaceHighlight(inventoryM.selected);
    }

    /*public void OnApplicationFocus(bool hasFocus) {//wird vor Start() aufgerufen!
        if (hasFocus) {
            if (inUI) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }
        else {
            inUI = true;
            OpenInventory();
        }
    }*/

    void GetPlayerInput() {
        if (Input.GetButtonDown("Inventory")) {
            inUI = !inUI;
            if (inUI) OpenInventory();
            else CloseInventory();
        }
        horizontal = Input.GetAxis("Horizontal") ;
        vertical = Input.GetAxis("Vertical");
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY += Input.GetAxis("Mouse Y") * sensitivity;
        mouseY = Mathf.Clamp(mouseY, -90f, 90f);
        jump = Input.GetAxis("Jump");
        sprint = Input.GetButton("Sprint");
        
        if ( Input.GetButtonDown("Jump")) {
            if (Time.time - time < 0.3f) {
                fly = !fly;
            }
            time = Time.time;
        }

        if (highlight.gameObject.activeSelf) {
            //Blöcke abbauen
            if (Input.GetMouseButtonDown(0) && !inUI) {
                if (highlight.face.activeSelf) highlight.RemoveBlock(inventoryM.selected);
            }
            //Blöcke platzieren
            if (Input.GetMouseButtonDown(1) && !inUI) {
                if (highlight.face.activeSelf) highlight.PlaceBlock(inventoryM.selected);
            }
            if (Input.GetMouseButtonDown(2)) {
               hotbar.selectItem(highlight.getFacing().item);
            }
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            World.currend.GrowTree(highlight.breakPos + Vector3Int.down, inventoryM.selected.id);
        }
		if (Input.GetKeyDown(KeyCode.F3)) {
			debugscreen.gameObject.SetActive(!debugscreen.gameObject.activeSelf);
		}
	}

    void CalcMovement() {
        //Umsehen
        transform.rotation = Quaternion.Euler(Vector3.up * mouseX);
        cam.transform.localRotation = Quaternion.Euler(Vector3.left * mouseY);
        
        //Kollison für nicht solid
        Block b = World.currend.GetBlock(Position);
        if (b.type == BType.Air || b.isSolid) speedEffect = 1f;
        else speedEffect = 0.3f;

        //Bewegung
        Vector3 move = (transform.forward * vertical + transform.right * horizontal) * speedEffect;
        if (fly) {
            move += transform.up * jump * 0.5f;
            if (sprint) move *= flySpeed;
            else move *= sprintSpeed;
        }
        else if (sprint) move *= sprintSpeed;
        else move *= walkSpeed;

        controller.Move(move * Time.deltaTime);
        
        //Gravitation v = 0.5g * t^2
        if (grounded && velocity.y < 0) velocity.y = -0.1f;
        else if (fly) velocity.y = 0f;
        else velocity.y += gravity * speedEffect * Time.deltaTime;

        if (jump >= 1f && grounded) {
            velocity.y = Mathf.Sqrt(jumpForce * gravity * speedEffect * -2f);
        }

        controller.Move(velocity * Time.deltaTime);    
    }

    void OpenInventory() {
        inventory.Load();
        inventory.gameObject.SetActive(true);
        highlight.gameObject.SetActive(false);
        crosshair.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }
    void CloseInventory() {
        inventory.gameObject.SetActive(false);
        highlight.gameObject.SetActive(true);
        crosshair.SetActive(true);
		Cursor.lockState = CursorLockMode.Locked;
    }

    //Richtung berechnen
    void CalcDirection() {
        //6-Achsen (+x-x+y-y+z-z)
        Vector3 facing = Facing;

        if (Vector3.Angle(facing, Vector3.forward) < 45) {
            dir6 = 5;
        } else if (Vector3.Angle(facing, Vector3.back) < 45) {
            dir6 = 4;
        } else if (Vector3.Angle(facing, Vector3.left) < 45) {
            dir6 = 3;
        } else if (Vector3.Angle(facing, Vector3.right) < 45) {
            dir6 = 2;
        } else if (Vector3.Angle(facing, Vector3.up) < 45) {
            dir6 = 1;
        } else {
            dir6 = 0;
        }
        //4-Achsen (+x-x+z-z)

        facing.y = 0;
        if (Vector3.Angle(facing, Vector3.left) < 45) {
            dir4 = 3;
        } else if (Vector3.Angle(facing, Vector3.right) < 45) {
            dir4 = 2;
        } else if (Vector3.Angle(facing, Vector3.back) < 45) {
            dir4 = 1;
        } else {
            dir4 = 0;
        }
    }
    public CreativeInventory GetCreativeInventory() {
        CreativeInventory inventory = new();
        for (int i = 1; i < ID.items.Length; i++) {
            inventory.add(ID.items[i]);
        }
        return inventory;
    }
}
