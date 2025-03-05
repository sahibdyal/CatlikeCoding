using UnityEngine;
using static UnityEngine.Mathf;
public static class Wave
{

    public delegate Vector3 Function(float u,float v, float t);
    static Function[] functions = { WaveFunc,MixedWave,Ripple,Sphere,Torus};

    public enum FunctionName {WaveFunc,MixedWave,Ripple,Sphere,Torus}


    public static Function GetFunction(FunctionName index) 
    {
        return functions[(int)index];
    }

    public static FunctionName GetNextFunction(FunctionName name) 
    {
        return (int)name < functions.Length-1 ? name + 1 : 0;
        
    }

    public static FunctionName GetRandomFunction(FunctionName name) 
    {
        var choice = (FunctionName)Random.Range(0,functions.Length);
        return choice == name?0:choice;
    }

    public static Vector3 WaveFunc(float u,float v,float t) 
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;
        return p;
    }

    public static Vector3 MixedWave(float u,float v, float t)
    {
        Vector3 p;
        p.y = Sin(PI * (u + (0.5f*t)));
        p.y += Sin(2f * PI * (v+t))*(1f/2f);
        p.y += Sin(PI * (u+v+0.25f*t))*(1f/2f);
        p.x = u;
        p.z = v;

        return p;
    }

    public static Vector3 Ripple(float u, float v,float t) 
    {
        float d = Sqrt(u*u+v*v);
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (4f * d) - t);
        p.y = p.y / (1 + 10f*d);
        p.z = v;
        return p;
    }

    public static Vector3 Sphere(float u,float v,float t) 
    {
        float r = 0.9f+0.1f*Sin(PI*(8f*u +6f*v+t));
        float s = r*Cos(0.5f*PI*v); 
        Vector3 p;
        p.x = s*Sin(PI*u);
        p.y = r*Sin(0.5f*PI*v);
        p.z = s*Cos(PI*u);
        return p;

    }

    public static Vector3 Torus(float u , float v, float t) 
    {
        float r1 = 0.55f + 0.15f*Sin(PI*(6f*u+0.5f*t));
        float r2 = 0.15f+ 0.15f*Sin(PI*(8f*u + 6f*v +t));
        float s = r1+r2 * Cos(PI * v);
        Vector3 p;
        p.x = s*Sin(PI*u);
        p.y = r2*Sin(PI*v);
        p.z = s*Cos(PI*u);
        return p;

    }

    public static Vector3 Morph(float u, float v,float t,Function from, Function to,float progress) 
    {
        return Vector3.LerpUnclamped(from(u,v,t),to(u,v,t),SmoothStep(0f,1f,progress));
    }
        
}
