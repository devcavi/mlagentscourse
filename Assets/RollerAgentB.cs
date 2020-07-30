using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RollerAgentB : Agent
{
	Rigidbody rBody;
	public Transform target;
	public float speed = 250;
    public bool randomVel = true;
	float speedRand;
    bool touchingGround;
    const string k_Ground = "Floor"; // Tag of ground object.

    

	void Start () {
		rBody = GetComponent<Rigidbody>();
        speedRand = Random.value*(speed-30.0f) + 30.0f;
	}

	public override void OnEpisodeBegin()
	{
		if (this.transform.localPosition.y < 0 || this.transform.localPosition.y > 10)
		{
		    // Se o agente cair da plataforma, o seu momento é zero.
		    this.rBody.angularVelocity = Vector3.zero;
		    this.rBody.velocity = Vector3.zero;
		    this.transform.localPosition = new Vector3( 0, 0.5f, 0);
			if (randomVel)
            	speedRand = Random.value*(speed-30.0f) + 30.0f; //minimo 30.0f

		}

		// Move o alvo para um novo ponto
        target.localPosition = new Vector3(Random.value * 8 - 4,
				                   Random.value*2.0f + 0.5f,
				                   Random.value * 8 - 4);	
    }

    void OnCollisionEnter(Collision col)
	{
		if (col.transform.CompareTag(k_Ground)){
			touchingGround = true;

        }
	}

	void OnCollisionExit(Collision col)
	{
		if (col.transform.CompareTag(k_Ground)){
        	touchingGround = false;
        }
	}


	public override void CollectObservations(VectorSensor sensor)
	{
        sensor.AddObservation(touchingGround ? 1 : 0);
	    // Adicionando as posições do agente e do alvo para serem observadas.
	    sensor.AddObservation(target.localPosition);
	    sensor.AddObservation(this.transform.localPosition);

	    // Adicionando a velocidade do agente para ser observada.
		sensor.AddObservation(rBody.velocity);
        if(randomVel){
            sensor.AddObservation(speedRand/speed);
        } else {
            sensor.AddObservation(speed/250.0f);
        } 				
	}

	public override void OnActionReceived(float[] vectorAction)
	{
	    // Vetor de ações com tamanho dois.
	    Vector3 controlSignal = Vector3.zero;
	    controlSignal.x = vectorAction[0];
	    controlSignal.y = vectorAction[1];
		controlSignal.z = vectorAction[2];
		Vector3 force_app = (controlSignal * speed);

		if(randomVel){
			force_app = (controlSignal * speedRand);
		}

		// Adicionando uma força ao agente de acordo com os valores retornados do vetor ação da rede neural.
	    rBody.AddForce(force_app);

	    // Distância entre o agente e o alvo.
	    float distanceTotarget = Vector3.Distance(this.transform.localPosition, target.localPosition);

		//Recompensas

        AddReward(-0.0001f);
		if(!touchingGround)
			AddReward(-0.005f);

        // Se atingir o alvo, o agente é recompensado positivamente.
        if (distanceTotarget  < 1.42f)
        {
            AddReward(1.0f);
            EndEpisode();
        }

        // Se o agente cair, o episódio se encerra, sem o agente ser recompensado.
        if (this.transform.localPosition.y < 0 || this.transform.localPosition.y > 10)
        {
            SetReward(-0.5f);
            EndEpisode();		
        }
		
		// Fim das Recompensas
	}

}
