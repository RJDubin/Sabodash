using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sabotages : MonoBehaviour {
    // Start is called before the first frame update

    public struct Sabotage {
        public String name;
        public int dur;
        public int cost;
        public bool overlappable;
        public int relProb;

        public Sabotage(String newname, int duration, int mycost, bool overlap, int relativeProb) {
            name = newname;
            dur = duration;
            cost = mycost;
            overlappable = overlap;
            relProb = relativeProb;
        }
    }

    // Sabotage overlapping control
    private static List<bool> sabotageInUse = new List<bool>();

    private static float embiggenScale = 1.5f;
    public static List<String> sabNamesList = new List<String>();

    // Sabotage costs and durations
    // Sabotage(name, duration, cost, cd, overlappable)
    public static List<Sabotage> sabVars = new List<Sabotage>();

    // Sabotage probabilites
    private static List<int> sabProbs = new List<int>();

    void Start() {

        sabVars.Add(new Sabotage("Big ", 5, 1, false, 1));
        sabVars.Add(new Sabotage("Grey", 5, 1, false, 1));
        sabVars.Add(new Sabotage("Grav", 5, 1, false, 1));
        sabVars.Add(new Sabotage("Ctrl", 5, 1, false, 1));
        sabVars.Add(new Sabotage("Bncy", 5, 1, false, 1));
        sabVars.Add(new Sabotage("Stop", 3, 1, false, 1));
        sabVars.Add(new Sabotage("Frwd", 3, 1, false, 1));

        // Initialize control lists
        for (int i = 0; i < sabVars.Count; i++) {
            // Add to names
            sabNamesList.Add(sabVars[i].name);

            // Usage
            sabotageInUse.Add(false);

            // Probabilities
            for (int j = 0; j < sabVars[i].relProb; j++) {
                sabProbs.Add(i);
            }
        }

    }

    public static int GrantSabotage() {
        int sabotageClaimedIndex = UnityEngine.Random.Range((int)0, (int)sabProbs.Count);
        return sabProbs[sabotageClaimedIndex];
    }

    public static void ApplySabotage(int sabTriggered, Player callingPlayer) {
        if ((sabVars[sabTriggered].overlappable || !sabotageInUse[sabTriggered])) {
            Player furthest = callingPlayer;
            foreach (Player p in GameState.alivePlayers) {
                if (p != callingPlayer && sabTriggered != 6) {
                    if (sabTriggered == 0) {// Makes other players bigger and slower
                        p.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f) * embiggenScale;
                        p.sab_vel_percent = 0.5f;
                    }
                    if (sabTriggered == 1) { p.sprite.color = Color.gray; }
                    if (sabTriggered == 2) { p.rigbod.gravityScale = -1 * Mathf.Abs(p.rigbod.gravityScale); }
                    if (sabTriggered == 3) { p.directionScale *= -1; }
                    if (sabTriggered == 4) { p.boxcollider.sharedMaterial = p.mat_bouncy; }
                    if (sabTriggered == 5) { p.sab_vel_percent = 0f; }
                    p.outline.color = callingPlayer.sprite.color;
                }
                else if (p.transform.position.x > furthest.transform.position.x) furthest = p;
            }
            if(sabTriggered == 6) callingPlayer.transform.position = furthest.transform.position;
            // Sabotage succesfully used
            sabotageInUse[sabTriggered] = true;
        }

        else {
            // Sabotage failed to use
        }
    }
    public static void ResetSabotage(int sabNumber, Player player) {
        // Reset only applied sabotage
        foreach (Player p in FindObjectsOfType<Player>()) {
            if (sabNumber == 0) {
                p.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); ;
                p.sab_vel_percent = 1f;
            }
            if (sabNumber == 1) { p.sprite.color = GameState.possibleColours[p.colourIndex]; }
            if (sabNumber == 2) { p.rigbod.gravityScale = Mathf.Abs(p.rigbod.gravityScale); }
            if (sabNumber == 3) { p.directionScale = Mathf.Abs(p.directionScale); }
            if (sabNumber == 4) { p.boxcollider.sharedMaterial = p.mat_normal; }
            if (sabNumber == 5) { p.sab_vel_percent = 1f; }
            if (sabNumber == 6) { }
            if (sabNumber == 7) { }
            p.outline.color = Color.black;
        }
        sabotageInUse[sabNumber] = false;
    }

    // Update is called once per frame
    void Update() {

    }
}