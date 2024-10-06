using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private Camera mainCamera;
    private int width;
    private int height;
    [SerializeField] float speed;
    [SerializeField] float zoomSpeed = 1f;
    public bool CamMove;
    private Vector3 CamPos;
    private Vector2 LastCursorPos;
    // Start is called before the first frame update
    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        CamPos = Vector3.zero;
        mainCamera = GetComponent<Camera>();
    }

    public RaycastHit Intersection(Vector2 Pos, float raylenght)
    {
        Ray ray = mainCamera.ScreenPointToRay(Pos);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, raylenght);
        Debug.DrawRay(ray.origin, ray.direction * raylenght, Color.green, 0.0001f);
        return hit;
    }

    // Update is called once per frame
    void Update()
    {
        LastCursorPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);        
    }

    private void LateUpdate()
    {
        transform.Translate(speed * transform.position.y * CamPos * Time.deltaTime);
    }
    private void FixedUpdate()
    {
        float mw = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            RaycastHit hit = Intersection(mousePos, 1000);
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Building"))
            {
                hit.collider.gameObject.GetComponent<Building>()?.Select();
            }
        }

        else if (Input.GetMouseButton(1))
        {
            Vector2 dp = (LastCursorPos - new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 pos = new Vector2(width / 2, height / 2);
            RaycastHit hit = Intersection(pos, 1000);
            transform.RotateAround(hit.point, Vector3.up, -dp.x * speed * transform.position.y * Time.deltaTime);
        }
        else if (Input.mousePosition.x < 20)
        {
            CamPos.x = -1;
        }
        else if (Input.mousePosition.x > Screen.width - 20)
        {
            CamPos.x = 1;
        }
        else if (Input.mousePosition.y < 20)
        {
            CamPos.z = -Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad); ;
            CamPos.y = -Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad); ;
        }
        else if (Input.mousePosition.y > Screen.height - 20)
        {
            CamPos.z = Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad); ;
            CamPos.y = Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad); ;
        }
        else if (mw > 0.1)
        {
            CamPos.z = 1;

        }
        else if (mw < -0.1)
        {
            CamPos.z = -1;
        }
        else
        {
            CamPos.x = 0;
            CamPos.y = 0;
            CamPos.z = 0;
        }
        mainCamera.orthographicSize -= mw * zoomSpeed;
    }
}
