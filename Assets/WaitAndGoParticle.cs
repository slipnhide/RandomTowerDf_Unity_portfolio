using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAndGoParticle : MonoBehaviour
{
    ParticleSystem particleSystem;
    ParticleSystem.Particle[] particles;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }
    [ContextMenu("Move")]
    void St()
    {
        Set(1, Vector3.zero);
    }

    public virtual void Set(float WaitTime, Vector3 Pos)
    {
        StartCoroutine(Go(WaitTime, Pos));
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
              //  Debug.Log(particles[i].position);
              //  Debug.Log(Vector2.Lerp(particles[i].position, new Vector3(_Pos.x, _Pos.y, 0), 0.1f));
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
