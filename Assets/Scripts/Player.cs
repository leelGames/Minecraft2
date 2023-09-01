using UnityEngine;
using TMPro;

public class Player : MonoBehaviour {
    public Camera cam;
    public DebugScreen debugscreen;
    public Highlight highlight;
    public TextMeshProUGUI selectedBlockText;
    public CharacterController controller;

    public byte selected;
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
    float scroll;
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
		debugscreen.gameObject.SetActive(false);
		Cursor.lockState = CursorLockMode.Locked;
        realGravity = gravity;
        jumpState = 0;
        selectedBlockText.text = BD.blocks[selected].blockName;
        cam.farClipPlane = (Main.s.lodrenderDistance + 1) * VD.LODWidth;
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

    void GetPlayerInput() {
        if (Input.GetButtonDown("Cancel")) {
            Cursor.lockState = CursorLockMode.None;
        }
        if (hold) hold = false;
        if (jump != 0) hold = true;
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        jump = Input.GetAxis("Jump");
        scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetButtonDown("Sprint")) isSprinting = true;
        else if (Input.GetButtonUp("Sprint")) isSprinting = false;

        if (highlight.gameObject.activeSelf) {
            //Bl�cke abbauen
            if (Input.GetMouseButtonDown(0)) {
                Cursor.lockState = CursorLockMode.Locked;
                if (highlight.face.activeSelf) highlight.RemoveBlock();
            }
            //Bl�cke platzieren
            if (Input.GetMouseButtonDown(1)) {
                if (highlight.face.activeSelf) highlight.PlaceBlock();
            }
            if (Input.GetMouseButtonDown(2)) {
                selected = highlight.world.GetVoxel(highlight.breakPos).id;
                selectedBlockText.text = BD.blocks[selected].blockName;
            }
        }
        //TODO Inventarsystem 
        //Bl�cke ausw�hlen
        if (scroll != 0) {
            if (scroll > 0) selected++;
            else selected--;

            if (selected > (byte)BD.blocks.Length - 1) selected = 1;
            else if (selected < 1) selected = (byte)(BD.blocks.Length - 1);

            selectedBlockText.text = BD.blocks[selected].blockName;
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            highlight.world.GrowTree(highlight.breakPos + Vector3Int.down, selected);
        }
		if (Input.GetKeyDown(KeyCode.F3)) {
			debugscreen.gameObject.SetActive(!debugscreen.gameObject.activeSelf);
		}

	}
    //Logik f�r Springen und Fliegen
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
