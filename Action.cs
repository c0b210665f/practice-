using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    private CarAgent carAgent;
    private Evaluator evaluator = Evaluator.getInstance();
    private CarInformation carInformation;
    private RewardCalculation rewardCalculation;

    public Action(CarAgent carAgent)
    {
        this.carAgent = carAgent;
        this.carInformation = carAgent.carInformation;
        this.rewardCalculation = carAgent.rewardCalculation;
    }

    public void ActionProcess(float[] vectorAction)
    {
        var lastPos = carAgent.transform.position;  // carAgentの現在の位置をlastPosに保存

        if (carAgent.generateNew)
        {
            carAgent.time++;
        }

        // 車両の停止時間と共通報酬のインターバルを更新
        if (carAgent.id == 0)
        {
            carInformation.CarInformationController(carAgent.stopTime, carAgent.commonRewardInterval);
        }
        // 
        if (carInformation.rewardTime >= carAgent.commonRewardInterval && carAgent.canGetCommonReward)
        {
            float commonReward = rewardCalculation.CalculateCommonReward();  //rewardCalculationwotukatte 共通報酬を計算
            carAgent.AddReward(commonReward);                                // carAgentに報酬を追加
        }
        else if (!carAgent.canGetCommonReward && carInformation.rewardTime < carAgent.commonRewardInterval)
        {
            carAgent.canGetCommonReward = true;
        }

        float horizontal = vectorAction[0];
        float vertical = vectorAction[1];
        vertical = Mathf.Clamp(vertical, -1.0f, 1.0f);
        horizontal = Mathf.Clamp(horizontal, -1.0f, 1.0f);

        carAgent.movement.MoveCar(horizontal, vertical, Time.fixedDeltaTime);     // carAgentの移動オブジェクトに対して、水平方向と垂直方向の操作血を与えて車両を移動させる

        float individualReward = rewardCalculation.CalculateIndividualReward();   // 個別報酬を計算するためにrewardCalculationを使用

        var moveVec = carAgent.transform.position - lastPos;
        float angle = Vector3.Angle(moveVec, carAgent.currentTrack.forward);
        float angleReward = rewardCalculation.CalculateAngleReward(moveVec, angle, vertical);

        carAgent.AddReward(individualReward + angleReward);
        // 車両が後方に車を見つけるかつ側方には車を見つけていない場合
        if (carAgent.foundCarBackward && !carAgent.foundCarSide)
        {
            evaluator.addBehavior(Time.realtimeSinceStartup, (int)carAgent.speed, false, vectorAction);
        }
        // 車両が前方に車を見つけるかつ側方には車を見つけていない場合
        if (carAgent.foundCarForward && !carAgent.foundCarSide)
        {
            evaluator.addBehavior(Time.realtimeSinceStartup, (int)carAgent.speed, true, vectorAction);
        }
        // 全体のデータをevaluatorに追加
        evaluator.addFullData(Time.frameCount, carAgent.transform.position, carAgent.previousObservations, horizontal, vertical);
    }
}