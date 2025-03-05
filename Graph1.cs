using UnityEngine;


public class Graph1 : MonoBehaviour
{
    [SerializeField] private Transform pointPrefab;
    [SerializeField] private int Resolution;
    private Vector3 position = Vector3.zero;
    Vector3 scale;
    [SerializeField] private float waveAmplitude;
    [SerializeField] private float waveFrequency;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float scaleInp;
    [SerializeField] private float timeNow;
    [SerializeField] private float waitTime;
    [SerializeField, Min(0f)] private float functionDuration = 1f,transitionDuration =1f;
    [SerializeField,Tooltip("Helps Choose which wave Function to Choose")] private Wave.FunctionName function;
    [SerializeField] private Wave.FunctionName transitionFunction;
    bool transitioning;

    private enum TransitionType {Next,RandomType }
    [SerializeField]
    TransitionType transitionType;

    Transform[] points;
    Wave.Function f;


    void Awake() 
    {
        points = new Transform[Resolution*Resolution];
        
        scale = Vector3.one*scaleInp / Resolution;
        for (int i = 0; i<Resolution*Resolution; i++) 
        {
        
            Transform pointInstance = Instantiate(pointPrefab);
            
            pointInstance.localScale = scale;
            pointInstance.SetParent(this.transform,false);

            points[i] = pointInstance;
        }
        

    }

    void Update()
    {
        timeNow += Time.unscaledDeltaTime;
        if (transitioning) 
        {
            if (timeNow >= transitionDuration) 
            {
                timeNow -= transitionDuration;
                transitioning = false;
            }
        }
        else if (timeNow >= waitTime)
        {
            timeNow -= waitTime;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }
        

        if (transitioning)
        {
            UpdateFunctionTransition();
        }
        else 
        {
            UpdateFunction();
        }


        
        
    }

    void UpdateFunction() 
    {
        f = Wave.GetFunction(function);
        float Step = 2f / Resolution;
        float v = 0.5f * Step - 1f;
        float time = moveSpeed * Time.time;
        
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == Resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * Step - 1f;
            }

            float u = (x + 0.5f) * Step - 1f;


            points[i].position = f(u, v, time);

        }
    }
    void UpdateFunctionTransition ()
    {
        Wave.Function from = Wave.GetFunction(transitionFunction),
            to = Wave.GetFunction(function);
        float progress = timeNow / transitionDuration;
        float time = Time.time;
        float Step = 2f / Resolution;
        float v = 0.5f * Step - 1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)  
        {
            if (x == Resolution) 
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * Step - 1f;
            }

            float u = (x + 0.5f) * Step - 1f;
            points[i].position = Wave.Morph(u,v,time,from,to,progress);
        }
    }

    void PickNextFunction() 
    {
        function = transitionType == TransitionType.Next ?
            Wave.GetNextFunction(function) :
            Wave.GetRandomFunction(function);
    }






}
