using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    public int m_CurrentFps { get; private set; }
    public int m_MinFps { get; private set; }
    public int m_MaxFps { get; private set; }

    public bool Show = true;
    public bool MsPerFrame = false;
    public bool Peaks = false;
    public bool Memory = false;
    public bool ShowBottom = false;
    public int FontSize = 20;
    public Color FontColor = Color.white;

    float timeCounter = 0f;

    private float gcMemory = 0F;
    private float lastGcMemory = 0F;
    private float gcMemoryInMB = 0F;

    private float gcDiff = 0F;
    private float gcDiffAVG = 0F;
    private float gcDiffAVGkB = 0F;

    private float byteToMB = 0.00000095367431640625F;   //	/ 1024 / 1024;
    private float byteTokB = 0.0009765625F;         //	/ 1024 / 1024;

    private System.Text.StringBuilder stringBuilder;
    private GUIStyle myStyle;
    const int historyLength = 15;
    Queue<int> fpsHistory = new Queue<int>(historyLength + 1);

    private void Start()
    {
        stringBuilder = new System.Text.StringBuilder(80);
        myStyle = new GUIStyle { normal = new GUIStyleState { textColor = FontColor }, richText = true };
        myStyle.fontSize = FontSize * Mathf.Max(Screen.height, Screen.width) / 50 / 20;
    }

    private void Update()
    {
        //Total GC Mem
        gcMemory = (((float)System.GC.GetTotalMemory(false)));
        gcMemoryInMB = gcMemory * byteToMB;

        //Total GC Increase per Frame
        gcDiff = gcMemory - lastGcMemory;
        lastGcMemory = gcMemory;

        gcDiffAVG += (gcDiff - gcDiffAVG) * 0.03f;
        gcDiffAVGkB = gcDiffAVG * byteTokB;


        // measure average frames per second
        timeCounter += Time.unscaledDeltaTime;
        m_FpsAccumulator++;
        if (timeCounter >= fpsMeasurePeriod)
        {
            m_CurrentFps = (int)(m_FpsAccumulator / timeCounter);
            m_FpsAccumulator = 0;
            timeCounter = 0f;

            //calc peacks
            if (m_CurrentFps > 0)
            {
                fpsHistory.Enqueue(m_CurrentFps);
                while (fpsHistory.Count > historyLength)
                    fpsHistory.Dequeue();
                if (fpsHistory.Count > historyLength / 2)
                {
                    m_MinFps = fpsHistory.Min();
                    m_MaxFps = fpsHistory.Max();
                }
            }
        }
    }

    private string GetText()
    {
        stringBuilder.Length = 0;
        stringBuilder.AppendFormat("FPS: <b>{0,3:0.}</b>", m_CurrentFps);

        if (MsPerFrame)
        {
            stringBuilder.AppendFormat(" [{0,3:0.0} ms]", 1000f / m_CurrentFps);
        }

        stringBuilder.AppendLine();

        if (Peaks)
        {
            if (fpsHistory.Count > historyLength / 2)
                stringBuilder.AppendFormat("MIN: {0,3:0.}  MAX: {1,3:0.}\n", m_MinFps, m_MaxFps);
        }

        if (Memory)
        {
            stringBuilder.AppendFormat("MEM: {0,5:0.} mb {1,5:0.} kb/fr\n", gcMemoryInMB, gcDiffAVGkB);
        }

        return stringBuilder.ToString();
    }

    private void OnGUI()
    {
        if (Show)
        {
            var x = Screen.width / 2 - 60;
            var lines = 1;
            if (Memory) lines += 2;
            if (Peaks) lines += 1;
            var y = ShowBottom ? Screen.height - GUI.skin.label.fontSize * lines - 10 : 0;
            GUI.Label(new Rect(x, y, 1000, GUI.skin.label.fontSize * lines + 10), GetText(), myStyle);
        }
    }
}