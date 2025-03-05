using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI display;
    [SerializeField,Range(0f,1f)]
    private float sampleDuration;
    private float maxDuration = float.MaxValue;
    private float minDuration = 0f;

    
     enum DisplayType{FPS,MS}

    [SerializeField]
    private DisplayType displayType;

    int frames = 0;
    float duration;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        float frameDuration = Time.unscaledDeltaTime;
        //updating each frame
        frames += 1;
        duration += frameDuration;

        if (frameDuration < maxDuration)
        {
            maxDuration = frameDuration;
        }
        if (frameDuration > minDuration) 
        {
            minDuration = frameDuration;
        }

        if (duration >= sampleDuration)
        {

            if (displayType == DisplayType.FPS)
            {
                display.SetText("FPS:\n{0:0}\n{1:0}\n{2:0}",
                1f / maxDuration,
                frames / duration,
                1f / minDuration);
            }
            else 
            {
                display.SetText("MS:\n{0:1}\n{1:1}\n{2:1}",
                1000f * maxDuration,
                1000f*duration/frames,
                1000f * minDuration);
            }
            
            duration = 0;
            frames = 0;
            maxDuration = float.MaxValue;
            minDuration = 0f;

        }
        

        

       
    }

   
}
