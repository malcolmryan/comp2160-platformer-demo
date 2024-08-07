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

    [SetUp]
    override public void Setup()
    {
        base.Setup();
        // Note: this does not complete until the next frame
        SceneManager.LoadScene("Tests/PlayMode/Scenes/Test Room 1");
    }
    
    [TearDown]
    public void Teardown()
    {
        base.TearDown();
    }

    [UnityTest]
    public IEnumerator TestGravity0()
    {
        Vector3 pos = new Vector3(-5,-4,0);
        GameObject player = GameObject.Instantiate(playerPrefab, pos, Quaternion.identity);
        Rigidbody2D rigidbody = player.GetComponent<Rigidbody2D>();

        // ContactTracker needs to run for a frame to register contact
        Assert.That(rigidbody.position.x, Is.EqualTo(pos.x).Within(0.01));
        Assert.That(rigidbody.position.y, Is.EqualTo(pos.y).Within(0.01));

        yield return new WaitForFixedUpdate();
        
        Assert.That(rigidbody.position.x, Is.EqualTo(pos.x).Within(0.01));
        Assert.That(rigidbody.position.y, Is.EqualTo(-4).Within(0.01));
    }

    [UnityTest]
    public IEnumerator TestGravity1()
    {
        Vector3 pos = new Vector3(4,-3,0);
        GameObject player = GameObject.Instantiate(playerPrefab, pos, Quaternion.identity);
        Rigidbody2D rigidbody = player.GetComponent<Rigidbody2D>();

        // ContactTracker needs to run for a frame to register contact
        Assert.That(rigidbody.position.x, Is.EqualTo(pos.x).Within(0.01));
        Assert.That(rigidbody.position.y, Is.EqualTo(pos.y).Within(0.01));

        yield return new WaitForFixedUpdate();
        
        Assert.That(rigidbody.position.x, Is.EqualTo(pos.x).Within(0.01));
        Assert.That(rigidbody.position.y, Is.EqualTo(-3).Within(0.01));
    }

    [UnityTest]
    public IEnumerator TestGravity2()
    {
        Vector3 pos = new Vector3(-5,0,0);
        GameObject player = GameObject.Instantiate(playerPrefab, pos, Quaternion.identity);
        Rigidbody2D rigidbody = player.GetComponent<Rigidbody2D>();

        Assert.That(rigidbody.position.x, Is.EqualTo(pos.x).Within(0.01));
        Assert.That(rigidbody.position.y, Is.EqualTo(pos.y).Within(0.01));

        yield return new WaitForSeconds(1);

        Assert.That(rigidbody.position.x, Is.EqualTo(pos.x).Within(0.01));
        Assert.That(rigidbody.position.y, Is.EqualTo(-4).Within(0.01));
    }

    [UnityTest]
    public IEnumerator TestGravity3()
    {
        Vector3 pos = new Vector3(4,0,0);
        GameObject player = GameObject.Instantiate(playerPrefab, pos, Quaternion.identity);
        Rigidbody2D rigidbody = player.GetComponent<Rigidbody2D>();

        Assert.That(rigidbody.position.x, Is.EqualTo(pos.x).Within(0.01));
        Assert.That(rigidbody.position.y, Is.EqualTo(pos.y).Within(0.01));

        yield return new WaitForSeconds(1);

        Assert.That(rigidbody.position.x, Is.EqualTo(pos.x).Within(0.01));
        Assert.That(rigidbody.position.y, Is.EqualTo(-3).Within(0.01));
    }

}
