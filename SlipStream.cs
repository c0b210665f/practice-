using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipStream
{
    private CarAgent carAgent;
    private RewardCalculation rewardCalculation;
    public SlipStream(CarAgent carAgent)
    {
        this.carAgent = carAgent;
        this.rewardCalculation = carAgent.rewardCalculation;
    }

    public void JudgeSlipStream(string tag ,float distanceObservedObject)
    {
        if (carAgent.foundTruckBackward)
        {
            float carXPosition = carAgent.transform.position.x;
            if (carXPosition >= -3f && carXPosition <= -1f)
            {
                Debug.Log("SlipStream");
                Debug.Log("slipStreamReward :" + carAgent.slipStreamReward );
                if(distanceObservedObject <= 5)
                {
                    carAgent.AddReward(carAgent.rewardWeight*carAgent.slipStreamReward);
                    Debug.Log("slipReward" + carAgent.slipStreamReward);
                }else
                {
                    carAgent.AddReward(carAgent.rewardWeight*carAgent.subSlipStreamReward);
                    Debug.Log("slipReward" + carAgent.subSlipStreamReward);
                }
                    
            }
            else if(carXPosition < -3f && carXPosition > -1f)
            {
                Debug.Log("unSlipStream");
                carAgent.AddReward(0);
                carAgent.foundTruckBackward = false;
            }
            else if(tag == null)
            {
                Debug.Log("unSlipStream");
                carAgent.AddReward(0);
                carAgent.foundTruckBackward = false;
            }
        }
    }
}
