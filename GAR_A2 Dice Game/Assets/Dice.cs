using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Dice : MonoBehaviour
{
    [SerializeField] private float tolerance = 0.99f;
    
    private Rigidbody rb;
    private bool hasStoppedRolling = false; 
    private bool hasThrowDelayFinished = false; 
    private int diceIndex =-1;

    public static UnityAction<int,int> OnDiceResult;
    
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // contains logic for checking the dice if its done rolling or not
    void Update()
    {
        if (!hasThrowDelayFinished) { return; }

        //tells us if the rigid body is still detecting that it's rolling or not. If not rolling, then it executes this code
        if (!hasStoppedRolling && rb.linearVelocity.sqrMagnitude == 0f) {

            hasStoppedRolling = true;
            GetSideUP();
        } 
    }

    private void GetSideUP() {
        // map each face number to its local-space normal
        (Vector3 localDir, int value)[] faceMap = new (Vector3, int)[]
        {
            (Vector3.forward, 1),   // Front face = 1
            (Vector3.up, 2),        // Top face = 2
            (Vector3.left, 3),      // Left face = 3
            (Vector3.right, 4),     // Right face = 4
            (Vector3.down, 5),      // Bottom face = 5
            (Vector3.back, 6)       // Back face = 6     
        };

        int topValue = -1;
        float maxDot = -1f;

        foreach (var face in faceMap)
        {
            Vector3 worldDir = transform.TransformDirection(face.localDir);

            float dot = Vector3.Dot(worldDir, Vector3.up);

            if (dot > maxDot)
            {
                maxDot = dot;
                topValue = face.value;
            }
        }

        if (topValue != -1)
        {
            OnDiceResult?.Invoke(diceIndex, topValue);
            Debug.Log($"Dice {diceIndex} result: {topValue}");
        }
        else
        {
            Debug.Log("Failed to determine top face");
        }
    }

    //applies force to the dice
    internal void RollDice(float _throwForce, float _rollForce, int _diceIndex) {
        
        diceIndex = _diceIndex;

        //add a random variety on every dice throw
        float randomVariance = Random.Range(-1f, 1f);
        rb.AddForce(transform.forward * (_throwForce + randomVariance), ForceMode.Impulse); // throws dice forward

        // rotates dice randomly
        float rollX =Random.Range(0f, 1f);
        float rollY =Random.Range(0f, 1f);
        float rollZ =Random.Range(0f, 1f);

        rb.AddTorque(new Vector3 (rollX, rollY, rollZ)*(_rollForce + randomVariance));

        StartCoroutine(ThrowDelay());
    }

    private IEnumerator ThrowDelay() {
        
        yield return new WaitForSeconds(1);
        hasThrowDelayFinished = true;
    }
}
