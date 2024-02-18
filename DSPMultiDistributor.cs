using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]



namespace DSPMultiDistributor
{
    [BepInPlugin("Appun.DSP.plugin.MultiDistributor", "DSPMultiDistributor", "0.0.2")]
    [HarmonyPatch]
	public class Main : BaseUnityPlugin
	{

        //public static float magnificationRate = 0.8f;

        public void Awake()
        {
            LogManager.Logger = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

		}


        [HarmonyPrefix, HarmonyPatch(typeof(UIDispenserWindow), "_OnClose")]
        public static void UIDispenserWindow_OnClose_Prefix2(UIDispenserWindow __instance)
        {
            GameMain.data.localLoadedPlanetFactory.transport.RefreshDispenserTraffic();

        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlanetTransport), "RefreshDispenserTraffic")]
        public static void PlanetTransport_RefreshDispenserTraffic_Postfix(PlanetTransport __instance, int keyId)
        {
            for (int l = 1; l < __instance.dispenserCursor; l++)
            {
                DispenserComponent dispenserComponent2 = __instance.dispenserPool[l];
                    if (dispenserComponent2 != null && dispenserComponent2.storage != null && dispenserComponent2.id == l)
                    {
                        int filter = dispenserComponent2.filter;
                        if (filter == 0)
                        {

                            if (dispenserComponent2.storageMode == EStorageDeliveryMode.Supply)
                            {
                                for (int p = 0; p < dispenserComponent2.storage.size; p++)
                                {
                                    //int itemId = dispenserComponent2.storage.grids[p].itemId;
                                    LogManager.Logger.LogInfo("(----------------------------UIDispenserWindow 5");
                                    int filter2 = dispenserComponent2.storage.grids[p].filter;
                                    if (filter2 > 0)
                                    //     if (itemId > 0)
                                    {
                                        for (int m = 1; m < __instance.dispenserCursor; m++)
                                        {
                                            LogManager.Logger.LogInfo("(----------------------------UIDispenserWindow 6");
                                            DispenserComponent dispenserComponent3 = __instance.dispenserPool[m];
                                            if (dispenserComponent3 != null && dispenserComponent3.id == m && dispenserComponent3.storageMode == EStorageDeliveryMode.Demand && dispenserComponent3.filter == filter2)
                                            {
                                                dispenserComponent2.AddPair(l, 0, m, 0);
                                                dispenserComponent3.AddPair(l, 0, m, 0);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (dispenserComponent2.storageMode == EStorageDeliveryMode.Demand)
                            {
                                for (int p = 0; p < dispenserComponent2.storage.size; p++)
                                {
                                    int filter2 = dispenserComponent2.storage.grids[p].filter;
                                    if (filter2 > 0)
                                    {
                                        for (int n = 1; n < __instance.dispenserCursor; n++)
                                        {
                                            DispenserComponent dispenserComponent4 = __instance.dispenserPool[n];
                                            if (dispenserComponent4 != null && dispenserComponent4.id == n && dispenserComponent4.storageMode == EStorageDeliveryMode.Supply && dispenserComponent4.filter == filter2)
                                            {
                                                dispenserComponent2.AddPair(n, 0, l, 0);
                                                dispenserComponent4.AddPair(n, 0, l, 0);
                                            }
                                        }
                                    }
                                }
                            }
                            int logisticCourierCarries = GameMain.history.logisticCourierCarries;
                            dispenserComponent2.OnRematchPairs(__instance.factory, __instance.dispenserPool, keyId, logisticCourierCarries);
                        }
                    }



            }
        }

        public class LogManager
        {
            public static ManualLogSource Logger;
        }
    }
}