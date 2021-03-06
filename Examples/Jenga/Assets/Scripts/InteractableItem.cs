﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cave;

public class InteractableItem : CollisionSynchronization
{


    private new Rigidbody rigidbody;

    private bool currentlyInteracting;

    private FlyStickSim attachedWandSim;
    private FlyStickInteraction attachedWand;


    private Transform interactionPoint;

    private Vector3 posDelta;

    private Quaternion rotationDelta;

    private float angle;

    private Vector3 axis;

    public float rotationFactor = 400f;
    public float velocityFactor = 200f;

    public InteractableItem()
        : base(new[] { Cave.EventType.OnCollisionEnter })
    {

    }

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        velocityFactor /= rigidbody.mass;
        rotationFactor /= rigidbody.mass;

        GameObject interactionObject = GameObject.Find("InteractionObject");

        if (interactionObject != null)
        {
            interactionPoint = interactionObject.transform;
        }
        else
        {
            Debug.Log("InteractionObject is missing!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (attachedWand && currentlyInteracting)
        {
            posDelta = attachedWand.transform.position - interactionPoint.position;
            this.rigidbody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime; //TODO change to networkTime

            rotationDelta = attachedWand.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (!float.IsInfinity(axis.x) && !float.IsInfinity(axis.y) && !float.IsInfinity(axis.z))
            {
                if (angle > 180)
                {
                    angle -= 360;
                }
                this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
            }
        }


        //TODO just for FlyStickSim, delete as Sim is no longer needed!
        else
        if (attachedWandSim && currentlyInteracting)
        {
            posDelta = attachedWandSim.transform.position - interactionPoint.position;
            this.rigidbody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime; //TODO change to networkTime

            rotationDelta = attachedWandSim.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
            {
                angle -= 360;
            }
            this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
        }
    }

    public void BeginInteraction(FlyStickSim wand)
    {
        attachedWandSim = wand;
        interactionPoint.position = wand.transform.position;
        interactionPoint.rotation = wand.transform.rotation;
        interactionPoint.SetParent(transform, true);

        currentlyInteracting = true;
    }

    public void EndInteraction(FlyStickSim wand)
    {
        if (wand == attachedWandSim) //not needed for us
        {
            attachedWandSim = null;
            currentlyInteracting = false;
        }
    }

    public void BeginInteraction(FlyStickInteraction wand)
    {
        attachedWand = wand;
        interactionPoint.position = wand.transform.position;
        interactionPoint.rotation = wand.transform.rotation;
        interactionPoint.SetParent(transform, true);

        currentlyInteracting = true;
    }

    public void EndInteraction(FlyStickInteraction wand)
    {
        if (wand == attachedWand) //not needed for us
        {
            attachedWand = null;
            currentlyInteracting = false;
        }
    }

    public bool isInteracting()
    {
        return currentlyInteracting;
    }

    private bool IsBrickInFirstRow()
    {
        return transform.position.z > 0 - transform.localScale.z / 2 && transform.position.z < transform.localScale.z * 3
               && transform.position.x > 0 && transform.position.x < transform.localScale.x
               && transform.position.y > 0 && transform.position.y < transform.localScale.y;
    }

    public override void OnSynchronizedCollisionEnter(GameObject other)
    {
        if (other.name == "Plane" && !IsBrickInFirstRow())
        {
            TowerInteractivity tower = FindObjectOfType<TowerInteractivity>();
            if (tower.state != TowerInteractivity.State.TowerCrashed && transform.GetComponent<Renderer>().material.color != Color.green)
            {
                tower.state = TowerInteractivity.State.TowerCrashed;
                tower.Players[Player.ActivePlayer].Score++;
                Debug.Log("Tower crashed at " + TimeSynchronizer.time);


                InfoScreenManager infoScreen = FindObjectOfType<InfoScreenManager>();
                infoScreen.LoserView();
            }
        }
    }
}