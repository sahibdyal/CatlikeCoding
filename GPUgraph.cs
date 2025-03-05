using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GPUgraph : MonoBehaviour
{

    public UnityEngine.UI.Slider resoSlider;
    [SerializeField] private TextMeshProUGUI cubeCount;
   //<ComputeShader>
    [SerializeField]ComputeShader computeShader;

    //readonly modifier is used as we dont want to accidentaly set the ID integer 
    //
    static readonly int 
        positionsID = Shader.PropertyToID("_Positions"),
        resolutionID = Shader.PropertyToID("_Resolution"),
        stepID = Shader.PropertyToID("_Step"),
        timeID = Shader.PropertyToID("_Time"),
        transitionProgressID = Shader.PropertyToID("_TransitionProgress");
    [SerializeField] Material material;
    [SerializeField] Mesh mesh;
     
    //</ComputeShader>
    
     
    [SerializeField,Range(20, 1000)] private int Resolution;
    [SerializeField] private const int maxResolution = 1000;
    private Vector3 position = Vector3.zero;
    Vector3 scale;
    [SerializeField] private float waveAmplitude;
    [SerializeField] private float waveFrequency;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float scaleInp;
    [SerializeField] private float timeNow;
    [SerializeField] private float waitTime;
    [SerializeField, Min(0f)] private float functionDuration = 1f, transitionDuration = 1f;
    [SerializeField, Tooltip("Helps Choose which wave Function to Choose")] private Wave.FunctionName function;
    [SerializeField] private Wave.FunctionName transitionFunction;
    bool transitioning;
    

    private enum TransitionType { Next, RandomType }
    [SerializeField]
    TransitionType transitionType;

    
    Wave.Function f;

    private ComputeBuffer positionsBuffer;


    

    private void OnEnable() 
    {
        positionsBuffer = new ComputeBuffer(maxResolution*maxResolution,3*4);

    }

    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }
    void Update()
    {
        Resolution = (int)resoSlider.value;
        cubeCount.SetText("Cube_Count:-\n{0:0}",Resolution*Resolution);
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
        timeNow += Time.deltaTime;

        UpdateFunctionOnGPU();

    }

    void UpdateFunctionOnGPU() 
    {
        float step = 2f / Resolution;
        computeShader.SetInt(resolutionID,Resolution);
        computeShader.SetFloat(timeID,Time.time);
        if (transitioning) 
        {
            computeShader.SetFloat(transitionProgressID, Mathf.SmoothStep(0f,1f,timeNow/transitionDuration));
        }

        computeShader.SetFloat(stepID,step);

        //Below, we Set the Buffer and give it reference of the index of the Kernel. since we have only one kernel, we use 0.
        var kernelIndex = (int)function + (int)(transitioning?transitionFunction:function)*5;
        computeShader.SetBuffer(kernelIndex,positionsID,positionsBuffer);
        //Mathf.CeilToint give an int smallest or equal int to the float its applied to!
        //Since there are 8 threads per group so, we divide resolution by 8 to define how many objects to go in each group
        int groups = Mathf.CeilToInt(Resolution / 8f); 
        computeShader.Dispatch(kernelIndex,groups,groups,1);

        material.SetBuffer(positionsID,positionsBuffer);
        material.SetFloat(stepID,step);

        //Since, Unity does not know where the positions of box
        //is? we need to specify the bound where we need rendering to happen! This is used for frustum culling
        var bounds = new Bounds(Vector3.zero,Vector3.one*(2f+2f/Resolution));
        Graphics.DrawMeshInstancedProcedural(mesh,0,material,bounds,Resolution*Resolution);

    }

    void PickNextFunction()
    {
        function = transitionType == TransitionType.Next ?
            Wave.GetNextFunction(function) :
            Wave.GetRandomFunction(function);
    }

}
