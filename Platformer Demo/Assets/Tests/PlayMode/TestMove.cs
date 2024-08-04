using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class TestMove : InputTestFixture
{
    private GameObject playerPrefab = Resources.Load<GameObject>("Player");
    private Rigidbody2D rigidbody;
    private ContactTracker2D contacts;

    private Actions actions;
    private InputAction moveAction;

    private Keyboard keyboard;
    private Gamepad gamepad;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        base.Setup();
        // Note: this does not complete until the next frame
        SceneManager.LoadScene("Tests/PlayMode/Scenes/TestFlat");
        yield return null;
     
        keyboard = InputSystem.AddDevice<Keyboard>();
        gamepad = InputSystem.AddDevice<Gamepad>();
        Set(gamepad.leftStick, new Vector2(1,0));

    }
    
    [TearDown]
    public void Teardown()
    {
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator TestMove0()
    {
        Vector3 pos = new Vector3(-4,-4,0);
        GameObject player = GameObject.Instantiate(playerPrefab, pos, Quaternion.identity);

        rigidbody = player.GetComponent<Rigidbody2D>();
        contacts = player.GetComponent<ContactTracker2D>();

        // physics needs to run for a frame to register contact
        Assert.AreEqual(0, contacts.NumContacts);
        Assert.That(rigidbody.position.y, Is.EqualTo(-4).Within(0.01));

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(1, contacts.NumContacts);
        Assert.That(rigidbody.position.y, Is.EqualTo(-4).Within(0.01));


        yield return new WaitForSeconds(10);
    }

}
