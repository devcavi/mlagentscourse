using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RollerAgentA : Agent
{
	Rigidbody rBody;
	public Transform target;
	public float speed = 10;


	void Start () {
		rBody = GetComponent<Rigidbody>();
	}

	public override void OnEpisodeBegin()
	{
		if (this.transform.localPosition.y < 0)
		{
		    // Se o agente cair da plataforma, o seu momento é zero.
		    this.rBody.angularVelocity = Vector3.zero;
		    this.rBody.velocity = Vector3.zero;
		    this.transform.localPosition = new Vector3( 0, 0.5f, 0);
		}

		// Move o alvo para um novo ponto
		target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
	}

	public override void CollectObservations(VectorSensor sensor)
	{
	    // Adicionando as posições do agente e do alvo para serem observadas.
	    sensor.AddObservation(target.localPosition);
	    sensor.AddObservation(this.transform.localPosition);

	    // Adicionando a velocidade do agente para ser observada.
		sensor.AddObservation(rBody.velocity.x);
	    sensor.AddObservation(rBody.velocity.z);
	}

	public override void OnActionReceived(float[] vectorAction)
	{
	    // Vetor de ações com tamanho dois.
	    Vector3 controlSignal = Vector3.zero;
	    controlSignal.x = vectorAction[0];
	    controlSignal.z = vectorAction[1];

		// Adicionando uma força ao agente de acordo com os valores retornados do vetor ação da rede neural.
	    rBody.AddForce(controlSignal * speed);

	    // Distância entre o agente e o alvo.
	    float distanceTotarget = Vector3.Distance(this.transform.localPosition, target.localPosition);

		//Recompensas

        // Se atingir o alvo, o agente é recompensado positivamente.
        if (distanceTotarget < 1.42f)
        {
        SetReward(1.0f);
        EndEpisode();
        }

        // Se o agente cair, o episódio se encerra, sem o agente ser recompensado.
        if (this.transform.localPosition.y < 0)
        {
        EndEpisode();
        }
		
		// Fim das Recompensas
	}

}
