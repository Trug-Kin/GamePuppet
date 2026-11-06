#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// Verification processes only. OS suite and normal play never install this.
// Prevent desktop keyboard/mouse activity from contaminating scripted inputs.
public sealed class VerificationInputIsolationV43:MonoBehaviour
{
 readonly List<InputDevice> disabled=new List<InputDevice>();
 public void Begin(string output)
 {
  var rows=new JArray();
  foreach(var device in InputSystem.devices)
   if(device.native&&device.enabled&&(device is Keyboard||device is Mouse))
   {disabled.Add(device);rows.Add(new JObject{["name"]=device.name,["id"]=device.deviceId,["native"]=device.native});InputSystem.DisableDevice(device);}
  File.WriteAllText(Path.Combine(output,"automation-input-isolation.json"),new JObject{["scope"]="This verification process only; native InputSystem keyboard/mouse temporarily disabled, not OS devices. Not used by os suite or normal gameplay.",["disabled_native_devices"]=rows}.ToString());
 }
 void OnDestroy(){foreach(var device in disabled)if(device.added)InputSystem.EnableDevice(device);}
}

#endif
