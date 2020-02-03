﻿using System.Collections;
using SDD.Events;
using Spline;
using UnityEngine;

namespace DefaultNamespace
{
    public class AIController : SimpleGameStateObserver
    {
        private Car currentCar;
        private Racer currentRacer;
        private Circuit currentCircuit;
        private BezierSpline circuitSpline;
        private int steps = 2000; 
        private float nextStep = 20;
        private Vector3 nextPosition;
        private bool IsMoving = false;
	
        #region MonoBehaviour lifecycle
        // Use this for initialization
        void Start ()
        {
            currentCar = GetComponent<Car>();
            currentRacer = GetComponent<Racer>();
            currentCircuit = GameManager.Instance.CurrentCircuit;
            circuitSpline = currentCircuit.LevelBaseSpline;
            currentCar.CurrentSpeed = 10;
            nextPosition = circuitSpline.GetPoint(nextStep / circuitSpline.GetTotalLength());
            nextPosition.y = 0.5f;
        }

        IEnumerator MoveCoroutine()
        {
            float timeElapsed = 0;
            IsMoving = true;

            while (timeElapsed < 10)
            {
                Debug.Log(timeElapsed);
                timeElapsed += Time.deltaTime;
                transform.position = transform.position + nextPosition.normalized;
                //Vector3.Lerp(transform.position, nextPosition, timeElapsed / 10);
                yield return null;
            }
            
            yield return IsMoving = false;     
        }

        // Update is called once per frame
        private void Update()
        {
            if (GameManager.Instance && !GameManager.Instance.IsPlaying) return;

            /*if (vInput > 0f)
            {
                currentCar.Accelerate();
            }
            else if (currentCar.CurrentSpeed > 0f)
            {
                if (vInput < 0f)
                {
                    currentCar.Decelerate(3f);
                }
                else
                {
                    currentCar.Decelerate();
                }
            }*/

            //transform.position = (nextPosition * Time.deltaTime);
            //transform.Rotate(nextPosition * Time.deltaTime);
            //}
            //else
            //{
            if (!IsMoving) {
                nextStep += 1;
                nextPosition = circuitSpline.GetPoint(nextStep / circuitSpline.GetTotalLength());
            }
            
            StartCoroutine(MoveCoroutine());
            //}
            /*else
            {
                nextPosition = circuitSpline.GetPoint(currentRacer.CurrentDistance + 5 % circuitSpline.GetTotalLength() / circuitSpline.GetTotalLength());
            }*/
            //Vector3 nextPosition = circuitSpline.GetPoint((Time.deltaTime % steps) / steps);

            //Vector3 nextPoint = circuitSpline.GetPoint(
            //(nextStep % circuitSpline.GetTotalLength()) / circuitSpline.GetTotalLength());
            //if (Vector3.Distance(transform.position,nextPoint) > 0f){
            //currentCar.CurrentSpeed += (currentCar.AccelerationRate * Time.deltaTime);
            //transform.Translate(nextPoint * Time.deltaTime);
            //transform.position = nextPoint;
            //}
            //else
            //{
            //currentCar.CurrentSpeed -= (currentCar.DecelerationRate * Time.deltaTime);
            //nextStep += 1;
            //}
        }
        #endregion
    }
}