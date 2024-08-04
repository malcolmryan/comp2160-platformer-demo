using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ContactTracker2D : MonoBehaviour
{    
    public int NumContacts {
        get { return contacts.Count; }
    }

    public List<ContactPoint2D> Contacts {
        get { return contacts; }
    }

    private ContactPoint2D[] tempContacts = new ContactPoint2D[4];
    private List<ContactPoint2D> contacts = new List<ContactPoint2D>();

#region Update
    // this code runs on the standard update loop

    void Update() 
    {
    }

#endregion Update

#region Physics Update
    // this code runs on the physics update loop

    void FixedUpdate()
    {
        // clear contacts at the beginning of a new physics frame
        contacts.Clear();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        RecordContacts(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        RecordContacts(collision);
    }

    private void RecordContacts(Collision2D collision)
    {
        // Note: this is called once per collider, on every physics frame
        // So we need to collect the contacts into a list

        // Resize the array if necessary
        int nContacts = collision.contactCount;
        if (nContacts > tempContacts.Length)
        {
            tempContacts = new ContactPoint2D[nContacts];
        }

        // Get contacts and add them to the list
        collision.GetContacts(tempContacts);
        for (int i = 0; i < nContacts; i++)
        {
            contacts.Add(tempContacts[i]);
        }        
    }
#endregion Physics Update

#region Gizmos
    void OnDrawGizmos()
    {
        for (int i = 0; i < contacts.Count; i++)
        {
            ContactPoint2D c = contacts[i];
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(c.point, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(c.point, c.point + c.normal);
        }
    }
#endregion Gizmos

}
