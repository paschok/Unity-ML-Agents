using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.Timeline;
using UnityEngine;

public class RectChaser : Agent
{
    [SerializeField] private Transform targetTransform;
    public float moveSpeed = 1f;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-3f, 2f), 0, Random.Range(-3f, 3f));
        targetTransform.localPosition = new Vector3(Random.Range(2.2f, 4f), 0, Random.Range(-3f, 3f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Target>(out Target target)) {
            SetReward(1f);
            EndEpisode();
        }
         if (other.TryGetComponent<Wall>(out Wall wall)) {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continiousAction = actionsOut.ContinuousActions;

        continiousAction[0] = Input.GetAxisRaw("Horizontal");
        continiousAction[1] = Input.GetAxisRaw("Vertical");
    }
}
