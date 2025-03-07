using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField,Range(1,8)]
    private int depth = 4;


    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    public FractalPart[][] parts;
    Matrix4x4[][] matrices; 

    static Quaternion[] quatrnionDirs = new Quaternion[] {Quaternion.Euler(0f, 0f, -90f),
        Quaternion.identity,Quaternion.Euler(0f,0f,90f),Quaternion.Euler(0f,90f,0f),
    Quaternion.Euler(0f,-90f,0f),Quaternion.Euler(-90f,0f,0f)};

    static Vector3[] vectorDir = new Vector3[] {Vector3.right,
    Vector3.up,Vector3.left,Vector3.forward,Vector3.back,Vector3.down};

    //FractalPart Struct
    public struct FractalPart  
    {
        public Vector3 direction,worldPosition;//we need public keyword to access the property!
        //public keyword only makes these components public in Fractal as Struct FractalPart is private! 
        public Quaternion rotation,worldRotation;
        //public Transform transform;//Transfrom To scale!//No Longer Required as we use Procedural Drawing and do not require a gameObject;
        public float spinAngle;
    }

    ComputeBuffer[] matricesBuffers;
    
    private void OnEnable()
    {
        parts = new FractalPart[depth][]; //here we create dataStructure
        matrices = new Matrix4x4[depth][];//4x4 matrix for each fractal part;
        matricesBuffers = new ComputeBuffer[depth];
        int stride = 16 * 4;

        for (int i = 0, length = 1; i < parts.Length; i++, length *= 5) //as there are five directions so (5 is multiplied)
        {                                                               //each iteration gets 5times the last items
            parts[i] = new FractalPart[length];
            matrices[i] = new Matrix4x4[length];//five times more parts so 5 times Matrices
            matricesBuffers[i] = new ComputeBuffer(length,stride);
        }


        //float scale = 1f; NO lonager required in Procedural drawing
        parts[0][0] = CreatePart(0); //Create FractalPart

        for(int li =1;li<parts.Length;li++) //Since first Fractal is just sphere, li starts from 1 
        {                                   //parts.length is [n][m]= n
            // scale *= 0.5f;  Scale Will be applied in 
            FractalPart[] levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi+=5) 
            {
                for (int ci = 0; ci < 5; ci++) 
                {
                    levelParts[fpi + ci] = CreatePart(ci);
                }
                
            }
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < matricesBuffers.Length; i++) 
        {
            matricesBuffers[i].Release();
        }
        parts = null;
        matrices = null;
        matricesBuffers = null;

        
    }

    private void OnValidate()//Editor-only function that Unity calls when the
                             //script is loaded or a value changes in the Inspector
    {
        if (parts != null && enabled) 
        {
            OnDisable(); 
            OnEnable();
        }
        
    }

    private void Update()
    {
        Quaternion deltaRotation = Quaternion.Euler(0f,22.5f*Time.deltaTime,0f);

        float spinAngleDelta = 22.5f * Time.deltaTime;

        FractalPart rootPart = parts[0][0];
        //rootPart.rotation *= deltaRotation;
        rootPart.spinAngle += spinAngleDelta;
        rootPart.worldRotation = rootPart.rotation*Quaternion.Euler(0f,rootPart.spinAngle,0f);
        parts[0][0] = rootPart;
        matrices[0][0] = Matrix4x4.TRS(rootPart.worldPosition,rootPart.worldRotation,Vector3.one);

        float scale = 1f;
        for (int li = 1; li < parts.Length; li++) 
        {
            scale *= 0.5f;
            FractalPart[] parentParts = parts[li - 1];
            FractalPart[] levelParts = parts[li];
            Matrix4x4[] levelMatrices = matrices[li];
            for (int fpi=0; fpi < levelParts.Length; fpi++) 
            {
                FractalPart parent = parentParts[fpi / 5];
                FractalPart part = levelParts[fpi];
                part.spinAngle += spinAngleDelta;
                part.rotation *= deltaRotation;
                part.worldRotation = parent.worldRotation*(part.rotation*Quaternion.Euler(0f,part.spinAngle,0f)); 
                part.worldPosition = parent.worldPosition + parent.worldRotation*(1.50f*scale * part.direction);
                levelParts[fpi] = part;
                levelMatrices[fpi] = Matrix4x4.TRS(part.worldPosition,part.worldRotation,scale*Vector3.one);
                //part.transform.localRotation = part.rotation;
            }
        }
    }


    public FractalPart CreatePart(int childIndex) => new FractalPart //Faster Way to deeclare a function of return type FractalPart
    {
        /* var go = new GameObject("Fractal_Part L" + levelIndex+"C"+childIndex);
         go.transform.localScale = scale * Vector3.one; 
         go.transform.SetParent(transform,false);
         go.AddComponent<MeshFilter>().mesh = mesh;
         go.AddComponent<MeshRenderer>().material = material;*/
        direction = vectorDir[childIndex],
        rotation = quatrnionDirs[childIndex]

        // return new FractalPart { };
    };


































    /*
    private void Start()
    {
        name = "Fractal" + depth;
        if (depth <= 1) { return; }

        
        
        Fractal childA = CreateFractal(vectorDir[0], quatrnionDirs[0]);
        Fractal childB = CreateFractal(vectorDir[1], quatrnionDirs[1]);
        Fractal childC = CreateFractal(vectorDir[2], quatrnionDirs[2]);
        Fractal childD = CreateFractal(vectorDir[3], quatrnionDirs[3]);
        Fractal childE = CreateFractal(vectorDir[4], quatrnionDirs[4]);
        Fractal childF = CreateFractal(vectorDir[5], quatrnionDirs[5]);
       /*
        Fractal childB = CreateFractal(Vector3.up, Quaternion.identity);
        Fractal childC = CreateFractal(Vector3.left, Quaternion.Euler(0f,0f,90f));
        Fractal childD = CreateFractal(Vector3.forward, Quaternion.Euler(0f,90f,0f));
        Fractal childE = CreateFractal(Vector3.back, Quaternion.Euler(0f,-90f,0f));
        Fractal childF = CreateFractal(Vector3.down, Quaternion.Euler(-90f,0f,0f));
       
        childA.transform.SetParent(transform,false);
        childB.transform.SetParent(transform,false);
        childC.transform.SetParent(transform,false);
        childD.transform.SetParent(transform,false);
        childE.transform.SetParent(transform,false);
        childF.transform.SetParent(transform,false);
        
    }

    private void Update()
    {
        transform.Rotate(0f,22.5f*Time.deltaTime,0f);
    }

    public Fractal CreateFractal(Vector3 direction,Quaternion orientation) 
    {
        Fractal child = Instantiate(this);
        child.depth = depth-1;
        child.transform.localPosition = 0.75f * direction;
        child.transform.localRotation = orientation;
        child.transform.localScale = 0.5f*Vector3.one;
        return child;

    }
*/

}
