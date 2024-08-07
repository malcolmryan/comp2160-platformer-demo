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

    [UnitySetUp]
    public IEnumerator Setup()
    {
        base.Setup();

        // Note: this does not complete until the next frame
        SceneManager.LoadScene("Tests/PlayMode/Scenes/Test Room 1");
        // wait a frame
        yield return null;    
    }
    
    [TearDown]
    public void Teardown()
    {
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator TestMove0()
    {
        Vector3 pos = new Vector3(-5,-4,0);
        GameObject player = GameObject.Instantiate(playerPrefab, pos, Quaternion.identity);
        rigidbody = player.GetComponent<Rigidbody2D>();

        // this needs to go here, not in Setup (why?)
        Gamepad gamepad = InputSystem.AddDevice<Gamepad>();

        // physics needs to run for a frame to register contact
        Assert.That(rigidbody.position.x, Is.EqualTo(pos.x).Within(0.01));
        Assert.That(rigidbody.position.y, Is.EqualTo(pos.y).Within(0.01));

        yield return new WaitForFixedUpdate();

        Assert.That(rigidbody.position.x, Is.EqualTo(pos.x).Within(0.01));
        Assert.That(rigidbody.position.y, Is.EqualTo(pos.y).Within(0.01));
        
        // move to the right for 1s
        Set(gamepad.leftStick, new Vector2(1,0));
        InputSystem.Update();

        yield return new WaitForSeconds(1);

        Assert.That(rigidbody.position.x, Is.EqualTo(0).Within(0.01));
        Assert.That(rigidbody.position.y, Is.EqualTo(pos.y).Within(0.01));
    }

}
