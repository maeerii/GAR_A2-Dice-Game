using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiceRoller : MonoBehaviour
{
    public Dice DicePrefab;
    public int AmountOfDice = 2;
    public float ThrowForce = 5f;
    public float RollForce = 10f;

    public static UnityAction<int> OnDiceRoll;

    private List<GameObject> spawnDice = new List<GameObject>();
    private List<int> results = new List<int>(); // store dice results for summary

    private void OnEnable()
    {
        Dice.OnDiceResult += CollectResult; 
    }

    private void OnDisable()
    {
        Dice.OnDiceResult -= CollectResult; 
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(RollDice());
        }
    }

    private IEnumerator RollDice() {
        OnDiceRoll?.Invoke(AmountOfDice);
        
        if (DicePrefab == null) { yield break; }

        foreach (var die in spawnDice) {
            Destroy(die);
        }
        spawnDice.Clear(); 
        results.Clear(); 

        for (int i = 0; i < AmountOfDice; i++) {
            Dice dice = Instantiate(DicePrefab, transform.position, transform.rotation);
            spawnDice.Add(dice.gameObject);
            dice.RollDice(ThrowForce, RollForce, i);
            yield return null;
        }
    } 

    // Collect results and log summary
    private void CollectResult(int diceIndex, int value) {
        if (results.Count <= diceIndex) {
            results.Add(value);
        }
        else {
            results[diceIndex] = value;
        }

        // Once all dice reported, log summary
        if (results.Count == AmountOfDice) {
            string values = string.Join(", ", results);
            int total = 0;
            foreach (int v in results) total += v;

            Debug.Log($"Rolled {AmountOfDice} dice → [{values}] (Total = {total})");
        }
    }
}
