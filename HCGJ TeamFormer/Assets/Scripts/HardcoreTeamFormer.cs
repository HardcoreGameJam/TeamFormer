using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HardcoreTeamFormer : MonoBehaviour {

    public bool debugMode;
    public float lowerInterval = 0.1f;
    public float upperInterval = 0.3f;
    public Text[] nameFields;
    public Animator hackerAnimation;
    public AudioClip selectParticipantSfx;

    private const int TEAM_SIZE = 3;

    private int tempSelectedParticipants;
    private ArrayList artists;
    private ArrayList designers;
    private ArrayList developers;
    private ArrayList drawnableParticipants;
    private AudioSource audioSource;

    private void Awake()
    {
        // Init components and variables
        audioSource = GetComponent<AudioSource>();
        artists = new ArrayList(HardcoreVariables.artists);
        designers = new ArrayList(HardcoreVariables.designers);
        developers = new ArrayList(HardcoreVariables.developers);
        drawnableParticipants = new ArrayList(artists.Count + designers.Count + developers.Count);

        // Update remaining participants and start shuffling
        UpdateDrawnableParticipants();
        ResetShuffling();

        // Log debug info
        if (debugMode)
        {
            PrintRemainingParticipantsCount();
        }
    }

    private void PrintRemainingParticipantsCount()
    {
        print(string.Format("= = = HCGJ = = = There are {0} participants left to be split into teams.", drawnableParticipants.Count));
    }

    private void UpdateDrawnableParticipants() {
        drawnableParticipants.Clear();
        drawnableParticipants.AddRange(artists);
        drawnableParticipants.AddRange(designers);
        drawnableParticipants.AddRange(developers);
    }

    private void Update()
    {
        // Select participant button
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            if (drawnableParticipants.Count > 0 && tempSelectedParticipants < TEAM_SIZE) 
            {
                DrawParticipant();
            }
        }

        // Create a screenshot
        if (Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/" + DateTime.Now.ToString("HHmmssfff") + ".jpg");
        }

        // Reset button
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (tempSelectedParticipants >= TEAM_SIZE) 
            {
                // Update remaining participants
                UpdateDrawnableParticipants();

                // Reset audio and visuals, selectedParticipants count and start shuffling again
                if (drawnableParticipants.Count > 0)
                {
                    //audioSource.Play();
                    hackerAnimation.enabled = true;
                    foreach (Text nameField in nameFields)
                    {
                        nameField.text = "";
                    }
                    tempSelectedParticipants = 0;
                    ResetShuffling();
                }

                // Log debug info
                if (debugMode)
                {
                    PrintRemainingParticipantsCount();
                }
            }
        }
    }

    private void DrawParticipant() {
        
        // Draw a random participant 
        string participant = (string) drawnableParticipants[Random.Range(0, drawnableParticipants.Count)];

        // Check what category this participant falls into
        RemoveParticipantByCategory(participant);

        // Increase the count of selected participants
        tempSelectedParticipants++;

        // Play SFX and update UI
        StopAllCoroutines();
        nameFields[tempSelectedParticipants - 1].text = "";
        audioSource.PlayOneShot(selectParticipantSfx);
        UpdateParticipantsFields(participant);
        ResetShuffling();

        // Stop background sound and animation if team is fully selected
        if (tempSelectedParticipants >= TEAM_SIZE) {
            //audioSource.Stop();
            hackerAnimation.enabled = false;
        } 

        // Log debug info
        if (debugMode)
        {
            print(string.Format("{0} was drawn and could be in a team with {1} others. We now have {2} temporary selected participants.", participant, drawnableParticipants.Count, tempSelectedParticipants));
        }
    }

    private void RemoveParticipantByCategory(string participant) 
    {
        if (HardcoreVariables.artists.Contains(participant))
        {
            foreach (string artist in artists)
            {
                drawnableParticipants.Remove(artist);
            }
            artists.Remove(participant);
        }
        else if (HardcoreVariables.designers.Contains(participant))
        {
            foreach (string designer in designers)
            {
                drawnableParticipants.Remove(designer);
            }
            designers.Remove(participant);
        }
        else if (HardcoreVariables.developers.Contains(participant))
        {
            foreach (string developer in developers)
            {
                drawnableParticipants.Remove(developer);
            }
            developers.Remove(participant);
        }
        if (artists.Count <= 0 || designers.Count <= 0 || developers.Count <= 0)
        {
            UpdateDrawnableParticipants();
        }
    }

    private void UpdateParticipantsFields(string participant) {
        foreach (Text participantField in nameFields) {
            if (participantField.text.Equals("")) {
                participantField.text = participant;
                break;
            }
        }
    }

    private void ResetShuffling()
    {
        foreach (var field in nameFields)
        {
            if (field.text.Equals(""))
            {
                StartCoroutine(ShuffleParticipants(Random.Range(lowerInterval, upperInterval), field));
                break;
            }
        }
    }

    private IEnumerator ShuffleParticipants(float waitTime, Text field)
    {
        while (true)
        {
            field.text = (string) drawnableParticipants[Random.Range(0, drawnableParticipants.Count)];
            yield return new WaitForSeconds(waitTime);
        }
    }
}
