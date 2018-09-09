using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour {
    public enum Direction
    {
        Up,Right,Down,Left
    }
    public float moveSpeed;             //Floating point variable to store the player's movement speed.
    //public bool isCollidedWithAWall;
    public List<Direction> collisionDirections;
    public Camera targetCamera;
    public GameObject DebugGeci;
    public GameObject tilemap;
    //private bool isCollidedWithAWall;
    void Start () {
        collisionDirections = new List<Direction>();
        //isCollidedWithAWall = false;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 mousePos = targetCamera.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 perpendicular = transform.position - mousePos;
        Vector3 perpendicular = Vector3.Cross(transform.position - mousePos, Vector3.forward);
        transform.rotation = Quaternion.LookRotation(new Vector3( 0,0,1), perpendicular);
        transform.rotation *= Quaternion.Euler(0, 0, -90); // this adds a 90 degrees Y rotation

        if (Input.GetMouseButtonDown(0))
        {
            int layerMask = (LayerMask.GetMask("Wall"));// LayerMask.NameToLayer("Wall")
            RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePos - transform.position, 1.0f, layerMask); 
            if (hit.collider != null)
            {
                Debug.Log(hit.point.x + " " + hit.point.y);
                var tilepos = tilemap.GetComponent<Tilemap>().WorldToCell(new Vector3(hit.point.x+(mousePos - transform.position).x/100, hit.point.y + (mousePos - transform.position).y / 100, 0));
                tilemap.GetComponent<Tilemap>().SetTile(tilepos, null); // Remove tile at 0,0,0
                Instantiate(DebugGeci, new Vector3(hit.point.x, hit.point.y,-1), Quaternion.Euler(0,0,0));
            }
        }
    }
    
    void FixedUpdate()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");
        for (int i = 0; i < collisionDirections.Count; i++)
        {
            if (collisionDirections[i] == Direction.Up)
            {
                y = Mathf.Min(y, 0);
            }
            else if (collisionDirections[i] == Direction.Down)
            {
                y = Mathf.Max(y, 0);
            }
            else if (collisionDirections[i] == Direction.Left)
            {
                x = Mathf.Max(x, 0);
            }
            else
            {
                x = Mathf.Min(x, 0);
            } 
        }
        var vect = new Vector2(x,y);
        vect = vect.normalized;

        GetComponent<Rigidbody2D>().velocity = vect*moveSpeed* Time.deltaTime;
        collisionDirections.Clear();
        /*GetComponent<Rigidbody2D>().MovePosition(
            new Vector2(transform.position.x, transform.position.y) +
            new Vector2(x: Input.GetAxisRaw("Horizontal"), y: Input.GetAxisRaw("Vertical")).normalized * moveSpeed * Time.deltaTime);*/
        //transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime, Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime, 0f));

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        collisionDirections.Clear();
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        //isCollidedWithAWall = true;
        collision.GetContacts(contacts);
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = contacts[i].normal;
            if (Mathf.Abs(normal.x) > 0.1f)
            {
                collisionDirections.Add((normal.x < 0.1f) ? Direction.Right : Direction.Left);
            }
            else
            {
                collisionDirections.Add((normal.y < 0.1f) ? Direction.Up : Direction.Down);
            }
            Debug.Log(collisionDirections[i].ToString()); 
        }
    }
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    isCollidedWithAWall = false;
    //}
}
