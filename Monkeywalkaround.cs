using MelonLoader;
using BTD_Mod_Helper;
using Monkeywalkaround;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using BTD_Mod_Helper.Extensions;
using System.Collections.Generic;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.SMath;
using UnityEngine;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Utils;
using System.Xml.Linq;
using Il2CppAssets.Scripts;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.ModOptions;

[assembly: MelonInfo(typeof(Monkeywalkaround.Monkeywalkaround), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace Monkeywalkaround;

public class Monkeywalkaround : BloonsTD6Mod
{
    private static float MovmentSpeed = 0.6f;
    private static readonly ModSettingFloat MovmentSpeedMultiplier = new(1f)
    {
        min = 0,
        max = 10,
        stepSize = 0.1f,
        slider = true,
        description = "Controls the Movement Speed of your Towers"

    };

    static int rnd(int min, int max)
    {
        System.Random random = new System.Random();
        return random.Next(min, max);
    }
    public override void OnApplicationStart()
    {
        ModHelper.Msg<Monkeywalkaround>("Monkey walks around loaded!");
    }
    public Il2CppAssets.Scripts.Simulation.SMath.Vector2 CalculateNewPosition(float oldPosX, float oldPosY, float oldPosZ, float directionDegrees)
    {
        // Umwandlung des Winkels von Grad in Radiant
        float directionRadians = directionDegrees * Mathf.Deg2Rad;

        // Berechnung der neuen X und Z Positionen
        float newX = oldPosX - Mathf.Cos(directionRadians) * MovmentSpeed; // 1.0f ist die Entfernung in Metern
        float newZ = oldPosZ + Mathf.Sin(directionRadians) * MovmentSpeed;

        return new Il2CppAssets.Scripts.Simulation.SMath.Vector2(newX, newZ);
    }
    public override void OnUpdate()
    {
        if (InGameData.CurrentGame != null && InGame.Bridge != null)
        {
            List<Tower> towers = InGame.instance.GetTowers();
            int TC = towers.Count;

            for (int i = 0; i < TC; i++)
            {
                if (towers[i] != null)
                {
                    for(int i2 = 0; i2<TimeManager.networkScale * MovmentSpeedMultiplier; i2++)
                    {
                        Il2CppAssets.Scripts.Simulation.SMath.Vector2 newPos = CalculateNewPosition(towers[i].Position.X, towers[i].Position.Z, towers[i].Position.Y, towers[i].Rotation + 90);
                        bool allowedToMove = true;
                        // Check if tower is within the allowable Y-axis upper bound and move upwards
                        if (!(newPos.y >= -109))
                        {
                            towers[i].RotateTower(rnd(-90, 90), false, false);
                            allowedToMove = false;
                        }

                        // Check if tower is within the allowable Y-axis lower bound and move downwards
                        if (!(newPos.y <= 114))
                        {
                            towers[i].RotateTower(rnd(-90, 90), false, false);
                            allowedToMove = false;
                        }

                        // Check if tower is within the allowable X-axis left bound and move left
                        if (!(newPos.x >= -145))
                        {
                            towers[i].RotateTower(rnd(-90, 90), false, false);
                            allowedToMove = false;
                        }

                        // Check if tower is within the allowable X-axis right bound and move right
                        if (!(newPos.x <= 145))
                        {
                            towers[i].RotateTower(rnd(-90, 90), false, false);
                            allowedToMove = false;
                        }
                        //TowerModel dartMonkey = Game.instance.model.GetTowerWithName(TowerType.DartMonkey).Duplicate();
                        //ModHelper.Msg<Monkeywalkaround>("id: " + InGame.instance.bridge.MyPlayerNumber + "  bool: " + InGame.Bridge.CanPlaceTowerAt(new UnityEngine.Vector2(newPos.x, newPos.y), dartMonkey, InGame.instance.bridge.MyPlayerNumber, ObjectId.FromString("1231")));
                        
                        
                        /*if (InGame.Bridge.CanPlaceTowerAt(new UnityEngine.Vector2(newPos.x, newPos.y), dartMonkey, InGame.instance.bridge.MyPlayerNumber, ObjectId.FromString("1231")))
                        {
                            towers[i].RotateTower(rnd(-90, 90), false, false);
                            allowedToMove = false;
                        }*/
                        if (allowedToMove)
                        {
                            towers[i].PositionTower(newPos);
                        }
                    }
                }
            }
        }
    }
}