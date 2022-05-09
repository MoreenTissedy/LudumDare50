using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mix : MonoBehaviour
{
    public RectTransform centerPoint;
    public SpriteRenderer effect;
    public Animator effectAnim;
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
    public float joltTime = 0.2f;
    
    private bool mixing = false;
    private List<int> lastPositions = new List<int>(5);
    private int currentQuarter = -1;
    public float speed, mixProcess;
    private Vector2 initialEffectScale;
    
    public float MixProcess => mixProcess;

    public float Speed => speed;

    private List<float> lastSpeed = new List<float>(5);
    private float lastTime;
    private ParticleSystem.MainModule effectMain;
    

    private void Start()
    {
        effect.color = blankColor;
        initialEffectScale = effectAnim.transform.localScale;
    }

    public void SetToKey()
    {
        mixProcess = keyMixValue + Random.Range(-keyMixWindow/2, keyMixWindow/2);
        speed = 0;
    }

    public void RandomJolt()
    {
        int sign = Random.value > 0.5f ? 1 : -1; 
        DOTween.To(() => mixProcess, (value) => mixProcess = value, mixProcess + maxJolt*sign, joltTime);
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
        
        UpdateEffectSpeed();
        UpdateEffectColor();

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

    private void UpdateEffectColor()
    {
        //color overmixed
        if (Mathf.Abs(mixProcess) > overMixValue)
        {
            effect.color = overmixColor;
        }
        //color nearly overmixed
        else if (Mathf.Abs(mixProcess) > overMixThreshold)
        {
            var percent = (overMixValue - Mathf.Abs(mixProcess)) / (overMixValue - overMixThreshold);
            effect.color = Color.Lerp(overmixColor, blankColor, percent);
        }
        //color if within key window
        else if (mixProcess > (keyMixValue - keyMixWindow / 2) && mixProcess < (keyMixValue + keyMixWindow / 2))
        {
            var percentKey = Mathf.Abs(keyMixValue - mixProcess) / (keyMixWindow / 2);
            effect.color = Color.Lerp(keyColor, blankColor, percentKey);
        }
        //set color blank
        else
        {
            effect.color = blankColor;
        }
    }

    private void UpdateEffectSpeed()
    {
        effectAnim.speed = Mathf.Clamp(Mathf.Abs(speed), minSimulation, maxSimulation);
        if (speed < 0)
        {
            effectAnim.transform.localScale = new Vector3(initialEffectScale.x, -initialEffectScale.y, 1);
        }
        else
        {
            effectAnim.transform.localScale = new Vector3(initialEffectScale.x, initialEffectScale.y, 1);
        }
    }

    private void RecordQuarter(int i)
    {
        if (currentQuarter == i)
            return;

        float targetSpeed = 0;
        currentQuarter = i;
        lastPositions.Add(i);
        if (lastPositions.Count > 4)
            lastPositions.RemoveAt(0);
        if (lastTime > 0)
        {
            lastSpeed.Add(speedCoef / (Time.timeSinceLevelLoad - lastTime));
            if (lastSpeed.Count>4)
                lastSpeed.RemoveAt(0);
            targetSpeed = lastSpeed.Average();
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
            if (k - lastI == 1 || k-lastI == -3)
                clockwiseChecklist++;
            else if (k - lastI == -1 || k-lastI == 3)
                counterclockwiseCheckllist++;
            lastI = k;
        }

       
        if (clockwiseChecklist == 3)
        {
            //clockwise detected
            DOTween.To(() => speed, (value) => speed = value, targetSpeed, joltTime);
        }
        else if (counterclockwiseCheckllist == 3)
        {
            //counter-clockwise detected
            DOTween.To(() => speed, (value) => speed = value, -targetSpeed, joltTime);
        }
        else
        {
            //speed = 0;
        }
    }

  
}
