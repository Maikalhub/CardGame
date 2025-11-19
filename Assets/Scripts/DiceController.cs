using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    public List<Rigidbody> Dice = new List<Rigidbody>();
    bool rolling = false;

    private void Update()
    {
        //Example: how to roll dice
        if (Input.GetMouseButtonDown(0))
        {
            RollDice(Dice);
            rolling = true;
        }

        //Example: how to sense when the dice stopped
        if (AllDiceStoppedMoving(Dice) == true && rolling == true)
        {
            Debug.Log("Stopped");
            rolling = false;

            //Example: how to get the top  number when dice stopped
            Debug.Log(DiceUpwardGamobject(Dice, 0).name);
            Debug.Log(DiceUpwardGamobject(Dice, 1).name);
        }
    }

    public void RollDice(List<Rigidbody> DiceList)
    {
        foreach (Rigidbody rb in DiceList)
        {
            rb.transform.position = Vector3.zero;

            int x = Random.Range(0, 360);
            int y = Random.Range(0, 360);
            int z = Random.Range(0, 360);
            Quaternion rotation = Quaternion.Euler(x, y, z);

            x = Random.Range(-25, 25);
            y = Random.Range(0, 25);
            z = Random.Range(-25, 25);
            Vector3 force = new Vector3(x, -y, z);

            x = Random.Range(-50, 50);
            y = Random.Range(-50, 50);
            z = Random.Range(-50, 50);
            Vector3 torque = new Vector3(x, y, z);

            rb.transform.rotation = rotation;
            rb.linearVelocity = force;
            rb.AddTorque(torque, ForceMode.VelocityChange);
        }
    }

    public bool AllDiceStoppedMoving(List<Rigidbody> DiceList)
    {
        foreach (Rigidbody rb in DiceList)
        {
            if(rb.linearVelocity != Vector3.zero || rb.angularVelocity != Vector3.zero)
            {
                return false;
            }
        }

        return true;
    }

    public GameObject DiceUpwardGamobject(List<Rigidbody> DiceList, int indexFromList)
    {
        List<GameObject> objects = new List<GameObject>();

        foreach (Transform sideDetector in DiceList[indexFromList].transform.GetComponentInChildren<Transform>())
        {
            objects.Add(sideDetector.gameObject);
        }

        GameObject highestObject = objects.OrderByDescending(obj => obj.transform.position.y).FirstOrDefault();

        if (highestObject != null)
        {
            return highestObject;
        }
        else
        {
            return null;
        }
    }
}
