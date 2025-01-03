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
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using BTD_Mod_Helper.Api.Helpers;

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
    private static readonly ModSettingBool TowerCollides = new(true);

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
                    //Debug.Log("TimeScale: " + TimeHelper.OverrideFastForwardTimeScale);
                    for (int i2 = 0; i2 <1 * MovmentSpeedMultiplier; i2++)
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
                        List<Tower> towers2 = InGame.instance.GetTowers();
                        if (TowerCollides)
                        {
                            int TC2 = towers2.Count;
                            //ModHelper.Msg<Monkeywalkaround>(towers[i].towerModel.radius);
                            for (int iT = 0; iT < TC2; iT++)
                            {
                                if (towers2[iT] != null && TC2 >= 2)
                                {
                                    if (towers2[iT].Position.X != towers[i].Position.X)
                                    {
                                        float maxDistance = 3 + towers2[iT].towerModel.radius;
                                        UnityEngine.Vector2 thisPosition = new UnityEngine.Vector2(newPos.x, newPos.y);
                                        UnityEngine.Vector2 otherPosition = new UnityEngine.Vector2(towers2[iT].Position.X, towers2[iT].Position.Y);

                                        // Berechne die Distanz zwischen den Positionen
                                        float distance = UnityEngine.Vector2.Distance(thisPosition, otherPosition);

                                        // Wenn die Distanz kleiner oder gleich der maximalen Entfernung ist
                                        if (distance <= maxDistance)
                                        {
                                            //ModHelper.Msg<Monkeywalkaround>("distance: " + distance + " x1: " + newPos.x + " x2: " + towers2[iT].Position.X);
                                            towers[i].RotateTower(rnd(-90, 90), false, false);
                                            allowedToMove = false;
                                        }
                                    }
                                }
                            }
                        }
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