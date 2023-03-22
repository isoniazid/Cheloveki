using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(Camera))]


public sealed class Observer : MonoBehaviour {

    [Range(0, 10f)]
    public float moveSpeed = 10f;
    [Range(0f, 5f)]
    public float sensitivity = 3;
    public bool isDragging { get; private set; }
    public new Camera camera { get; private set; }

    private Vector2 tempCenter, targetDirection, tempMousePos;
    private float tempSens;

    public float SCALE_STEP = 0.1f;

    private void Start() 
    {
        this.camera = GetComponent<Camera>();
        //this.camera.enabled = true;
        //this.camera.ortographic = true;
    }

    private void Update() 
    {
        UpdateInput();
        UpdatePosition();
    }

    private void UpdateInput() 
    {
        Vector2 mousePosition = Input.mousePosition;
        if (Input.GetMouseButtonDown(0)) OnPointDown(mousePosition);
        else if (Input.GetMouseButtonUp(0)) OnPointUp(mousePosition);
        else if (Input.GetMouseButton(0)) OnPointMove(mousePosition);

        if (Input.GetKey(KeyCode.E)) ChangeScale(SCALE_STEP);
        if (Input.GetKey(KeyCode.Q)) ChangeScale(-SCALE_STEP);

    }

    private void ChangeScale(float value)
    {
        this.camera.fieldOfView +=value;
    }




    private void UpdatePosition() 
    {
        float speed = Time.deltaTime * this.moveSpeed;
        if (this.isDragging) this.tempSens = this.sensitivity;
        else this.tempSens = Mathf.Lerp(this.tempSens, 0f, speed);
        Vector2 newPosition = this.position + this.targetDirection * this.tempSens;
        this.position = Vector2.Lerp(this.position, newPosition, speed);
    }

    private void OnPointDown(Vector2 mousePosition) 
    {
        this.tempCenter = GetWorldPoint(mousePosition);
        this.targetDirection = Vector2.zero;
        this.tempMousePos = mousePosition;
        this.isDragging = true;
    }

    private void OnPointMove(Vector2 mousePosition) 
    {
        if (this.isDragging) 
        {
            Vector2 point = GetWorldPoint(mousePosition);
            float sqrDist = (this.tempCenter - point).sqrMagnitude;
            if (sqrDist > 0.1f) 
            {
                this.targetDirection = (this.tempMousePos - mousePosition).normalized;
                this.tempMousePos = mousePosition;
            }
        }
    }

    private void OnPointUp(Vector2 mousePosition) 
    {
        this.isDragging = false;
    }

    

    public Vector2 position 
    {
        get { return this.transform.position;}
        set { this.transform.position = new Vector3(value.x, value.y, -10f); }
    }

    private Vector2 GetWorldPoint(Vector2 mousePosition) 
    {
        Vector2 point = Vector2.zero;
        Ray ray = this.camera.ScreenPointToRay(mousePosition);
        Vector3 normal = Vector3.forward;
        Vector3 position = Vector3.zero;
        Plane plane = new Plane(normal, position);
        float distance;
        plane.Raycast(ray, out distance);
        point = ray.GetPoint(distance);
        return point;
    }

}