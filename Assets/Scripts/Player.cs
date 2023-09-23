using UnityEngine;
using TMPro;

public class Player : MonoBehaviour {

    //static Player client;
    public Camera cam;
    public DebugScreen debugscreen;
    public Highlight highlight;
    public Hotbar hotbar;
    public Inventory inventory;
    public CharacterController controller;
    public InventoryManager inventoryM;
    //public MovementManager movement;
    public GameObject crosshair;

    //public int selected;

    public bool inUI;
    public int dir4;
    public int dir6;
    public bool useDir;

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
    bool hold = true;
    float verticalMomentumn;
    float realGravity;
    Vector3 velocity;

    int jumpState; //0: Nix 1: jump 2: hover 3:flyup 4: flydown
    bool isSprinting;

    public Vector3Int Position {
        get { return Vector3Int.FloorToInt(transform.position); }
        set { transform.position = value; }
    }
    public Vector3 Facing {
        get { return cam.transform.forward; }
    }

    void Start() {
        inventoryM = new(this);
        inventory.Init(); //Unity buggt rum und ruft startet Inventory nicht
		//debugscreen.gameObject.SetActive(false);
        realGravity = gravity;
        jumpState = 0;
        cam.farClipPlane = (Main.s.lodrenderDistance + 1) * VD.LODWidth;
        highlight.select(ID.items[0]);
        inUI = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        GetPlayerInput();
        GetJumpState();
        //Umsehen
        transform.Rotate(Vector3.up * mouseX);
        cam.transform.Rotate(Vector3.right * -mouseY);
        controller.Move(velocity);

        CalcVelocity();
        CalcDirection();

        highlight.PlaceHighlight();
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

    void GetPlayerInput() {
        if (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.E)) {
            inUI = !inUI;
            if (inUI) OpenInventory();
            else CloseInventory();
        }
        if (hold) hold = false;
        if (jump != 0) hold = true;
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        jump = Input.GetAxis("Jump");

        if (Input.GetButtonDown("Sprint")) isSprinting = true;
        else if (Input.GetButtonUp("Sprint")) isSprinting = false;

        if (highlight.gameObject.activeSelf) {
            //Blöcke abbauen
            if (Input.GetMouseButtonDown(0) && !inUI) {
                if (highlight.face.activeSelf) highlight.RemoveBlock();
            }
            //Blöcke platzieren
            if (Input.GetMouseButtonDown(1) && !inUI) {
                if (highlight.face.activeSelf) highlight.PlaceBlock();
            }
            if (Input.GetMouseButtonDown(2)) {
                highlight.selectFacing();
                hotbar.AddItem(ID.items[highlight.selected.item.id]);
            }
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            highlight.world.GrowTree(highlight.breakPos + Vector3Int.down, highlight.selected.id);
        }
		if (Input.GetKeyDown(KeyCode.F3)) {
			debugscreen.gameObject.SetActive(!debugscreen.gameObject.activeSelf);
		}
	}

    public CreativeInventory GetCreativeInventory() {
        CreativeInventory inventory = new();
        for (int i = 1; i < ID.items.Length; i++) {
            inventory.add(ID.items[i]);
        }
        return inventory;
    }

    //Logik für Springen und Fliegen
    void GetJumpState() {
        if (controller.isGrounded) jumpState = 0;

        if (jumpState == 0 && jump > 0) {
            jumpState = 1;
            verticalMomentumn = jumpForce * jump;
        } else if (jumpState == 1 && jump > 0 && !hold) {
            jumpState = 2;
            realGravity = 0;
        } else if (jumpState == 3 && jump > 0 && !hold) {
            jumpState = 1;
            realGravity = gravity;
        } else if (jumpState > 1 && jump > 0) {
            jumpState = 3;
            verticalMomentumn = jumpForce * jump;
        } else if (jumpState > 1 && jump < 0) {
            jumpState = 4;
            verticalMomentumn = jumpForce * jump;
        } else if (jumpState > 1) verticalMomentumn = 0;
    }

    void CalcVelocity() {
        //Gravitation
        if (!controller.isGrounded) verticalMomentumn += Time.deltaTime * realGravity;
        if (isSprinting && jumpState > 1) verticalMomentumn *= sprintSpeed;

        //Bewegung
        velocity = (transform.forward * vertical + transform.right * horizontal) * Time.deltaTime;
        if (isSprinting && jumpState > 1) velocity *= flySpeed;
        else if (isSprinting || jumpState > 1) velocity *= sprintSpeed;
        else velocity *= walkSpeed;

        velocity += verticalMomentumn * Time.deltaTime * Vector3.up;

        //TODO Kollison f�r nicht solid
        /*if (!world.getBlock(transform.position).isSolid) {
             velocity /= 6;
         }*/
    }

    //Richtung berechnen
    void CalcDirection() {

        //6-Achsen (+x-x+y-y+z-z)
        Vector3 facing = cam.transform.forward;

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
}
