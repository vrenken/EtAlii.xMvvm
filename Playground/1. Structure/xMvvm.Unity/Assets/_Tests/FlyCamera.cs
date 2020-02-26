﻿namespace EtAlii.xMvvm
{
    using UnityEngine;

    public class FlyCamera : MonoBehaviour {
     
        /*
        Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
        Converted to C# 27-02-13 - no credit wanted.
        Simple flycam I made, since I couldn't find any others made public.  
        Made simple to use (drag and drop, done) for regular keyboard layout  
        wasd : basic movement
        shift : Makes camera accelerate
        space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/
         
         
        float mainSpeed = 100.0f; //regular speed
        float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
        float maxShift = 1000.0f; //Maximum speed when holdin gshift
        float camSens = 0.25f; //How sensitive it with mouse
        private Vector3 _lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
        private float _totalRun= 1.0f;
         
        void Update ()
        {

            var currentTransform = transform;
            var eulerAngles = currentTransform.eulerAngles;
            _lastMouse = Input.mousePosition - _lastMouse ;
            _lastMouse = new Vector3(-_lastMouse.y * camSens, _lastMouse.x * camSens, 0 );
            _lastMouse = new Vector3(eulerAngles.x + _lastMouse.x , eulerAngles.y + _lastMouse.y, 0);
            currentTransform.eulerAngles = _lastMouse;
            _lastMouse =  Input.mousePosition;
            //Mouse  camera angle done.  
           
            //Keyboard commands
            Vector3 p = GetBaseInput();
            if (Input.GetKey (KeyCode.LeftShift)){
                _totalRun += Time.deltaTime;
                p  *= _totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else{
                _totalRun = Mathf.Clamp(_totalRun * 0.5f, 1f, 1000f);
                p = p * mainSpeed;
            }
           
            p = p * Time.deltaTime;
           Vector3 newPosition = transform.position;
            if (Input.GetKey(KeyCode.Space)){ //If player wants to move on X and Z axis only
                transform.Translate(p);

                var position = currentTransform.position;
                newPosition.x = position.x;
                newPosition.z = position.z;
                currentTransform.position = newPosition;
            }
            else{
                transform.Translate(p);
            }
           
        }
         
        private Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.
            var velocity = new Vector3();
            if (Input.GetKey (KeyCode.W)){
                velocity += new Vector3(0, 0 , 1);
            }
            if (Input.GetKey (KeyCode.S)){
                velocity += new Vector3(0, 0, -1);
            }
            if (Input.GetKey (KeyCode.A)){
                velocity += new Vector3(-1, 0, 0);
            }
            if (Input.GetKey (KeyCode.D)){
                velocity += new Vector3(1, 0, 0);
            }
            return velocity;
        }
    }
}
