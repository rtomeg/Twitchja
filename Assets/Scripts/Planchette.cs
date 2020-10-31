using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Planchette : MonoBehaviour
{
    private Collider col;
    private bool ouijaIsMoving;
    [SerializeField] private Texture2D handCursor;
    [SerializeField] private GameObject manita;

    private void Start()
    {
        col = GetComponent<Collider>();
        EventsManager.onOuijaResponseStarted += disableCollider;
        EventsManager.onOuijaResponseEnded += enableCollider;
    }

    private void Update()
    {
        if (ouijaIsMoving)
        {
            manita.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }
    }

    private void OnDestroy()
    {
        EventsManager.onOuijaResponseStarted -= disableCollider;
        EventsManager.onOuijaResponseEnded -= enableCollider;
    }

    private void OnMouseEnter()
    {
        Cursor.SetCursor(handCursor, Vector2.zero, CursorMode.Auto);
    }
    
    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }


    private void OnMouseDown()
    {
        EventsManager.onStartReadingTwitchResponses();
    }

    private void OnMouseUp()
    {
        EventsManager.onEndReadingTwitchResponses();
    }

    private void enableCollider(string s)
    {
        ouijaIsMoving = false;
        manita.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        col.enabled = true;
    }

    private void disableCollider()
    {
        ouijaIsMoving = true;
        manita.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        col.enabled = false;
    }
}