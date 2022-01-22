using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ProcessInputDataSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //Temporary variables cause Strings cannot be used
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        bool inputA = Input.GetKey(KeyCode.Space);
        
        Entities.ForEach((ref Entity player, ref RawInputData inputD, ref PlayerMoveComponent pMovement) => {
            //Set Input Data
            inputD.inputH = inputH;
            inputD.inputV = inputV;
            //Direction depends on input
            pMovement.targetDirection = new Unity.Mathematics.float3(inputD.inputH, 0, inputD.inputV);
        }).Schedule();

        Entities.ForEach((ref EnemyTag eTag) => {
            //Set Input Data
            eTag.isPlayerAttacking = inputA;
            
        }).Schedule();
        Debug.Log("Player attacking = " + inputA);
    }
}
