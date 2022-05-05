using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mix : MonoBehaviour
{
    public RectTransform centerPoint;
    public ParticleSystem effect;
    [ColorUsage(false)]
    public Color blankColor = Color.white;
    [ColorUsage(false)]
    public Color keyColor = Color.green;
    [ColorUsage(false)]
    public Color overmixColor = Color.black;
    
    public float speedCoef = 0.2f;
    public float speedReduceRate = 0.01f;
    public float keyMixValue = 150f;
    public float keyMixWindow = 100f;
    public float overMixValue = 1000f;
    public float overMixThreshold = 800f;
    public float maxJolt = 100;
    public float minSimulation = 0.2f, maxSimulation = 5f;
    
    private bool mixing = false;
    private List<int> lastPositions = new List<int>(5);
    private int currentQuarter = -1;
    public float speed, mixProcess;
    
    public float MixProcess => mixProcess;

    public float Speed => speed;

    private List<float> lastSpeed = new List<float>(5);
    private float lastTime;
    private ParticleSystem.MainModule effectMain;
    

    private void Start()
    {
        effectMain = effect.main;
        effectMain.startColor = blankColor;
    }

    public void SetToKey()
    {
        mixProcess = keyMixValue + Random.Range(-keyMixWindow/2, keyMixWindow/2);
        speed = 0;
    }

    public void RandomJolt()
    {
        mixProcess += Random.Range(-maxJolt, maxJolt);
    }
    public void RandomKey()
    {
        var bound = (overMixThreshold - keyMixWindow / 2);
        keyMixValue = Random.Range(-bound, bound);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            mixing = true;
        if (Input.GetMouseButtonUp(0))
            mixing = false;
        
        //reduce speed with time
        speed -= speed * speedReduceRate;
        //proceed process
        mixProcess += speed;
        //effect speed
        effectMain.simulationSpeed = Mathf.Clamp(Mathf.Abs(speed), minSimulation, maxSimulation);
        //color overmixed
        if (Mathf.Abs(mixProcess) > overMixValue)
        {
            effectMain.startColor = overmixColor;
        }
        else if (Mathf.Abs(mixProcess) > overMixThreshold)
        {
            var percent = (overMixValue - Mathf.Abs(mixProcess))/(overMixValue-overMixThreshold);
            effectMain.startColor = Color.Lerp(overmixColor, blankColor, percent);
        }
        //color if within key window
        else if (mixProcess > (keyMixValue - keyMixWindow / 2) && mixProcess < (keyMixValue + keyMixWindow / 2))
        {
            var percentKey = Mathf.Abs(keyMixValue-mixProcess)/(keyMixValue - keyMixWindow / 2);
            effectMain.startColor = Color.Lerp(keyColor, blankColor, percentKey);
        }
        else
        {
            effectMain.startColor = blankColor;
        }
        
        if (!mixing)
            return;
        Vector2 pointer = Input.mousePosition;
        Vector2 center = centerPoint.position;
        if (pointer.x > center.x && pointer.y > center.y)
        {
            RecordQuarter(0);
        }
        else if (pointer.x > center.x && pointer.y < center.y)
        {
            RecordQuarter(1);
        }
        else if (pointer.x < center.x && pointer.y < center.y)
        {
            RecordQuarter(2);
        }
        else if (pointer.x < center.x && pointer.y > center.y)
        {
            RecordQuarter(3);
        }

    }

    private void RecordQuarter(int i)
    {
        if (currentQuarter == i)
            return;


        currentQuarter = i;
        lastPositions.Add(i);
        if (lastPositions.Count > 4)
            lastPositions.RemoveAt(0);
        if (lastTime > 0)
        {
            lastSpeed.Add(speedCoef / (Time.timeSinceLevelLoad - lastTime));
            if (lastSpeed.Count>4)
                lastSpeed.RemoveAt(0);
            speed = lastSpeed.Average();
        }

        lastTime = Time.timeSinceLevelLoad;

        int lastI = -1;
        int clockwiseChecklist = 0;
        int counterclockwiseCheckllist = 0;
        foreach (int k in lastPositions)
        {
            if (lastI == -1)
            {
                lastI = k;
                continue;
            }
            else if (k - lastI == 1 || k-lastI == -3)
                clockwiseChecklist++;
            else if (k - lastI == -1 || k-lastI == 3)
                counterclockwiseCheckllist++;
            lastI = k;
        }

       
        if (clockwiseChecklist == 3)
        {
            //clockwise detected
        }
        else if (counterclockwiseCheckllist == 3)
        {
            //counter-clockwise detected
            speed = -speed;
        }
        else
        {
            //speed = 0;
        }
    }

  
}
