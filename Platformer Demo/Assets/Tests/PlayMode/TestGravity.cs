using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TestGravity : InputTestFixture
{
    private GameObject playerPrefab = Resources.Load<GameObject>("Player");
    private Rigidbody2D rigidbody;
    private ContactTracker2D contacts;

    [SetUp]
    override public void Setup()
    {
        base.Setup();
        // Note: this does not complete until the next frame
        SceneManager.LoadScene("Tests/PlayMode/Scenes/TestFlat");
    }
    
    [TearDown]
    public void Teardown()
    {
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator TestGravity0()
    {
        Vector3 pos = new Vector3(0,-4,0);
        GameObject player = GameObject.Instantiate(playerPrefab, pos, Quaternion.identity);
        rigidbody = player.GetComponent<Rigidbody2D>();
        contacts = player.GetComponent<ContactTracker2D>();

        // ContactTracker needs to run for a frame to register contact
        Assert.AreEqual(0, contacts.NumContacts);
        Assert.That(rigidbody.position.y, Is.EqualTo(-4).Within(0.01));

        yield return new WaitForFixedUpdate();
        
        Assert.AreEqual(1, contacts.NumContacts);
        Assert.That(rigidbody.position.y, Is.EqualTo(-4).Within(0.01));
    }

    [UnityTest]
    public IEnumerator TestGravity1()
    {
        GameObject player = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        rigidbody = player.GetComponent<Rigidbody2D>();
        contacts = player.GetComponent<ContactTracker2D>();

        Assert.AreEqual(0, contacts.NumContacts);
        Assert.That(rigidbody.position.y, Is.EqualTo(0).Within(0.01));

        yield return new WaitForSeconds(1);

        Assert.AreEqual(1, contacts.NumContacts);
        Assert.That(rigidbody.position.y, Is.EqualTo(-4).Within(0.01));
    }
}
