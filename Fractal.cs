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
            
    }

    
    
    private void Awake()
    {
        parts = new FractalPart[depth][]; //here we create dataStructure
        for (int i = 0, length = 1; i < parts.Length; i++, length *= 5) //as there are five directions so (5 is multiplied)
        {                                                               //each iteration gets 5times the last items
            parts[i] = new FractalPart[length];
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

    private void Update()
    {
        Quaternion deltaRotation = Quaternion.Euler(0f,22.5f*Time.deltaTime,0f);

        FractalPart rootPart = parts[0][0];
        rootPart.rotation *= deltaRotation;
        rootPart.worldRotation = rootPart.rotation;
        parts[0][0] = rootPart;

        float scale = 1f;
        for (int li = 1; li < parts.Length; li++) 
        {
            FractalPart[] parentParts = parts[li - 1];
            FractalPart[] levelParts = parts[li];
            for (int fpi=0; fpi < levelParts.Length; fpi++) 
            {
                FractalPart parent = parentParts[fpi / 5];
                FractalPart part = levelParts[fpi];
                part.rotation *= deltaRotation;
                part.worldRotation = parent.worldRotation*part.rotation; 
                part.worldPosition = parent.worldPosition + parent.worldRotation*(1.50f*part.worldRotation.x * part.direction);
                levelParts[fpi] = part;
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
