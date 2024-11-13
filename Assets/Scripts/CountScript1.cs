using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountScript1 : MonoBehaviour

{

    public TMP_Text teacherHoverCountText;
    public TMP_Text blackboardHoverCountText;
    public TMP_Text noFocusCountText;
    public TMP_Text lastChangeTimeStampText;

    public bool isFocusingOnTeacher = false; // The states controlling the timer
    public bool isFocusingOnBlackboard = false;
    public bool isNotFocusing = true;

    // Timer data;
    private float elapsedTimeOnTeacher = 0f; // Variable to store the elapsed time
    private float elapsedTimeOnBlackboard = 0f;
    private float elapsedTimeOff = 0f;
    private float elapsedTime = 0f;

    // Data used to track the 
    public CSV_Sender sender;
    public List<List<float>> eyetrack_data = new List<List<float>>();
    public float endOfSection_1;
    public float endOfSection_2;
    public float endOfSection_3;

    private float section1_length;
    private float section2_length;
    private float section3_length;

    private bool currentSection_is_2 = false;
    private bool currentSection_is_3 = false;
    private bool lecture_finished = false;




    // Update is called once per frame
    private void Start()
    {
        sender = GetComponent<CSV_Sender>();
        // calculate the length of each indivual section
        section1_length = endOfSection_1;
        section2_length = endOfSection_2 - endOfSection_1;
        section3_length = endOfSection_3 - endOfSection_2;
    }
    void Update()
    {
        if (isFocusingOnTeacher)
        {
            elapsedTimeOnTeacher += Time.deltaTime; // Increment the elapsed time
            teacherHoverCountText.text = elapsedTimeOnTeacher.ToString("F2"); // Update the timer text with 2 decimal places
        }

        if (isFocusingOnBlackboard)
        {
            elapsedTimeOnBlackboard += Time.deltaTime;
            blackboardHoverCountText.text = elapsedTimeOnBlackboard.ToString("F2");
        }

        if (isNotFocusing)
        {
            elapsedTimeOff += Time.deltaTime;
            noFocusCountText.text = elapsedTimeOff.ToString("F2");
        }

        elapsedTime += Time.deltaTime;


        // Switch to next section after defined time, store data in 2D List, then reset timers
        if (elapsedTime > section1_length && !currentSection_is_2)
        {
            Debug.Log("Finished Section 1");
            currentSection_is_2 = true;

            // Calculate Derived Values
            float totalFocus = elapsedTimeOnBlackboard + elapsedTimeOnTeacher;
            float totalTimeInSection1 = elapsedTime;
            float rel_Focus = totalFocus / totalTimeInSection1;

            List<float> data_section_1 = new List<float> { 
                                                elapsedTimeOnTeacher,
                                                elapsedTimeOnBlackboard,
                                                elapsedTimeOff,
                                                totalFocus,
                                                totalTimeInSection1,
                                                rel_Focus
            };
            Debug.Log(string.Join(", ", data_section_1));
            eyetrack_data.Add(data_section_1);

            // Reset Timers
            elapsedTimeOnBlackboard = 0; elapsedTimeOff = 0; elapsedTimeOnTeacher = 0; elapsedTime = 0;

        }

        if (currentSection_is_2 && !currentSection_is_3 && elapsedTime > section2_length)
        {
            Debug.Log("Finished Section 2");
            currentSection_is_3 = true;

            // Calculate derived Values

            float totalFocus = elapsedTimeOnBlackboard + elapsedTimeOnTeacher;
            float totalTimeInSection1 = elapsedTime;
            float rel_Focus = totalFocus / totalTimeInSection1;

            List<float> data_section_2 = new List<float> { 
                                                elapsedTimeOnTeacher,
                                                elapsedTimeOnBlackboard,
                                                elapsedTimeOff,
                                                totalFocus,
                                                totalTimeInSection1,
                                                rel_Focus
            };
            Debug.Log(string.Join(", ", data_section_2));
            eyetrack_data.Add(data_section_2);


            // Reset Timers
            elapsedTimeOnBlackboard = 0; elapsedTimeOff = 0; elapsedTimeOnTeacher = 0; elapsedTime = 0;

        }

        if (currentSection_is_3 && elapsedTime > section3_length && !lecture_finished)
        {
            

            // Calculate Derived Values and Store Data
            float totalFocus = elapsedTimeOnBlackboard + elapsedTimeOnTeacher;
            float totalTimeInSection1 = elapsedTime;
            float rel_Focus = totalFocus / totalTimeInSection1;

            List<float> data_section_3 = new List<float> { 
                                                elapsedTimeOnTeacher,
                                                elapsedTimeOnBlackboard,
                                                elapsedTimeOff,
                                                totalFocus,
                                                totalTimeInSection1,
                                                rel_Focus
            };
            Debug.Log(string.Join(", ", data_section_3));
            eyetrack_data.Add(data_section_3);

            Debug.Log("Finished Section3 ! Trying to Send CSV Data");
            lecture_finished = true;
            sender.SendEyetrackingData(eyetrack_data);
        }
    }



   

   
    public void countTeacherHover()
    {
        if (!isFocusingOnTeacher) lastChangeTimeStampText.text = elapsedTime.ToString("F2");
        isFocusingOnTeacher = true;
        isFocusingOnBlackboard = false;
        isNotFocusing = false;
        
    }

    public void countBlackboardHover()
    {
        if (!isFocusingOnBlackboard) lastChangeTimeStampText.text = elapsedTime.ToString("F2");
        isFocusingOnTeacher = false;
        isFocusingOnBlackboard = true;
        isNotFocusing = false;
        
    }

    public void countNoFocus()
    {
        if (!isNotFocusing) lastChangeTimeStampText.text = elapsedTime.ToString("F2");
        isFocusingOnTeacher = false;
        isFocusingOnBlackboard = false;
        isNotFocusing = true;
        
    }

    
}
