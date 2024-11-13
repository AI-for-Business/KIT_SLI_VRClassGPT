using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using System;
using System.Collections.Generic;

public class CSV_Sender : MonoBehaviour
{
    [SerializeField]
    private string flaskServerUrl = "http://172.17.52.147:5000/upload"; // Replace <IPv4> with your server's IP address (X.X.X.X:5000/upload)

    public TMP_Text DebugText;
    string csvData;
    public CountScript1 countscript;
    

    void Start()
    {

        countscript = GetComponent<CountScript1>();
        // ## Start Method used for debugging only ##

        // Generate sample CSV data
        // csvData = GenerateSampleCsv();

        // For Debugging - Comment it out for 
        // StartCoroutine(UploadCsv(csvData, GenerateDateAsFilename()));


    }

    private void Update()
    {
        // Wait for Button A or X to be Pressed then send CSV
        if (OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Three))
        {
            Debug.Log("Button Pressed!");
            // Start coroutine to upload CSV
            
            StartCoroutine(UploadCsv(csvData, GenerateDateAsFilename() ));
        }
    }

    // this method is exposed to the CountScript
    public void SendEyetrackingData (List<List<float>> eyetracking_data)
    {
        string csv = GenerateCSVfromData(eyetracking_data);

        StartCoroutine(UploadCsv(csv, GenerateDateAsFilename()));

    }

    // Just for testing
    string GenerateSampleCsv()
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine("Focus on Teacher, Focus on Slides, Looked elsewhere, Total Focus, Total Time, % Focus, Focus Level");
        csv.AppendLine("1.35,2.10,1.10,3.45,4.55,0.76,Medium");
        csv.AppendLine("1.67,3.2,0.6,4.87,5.47,0.89,High");
        csv.AppendLine("1.2,2.05,2.2,3.25,5.45,0.60,Low");
        return csv.ToString();
    }

    string GenerateCSVfromData(List<List<float>> eyetrack_data)
    {
        StringBuilder csv = new StringBuilder();

        // TODO FOCUS LEVEL (Append it here and generate somewhere else)
        csv.AppendLine("Focus on Teacher; Focus on Slides; Looked elsewhere; Total Focus; Total Time; % Focus");

        foreach (List<float> sectionData in eyetrack_data)
        {
            Debug.Log(sectionData);
            string row = string.Join(";", sectionData);
            csv.AppendLine(row);
           
        }

        return csv.ToString();
        Debug.Log("Generated CSV from EyeTrack");
    }

    IEnumerator UploadCsv(string csvData, string filename)
    {
        byte[] csvBytes = Encoding.UTF8.GetBytes(csvData);

        // Create a form and attach the CSV
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", csvBytes, filename, "text/csv");

        Debug.Log("Trying to reach: " + flaskServerUrl);

        using (UnityWebRequest www = UnityWebRequest.Post(flaskServerUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string debug_msg = "CSV uploaded successfully!";
                Debug.Log(debug_msg);
                DebugText.text = debug_msg;

            }
            else
            {
                string debug_msg = "Error uploading CSV: " + www.error;
                Debug.LogError(debug_msg);
                DebugText.text = debug_msg;
            }
        }
    }

    string GenerateDateAsFilename ()
    {
        DateTime now = DateTime.Now;

        // Format the string as YYYY-MM-DD_HH-MM-SS
        string timestamp = now.ToString("yyyy-MM-dd_HH-mm-ss");

        // Use the timestamp as the file name
        string fileName = $"eyetrack_data_{timestamp}.csv";

        return fileName;
    }
}
