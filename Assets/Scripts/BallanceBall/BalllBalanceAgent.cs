﻿using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BalllBalanceAgent : Agent
{
    public GameObject ball;
    Rigidbody ballRigidbody;
    EnvironmentParameters defaultParameters;
    public override void Initialize()
    {
        ballRigidbody = ball.GetComponent<Rigidbody>();
        defaultParameters = Academy.Instance.EnvironmentParameters;
        ResetScene();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(ballRigidbody.velocity);
        sensor.AddObservation(ball.transform.position);
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(transform.rotation.x);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var zangle = 2f * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        var xangle = 2f * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        if ((gameObject.transform.rotation.z < 0.25f && zangle > 0f) ||
            (gameObject.transform.rotation.z > -0.25f && zangle < 0f))
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 1), zangle);
        }

        if ((gameObject.transform.rotation.x < 0.25f && xangle > 0f) ||
            (gameObject.transform.rotation.x > -0.25f && xangle < 0f))
        {
            gameObject.transform.Rotate(new Vector3(1, 0, 0), xangle);
        }
        if ((ball.transform.position.y - gameObject.transform.position.y) < -2f ||
            Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f ||
            Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            SetReward(0.1f);
        }
    }

    public override void OnEpisodeBegin()
    {
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        ballRigidbody.velocity = new Vector3(0f, 0f, 0f);
        ball.transform.position = new Vector3(Random.Range(-1.5f, 1.5f), 4f, Random.Range(-1.5f, 1.5f))
            + gameObject.transform.position;
        ResetScene();
    }

    void ResetScene()
    {
        ballRigidbody.mass = defaultParameters.GetWithDefault("mass", 1.0f);
        var scale = defaultParameters.GetWithDefault("scale", 1.0f);
        ball.transform.localScale = new Vector3(scale, scale, scale);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(actionsOut);
    }
}
