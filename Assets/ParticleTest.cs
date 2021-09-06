using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTest : MonoBehaviour
{
    ParticleSystem particleSystem;
    ParticleSystem.Particle[] particles;
    Vector2 Pos;
    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }
    void Start()
    {
       // StartCoroutine(Circle());
    }
    public void Set(Vector2 _Pos)
    {
        Pos = _Pos;
        StartCoroutine(Circle());
    }

    // Update is called once per frame
    void Update()
    {
        //foreach (ParticleSystem.Particle particle in particles)
        //{
        //    particle.position = Vector2.Lerp(particle.position, new Vector2(10, 10), 0.1f);
        //}
        //for (int i = 0; i < particleSystem.GetParticles(particles); i++)
        //{
        //    //particles[i].position = Vector2.Lerp(particles[i].position, new Vector2(10, 10), 0.1f);
        //    particles[i].velocity += new Vector3(10, 10,0);
        //}
        //particleSystem.SetParticles(particles, particleSystem.GetParticles(particles));
        //int num = particleSystem.GetParticles(particles);
        //Debug.Log(num);
        //for (int i = 0; i < num; i++)
        //{
        //    particles[i].position = Vector2.Lerp(particles[i].position,Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.01f);
        //    Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //   // particles[i].velocity += new Vector3(10, 10, 0);
        //}
        //particleSystem.SetParticles(particles, num);
    }
    IEnumerator Circle()
    {
        float Radius=0.1f;
        float Degree = 0;
        Vector3 Base = transform.position;
        while (true)
        {
            int num = particleSystem.GetParticles(particles);
            //bug.Log(num);
            for (int i = 0; i < num; i++)
            {
                Vector3 Move = Base + (Radius * new Vector3(Mathf.Cos(Degree * Mathf.Deg2Rad), Mathf.Sin(Degree * Mathf.Deg2Rad), 0));
               // Debug.Log(Degree + "  " + Radius+ "  Base: "+ Base+ "  "+(Radius * new Vector3(Mathf.Cos(Degree * Mathf.Deg2Rad), Mathf.Sin(Degree * Mathf.Deg2Rad), 0)));
                particles[i].position = Vector2.Lerp(particles[i].position, Move, 0.01f);
               // particles[i].position += Vector3.up;
              //  Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                // particles[i].velocity += new Vector3(10, 10, 0);
            }
            particleSystem.SetParticles(particles, num);
            if(Radius<5)
            Radius += 0.3f;
            Degree += 10f;
            if (Degree > 360*2)
                break;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(Go(0, Pos));
    }
    IEnumerator Go(float WaitTime, Vector3 Pos)
    {
        yield return new WaitForSeconds(WaitTime);

        // Vector3 Base = new Vector3(10,10,0);
        Camera Mc = Camera.main;
        ParticleSystem.ShapeModule dd = particleSystem.shape;
        // Vector3 _Pos;


        Vector3 _Pos = Mc.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("전달받은 Pos : " + Pos);
        if (Camera.main.orthographic)
        {
            Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            _Pos = Camera.main.ScreenToWorldPoint(Pos);
        }
        else
        {

            _Pos = Pos;
            Debug.Log(_Pos);
            // _position.z = Camera.main.farClipPlane;
            //_position.z = Camera.main.nearClipPlane;
            _Pos.z = 10;
            _Pos = Camera.main.ScreenToWorldPoint(_Pos);
            Debug.Log(_Pos);
        }

        while (true)
        {
            int num = particleSystem.GetParticles(particles);
            for (int i = 0; i < 1; i++)
            {
                particles[i].position = Vector2.Lerp(particles[i].position, new Vector3(_Pos.x, _Pos.y, 0), 0.1f);
                if (Vector2.Distance(particles[i].position, new Vector3(_Pos.x, _Pos.y, 0)) < 0.5f)
                {
                    particles[i].remainingLifetime = 0;
                }
            }
            particleSystem.SetParticles(particles, num);
            if (particleSystem.GetParticles(particles) == 0)
            {
                Debug.Log("brak;");
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
